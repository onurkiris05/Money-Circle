using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ElephantSDK;
using UnityEditor;
using UnityEngine;
using VP.Nest.Economy;
using VP.Nest.Haptic;
using VP.Nest.UI.Currency;
using VP.Nest.UI.InGame;

public class SpawnerManager : Singleton<SpawnerManager>
{
    [Header("General Components")]
    [SerializeField] private CurrencyUI currencyUI;
    [SerializeField] private InGameUI inGameUI;

    [Space] [Header("Spawner Settings")]
    [SerializeField] private float spawnDelay, spawnDelayMin, tapSpawnDelayMin;
    [SerializeField] private float spawnDelayDecrement = 0.05f;
    [SerializeField] private float tapSpawnDelay = 0.4f;
    [SerializeField] private int tapCountToMax = 5;
    [SerializeField] private AnimationCurve tapStrengthCurve;

    [Space] [Header("Spawner Placement Settings")]
    [SerializeField] private Spawner spawnerPrefab;
    [SerializeField] private Transform minBound, maxBound;
    [SerializeField] private int column, row;

    public List<Spawner> Spawners = new List<Spawner>();

    private Coroutine speedUp;
    private int speedUpLifetime = 5;
    private int tapIndex;
    private float incomeDelay;
    private float rawTapStrength;
    private float tapStrengthPerTap;
    private float firstSpawnDelay;
    private float spawnDelayDiff = 1;
    private float curveRatio;
    private bool isSpawning;

    private void Start()
    {
        spawnDelay = RemoteConfigManager.SpawnDelay;
        spawnDelayMin = RemoteConfigManager.SpawnDelayMin;
        spawnDelayDecrement = RemoteConfigManager.SpawnDelayDecrement;
        tapCountToMax = RemoteConfigManager.TapCountMax;

        IncrementalManager.Instance.GetUpgradeCard(UpgradeType.Speed).OnCurrencyPurchase += IncreaseSpeed;

        if (PlayerPrefs.GetInt("NewGame") > 0)
        {
            spawnDelay = PlayerPrefs.GetFloat("SpawnDelay");
        }
        else
        {
            PlayerPrefs.SetFloat("SpawnDelay", spawnDelay);
        }

        incomeDelay = spawnDelay;

        StartCoroutine(ProcessTriggerAllSpawners());
    }

    public bool IsSpawnDelayMax()
    {
        return spawnDelay - 0.01f <= spawnDelayMin;
    }

    public void PlaySpeedUpEffects()
    {
        for (int i = 0; i < Spawners.Count; i++)
        {
            Spawners[i].PlaySpeedUpEffect();
        }
    }

    // public void SpeedUp()
    // {
    //     if (speedUp != null)
    //     {
    //         HapticManager.Haptic(HapticType.SoftImpact);
    //
    //         StopCoroutine(speedUp);
    //         speedUp = StartCoroutine(ProcessCooldown());
    //
    //         if (tapIndex >= tapCountToMax)
    //         {
    //             curveRatio = 1;
    //         }
    //         else
    //         {
    //             tapIndex++;
    //             rawTapStrength += tapStrengthPerTap;
    //             curveRatio = tapStrengthCurve.Evaluate(rawTapStrength / spawnDelayDiff);
    //         }
    //
    //         tapSpawnDelay = firstSpawnDelay - (spawnDelayDiff * curveRatio);
    //         tapSpawnDelay = Mathf.Clamp(tapSpawnDelay, tapSpawnDelayMin, Mathf.Infinity);
    //         spawnDelay = tapSpawnDelay;
    //     }
    //     else if (speedUp == null)
    //     {
    //         HapticManager.Haptic(HapticType.SoftImpact);
    //
    //         tapSpawnDelay = spawnDelay;
    //         tapStrengthPerTap = spawnDelayDiff / tapCountToMax;
    //
    //         rawTapStrength += tapStrengthPerTap;
    //         curveRatio = tapStrengthCurve.Evaluate(rawTapStrength / spawnDelayDiff);
    //
    //         firstSpawnDelay = spawnDelay;
    //         tapSpawnDelay = firstSpawnDelay - (spawnDelayDiff * curveRatio);
    //         tapSpawnDelay = Mathf.Clamp(tapSpawnDelay, tapSpawnDelayMin, Mathf.Infinity);
    //         spawnDelay = tapSpawnDelay;
    //
    //         speedUp = StartCoroutine(ProcessCooldown());
    //     }
    // }

    // IEnumerator ProcessCooldown()
    // {
    //     yield return new WaitForSeconds(speedUpLifetime);
    //
    //     rawTapStrength = 0;
    //     tapIndex = 0;
    //     spawnDelay = firstSpawnDelay;
    //     speedUp = null;
    // }

    public void IncreaseWallet(int value)
    {
        currencyUI.AddMoney(value, false);
    }

    public void TriggerAllSpawners(bool isLoop = false)
    {
        StartCoroutine(ProcessAllSpawners(isLoop));
    }

    IEnumerator ProcessAllSpawners(bool isLoop = false)
    {
        if (!isSpawning)
        {
            isSpawning = true;
            int firstMoney = (int)GameEconomy.PlayerMoney;

            for (int i = 0; i < Spawners.Count; i++)
            {
                Spawners[i].SpawnMoney();
            }

            int secondMoney = (int)GameEconomy.PlayerMoney;


            if (isLoop)
            {
                if (incomeDelay < spawnDelay)
                {
                    incomeDelay += spawnDelay / 5;
                    incomeDelay = Mathf.Clamp(incomeDelay, tapSpawnDelay, spawnDelay);
                }
            }
            else
            {
                HapticManager.Haptic(HapticType.SoftImpact);
                incomeDelay -= spawnDelay / 5;
                incomeDelay = Mathf.Clamp(incomeDelay, tapSpawnDelay, spawnDelay);
            }

            Debug.Log("From tap: " + incomeDelay);

            int income = secondMoney - firstMoney;
            int incomePerSecond = (int)(income / incomeDelay);
            inGameUI.UpdateIncomePerSecond(incomePerSecond);

            yield return new WaitForSeconds(tapSpawnDelay);
            isSpawning = false;
        }
    }

    IEnumerator ProcessTriggerAllSpawners()
    {
        while (true)
        {
            TriggerAllSpawners(true);

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void IncreaseSpeed()
    {
        spawnDelay -= spawnDelayDecrement;
        spawnDelay = Mathf.Clamp(spawnDelay, spawnDelayMin, Mathf.Infinity);
        PlayerPrefs.SetFloat("SpawnDelay", spawnDelay);

        Debug.Log("From button: " + incomeDelay);

        // if (speedUp != null)
        // {
        //     firstSpawnDelay -= spawnDelayDecrement;
        //     tapSpawnDelay = firstSpawnDelay - (spawnDelayDiff * curveRatio);
        //     tapSpawnDelay = Mathf.Clamp(tapSpawnDelay, tapSpawnDelayMin, Mathf.Infinity);
        //     spawnDelay = tapSpawnDelay;
        //     PlayerPrefs.SetFloat("SpawnDelay", firstSpawnDelay);
        // }
        // else
        // {
        //     spawnDelay -= spawnDelayDecrement;
        //     spawnDelay = Mathf.Clamp(spawnDelay, spawnDelayMin, Mathf.Infinity);
        //     PlayerPrefs.SetFloat("SpawnDelay", spawnDelay);
        // }
    }

    #region Editor Methods

#if UNITY_EDITOR
    [ContextMenu("Place Spawners")]
    private void SetBoard()
    {
        float xOffset = (maxBound.position.x - minBound.position.x) / (column - 1);
        float yOffset = (maxBound.position.y - minBound.position.y) / (row - 1);

        Vector3 spawnPos = minBound.position;

        Spawners.Clear();

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                Spawner spawner = (Spawner)PrefabUtility.InstantiatePrefab(spawnerPrefab, transform);
                spawner.transform.position = spawnPos;
                Spawners.Add(spawner);
                spawnPos = new Vector3(spawnPos.x + xOffset, spawnPos.y, spawnPos.z);
            }

            spawnPos = new Vector3(minBound.position.x, spawnPos.y + yOffset, spawnPos.z);
        }
    }

    [ContextMenu("Clear Spawners")]
    private void ClearBoard()
    {
        if (Spawners.Count > 0)
        {
            foreach (Spawner spawner in Spawners)
            {
                DestroyImmediate(spawner.gameObject);
            }
        }
        else
        {
            Spawners = FindObjectsOfType<Spawner>().ToList();

            if (Spawners.Count > 0)
            {
                foreach (Spawner spawner in Spawners)
                {
                    DestroyImmediate(spawner.gameObject);
                }
            }
        }

        Spawners.Clear();
    }
#endif

    #endregion
}