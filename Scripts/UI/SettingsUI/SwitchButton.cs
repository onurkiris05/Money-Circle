using System;
using UnityEngine;
using UnityEngine.UI;

public class SwitchButton : MonoBehaviour
{
    [SerializeField] private Button toggleButton;
    [SerializeField] private GameObject greenOverlay;
    [SerializeField] private RectTransform toggleCircle;
    [SerializeField] private RectTransform circleOffPos;
    [SerializeField] private RectTransform circleOnPos;

    private Action<bool> _onToggleButtonClickAction;
    private bool _isOn = true;

    public void AddListener(Action<bool> func)
    {
        _onToggleButtonClickAction = func;
        toggleButton.onClick.AddListener(HandleToggleSliderClick);
    }

    private void HandleToggleSliderClick()
    {
        _isOn = !_isOn;
        Switch(_isOn);
        _onToggleButtonClickAction?.Invoke(_isOn);
    }

    public void Switch(bool value)
    {
        toggleCircle.position = value ? circleOnPos.position : circleOffPos.position;
        greenOverlay.SetActive(value);
    }
}