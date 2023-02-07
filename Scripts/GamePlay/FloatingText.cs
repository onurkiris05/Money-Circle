using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshPro headerText;

    public TextMeshPro Text => headerText;

    public void SetText(int value)
    {
        headerText.text = $"+{value}";
    }

    public void ReturnToPool()
    {
        FloatingTextPool.Instance.Pool.Release(this);
    }
}
