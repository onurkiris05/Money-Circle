using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using DG.Tweening;
using ElephantSDK;
using UnityEngine;
using VP.Nest.Analytics;
using Random = UnityEngine.Random;

public class WheelManager : Singleton<WheelManager>
{
    [Header("Wheel Settings")]
    [SerializeField] private LayerMask checkLayerMask;
    [SerializeField] private Wheel wheelPrefab;
    [SerializeField] private float scaleIncrementPerLevel;

    public List<Wheel> Wheels = new List<Wheel>();

    private int mergeIndex = 1;

    private void Start()
    {
        scaleIncrementPerLevel = RemoteConfigManager.ScaleIncrementPerLevel;

        IncrementalManager.Instance.GetUpgradeCard(UpgradeType.Add).OnCurrencyPurchase += AddWheel;

        AnalyticsManager.LogLevelStartEvent(mergeIndex);
    }

    public void PlayIncomeEffects()
    {
        for (int i = 0; i < Wheels.Count; i++)
        {
            Wheels[i].PlayIncomeEffect();
        }
    }

    public void AddWheel()
    {
        Wheel wheel = Instantiate(wheelPrefab, transform);

        Vector3 spawnPos = GetAvailablePos(wheel.CoreRadius);

        wheel.transform.localScale = new Vector3(0, 0, 0);
        wheel.transform.position = spawnPos;
        wheel.transform.DOScale(new Vector3(1, 1, 1), 0.15f).SetEase(Ease.OutBack);
    }

    public void AddWheel(int level, Vector3 spawnPos, bool isParticlePlay = false)
    {
        Wheel wheel = Instantiate(wheelPrefab, transform);

        wheel.transform.localScale = new Vector3(0, 0, 0);
        wheel.transform.position = spawnPos;
        wheel.transform.DOScale(new Vector3(1, 1, 1), 0.15f).SetEase(Ease.OutBack);
        wheel.SetUp(level, scaleIncrementPerLevel);

        if (isParticlePlay)
        {
            wheel.PlayMergeParticle();

            if (mergeIndex < level)
            {
                AnalyticsManager.LogLevelCompleteEvent(mergeIndex);
                mergeIndex = level;
                AnalyticsManager.LogLevelStartEvent(mergeIndex);
            }
        }

        FTUEManager.Instance.Step3?.Invoke();
    }

    Vector3 GetAvailablePos(float radius)
    {
        List<Spawner> spawners = SpawnerManager.Instance.Spawners;
        List<Spawner> availableSpawners = new List<Spawner>();

        for (int i = 0; i < spawners.Count; i++)
        {
            if (!Physics.CheckSphere(spawners[i].transform.position, radius, checkLayerMask))
            {
                availableSpawners.Add(spawners[i]);
            }
        }

        List<Spawner> sorted = availableSpawners.OrderBy(x =>
            Vector3.Distance(x.transform.position, Vector3.zero)).ToList();

        return sorted[0].transform.position;
    }
}