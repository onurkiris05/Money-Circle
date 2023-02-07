using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class FloatingTextPool : Singleton<FloatingTextPool>
{
    [SerializeField] FloatingText floatingTextPrefab;

    public IObjectPool<FloatingText> Pool => floatingTextPool;


    IObjectPool<FloatingText> floatingTextPool;

    void Awake()
    {
        floatingTextPool = new ObjectPool<FloatingText>
        (
            CreateFloatingText,
            ActionOnGet,
            ActionOnRelease
        );
    }

    FloatingText CreateFloatingText()
    {
        FloatingText floatingText = Instantiate(floatingTextPrefab, transform);
        return floatingText;
    }

    void ActionOnGet(FloatingText floatingText)
    {
        floatingText.gameObject.SetActive(true);
    }

    void ActionOnRelease(FloatingText floatingText)
    {
        floatingText.gameObject.SetActive(false);
        floatingText.transform.position = transform.position;
    }
}