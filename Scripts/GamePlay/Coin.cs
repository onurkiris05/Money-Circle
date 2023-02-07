using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int Value;

    private void OnEnable()
    {
        transform.DOMove(new Vector3(2, 4, 0), 1f).SetEase(Ease.InBack).OnComplete(() =>
        {
            //Add money as Value
            gameObject.SetActive(false);
        });
    }
}