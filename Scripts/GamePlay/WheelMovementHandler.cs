using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using VP.Nest.Haptic;

public class WheelMovementHandler : MonoBehaviour
{
    [SerializeField] private LayerMask checkLayer;
    [SerializeField] private SphereCollider mainCollider, wheelCoreCollider;
    [SerializeField] private MeshRenderer myMesh;
    [SerializeField] private Material transMat, originalMat;

    public float CoreRadius => wheelCoreCollider.radius;

    private Rigidbody rigidbody;
    private WheelMergeHandler wheelMergeHandler;
    private bool isReplacing;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        wheelMergeHandler = GetComponent<WheelMergeHandler>();
    }

    public void OnMove()
    {
        HapticManager.Haptic(HapticType.LightImpact);

        rigidbody.isKinematic = false;
        mainCollider.isTrigger = true;
        wheelCoreCollider.isTrigger = true;

        var matList = new List<Material>() { myMesh.materials[0], transMat };
        myMesh.materials = matList.ToArray();

        transform.position = new Vector3(transform.position.x, transform.position.y, -5f);
    }

    public void OnPlace()
    {
        HapticManager.Haptic(HapticType.LightImpact);

        Vector3 checkPos = new Vector3(transform.position.x, transform.position.y, 0f);

        wheelCoreCollider.isTrigger = false;
        mainCollider.isTrigger = false;

        var matList = new List<Material>() { myMesh.materials[0], originalMat };
        myMesh.materials = matList.ToArray();

        if (!Physics.CheckSphere(checkPos, wheelCoreCollider.radius, checkLayer))
        {
            transform.position = checkPos;
            rigidbody.isKinematic = true;
        }
        else
        {
            GoToAvailablePos();
        }
    }

    public void GoToAvailablePos()
    {
        if (!isReplacing)
        {
            isReplacing = true;

            List<Spawner> spawners = SpawnerManager.Instance.Spawners;
            List<Spawner> availableSpawners = new List<Spawner>();

            for (int i = 0; i < spawners.Count; i++)
            {
                if (!Physics.CheckSphere(spawners[i].transform.position, wheelCoreCollider.radius, checkLayer))
                {
                    availableSpawners.Add(spawners[i]);
                }
            }

            List<Spawner> sorted = availableSpawners.OrderBy(x =>
                Vector3.Distance(x.transform.position, transform.position)).ToList();

            transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
            transform.DOMove(sorted[0].transform.position, 10).SetSpeedBased(true).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    wheelMergeHandler.IsReplacing = false;
                    rigidbody.isKinematic = true;
                    isReplacing = false;
                });
        }
    }
}