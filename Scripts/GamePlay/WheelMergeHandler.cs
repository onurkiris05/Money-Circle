using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using VP.Nest.Haptic;

public class WheelMergeHandler : MonoBehaviour
{
    [SerializeField] private SphereCollider wheelCoreCollider;
    [SerializeField] private float wheelMergeRadius = 1f;

    public int Level => myWheel.Level;
    public bool IsReplacing;

    private Rigidbody rigidbody;
    private WheelMovementHandler wheelMovementHandler;
    private Wheel myWheel;
    private bool canMerge;

    private void Awake()
    {
        myWheel = GetComponent<Wheel>();
        rigidbody = GetComponent<Rigidbody>();
        wheelMovementHandler = GetComponent<WheelMovementHandler>();
    }

    public void Set()
    {
        canMerge = true;
        wheelCoreCollider.enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wheel") && !canMerge && !IsReplacing && !rigidbody.isKinematic)
        {
            WheelMergeHandler otherWheel = collision.gameObject.GetComponent<WheelMergeHandler>();

            if (Vector3.Distance(transform.position, otherWheel.transform.position) > wheelMergeRadius)
            {
                IsReplacing = true;
                wheelMovementHandler.GoToAvailablePos();
                HapticManager.Haptic(HapticType.LightImpact);

                return;
            }


            if (otherWheel.Level == Level)
            {
                HapticManager.Haptic(HapticType.MediumImpact);

                otherWheel.Set();
                Set();

                //Lift up both wheels to avoid wrong spawner behaviours
                Vector3 spawnPos = otherWheel.transform.position;
                otherWheel.transform.position = new Vector3(otherWheel.transform.position.x,
                    otherWheel.transform.position.y, -5f);

                transform.position = new Vector3(transform.position.x, transform.position.y, -5f);

                //Handle merge animations
                transform.DOMove(otherWheel.transform.position, 0.15f).OnComplete(() =>
                {
                    otherWheel.transform.DOScale(new Vector3(0, 0, 0), 0.15f)
                        .OnComplete(() => Destroy(otherWheel.gameObject));

                    transform.DOScale(new Vector3(0, 0, 0), 0.15f)
                        .OnComplete(() =>
                        {
                            WheelManager.Instance.AddWheel(Level + 1, spawnPos, true);
                            Destroy(gameObject);
                        });
                });
            }
            else
            {
                wheelMovementHandler.GoToAvailablePos();
            }
        }
    }
}