using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    [SerializeField] private ParticleSystem mergeParticle;
    [SerializeField] private GameObject wheelBody;
    [SerializeField] private TextMeshPro levelText;

    public int Level = 1;
    public float CoreRadius => wheelMovementHandler.CoreRadius;

    private WheelMovementHandler wheelMovementHandler;

    private void Awake()
    {
        wheelMovementHandler = GetComponent<WheelMovementHandler>();
    }

    private void OnEnable()
    {
        WheelManager.Instance.Wheels.Add(this);
    }

    private void OnDestroy()
    {
        WheelManager.Instance.Wheels.Remove(this);
    }

    public void SetUp(int level, float scaleIncrementPerLevel)
    {
        //Adjust level settings
        Level = level;
        levelText.text = $"{Level}";

        //Adjust size settings
        float scaleValue = scaleIncrementPerLevel * (level - 1);

        wheelBody.transform.localScale = new Vector3(wheelBody.transform.localScale.x + scaleValue,
            wheelBody.transform.localScale.y + scaleValue, wheelBody.transform.localScale.z + scaleValue);
    }

    public void PlayMergeParticle()
    {
        mergeParticle.Play();
    }

    public void PlayIncomeEffect()
    {
        transform.DOComplete();
        transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f),0.3f).From();
    }
}