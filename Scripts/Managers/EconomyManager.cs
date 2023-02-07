using System;
using System.Collections;
using System.Collections.Generic;
using ElephantSDK;
using UnityEngine;

public class EconomyManager : Singleton<EconomyManager>
{
    [Header("Income Settings")]
    [SerializeField] private float incomeRate = 1;
    [SerializeField] private float incomeRateIncrement = 0.1f;

    public float IncomeRate => incomeRate;

    private void Start()
    {
        incomeRateIncrement = RemoteConfigManager.IncomeIncrementPerLevel;

        if (PlayerPrefs.GetInt("NewGame") > 0)
        {
            incomeRate = PlayerPrefs.GetFloat("IncomeRate");
        }
        else
        {
            PlayerPrefs.SetFloat("IncomeRate", incomeRate);
        }

        IncrementalManager.Instance.GetUpgradeCard(UpgradeType.Income).OnCurrencyPurchase += IncreaseIncomeRate;
    }

    private void IncreaseIncomeRate()
    {
        incomeRate += incomeRateIncrement;

        PlayerPrefs.SetFloat("IncomeRate", incomeRate);
    }
}