using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VP.Nest.Economy;
using VP.Nest.Haptic;
using VP.Nest.UI;

namespace _Main.Scripts.UI.Incrementals
{
    public abstract class ButtonHandler : MonoBehaviour
    {
        [SerializeField] private Image buttonImage;
        [SerializeField] private Image overlay;
        [SerializeField] protected TextMeshProUGUI priceText;
        private Button _button;
        private Button B => _button ??= GetComponent<Button>();
        public float currentPrice;
        protected bool IsOpen;
        [SerializeField] private Button disabledButton;

        protected virtual void OnEnable()
        {
            GameEconomy.OnPlayerMoneyUpdate += ButtonState;
        }

        protected virtual void OnDisable()
        {
            GameEconomy.OnPlayerMoneyUpdate -= ButtonState;
        }

        public virtual void Start()
        {
            B.onClick.AddListener(ClickFunc);
            // disabledButton.onClick.AddListener(NotClickAnimation);
        }

        #region ButtonAnimationMethods

        private void ButtonClickAnimation()
        {
            HapticManager.Haptic(HapticType.Selection);
            UIManager.Instance.CurrencyUI.SpendMoney(currentPrice, false);

            buttonImage.transform.DOComplete();
            buttonImage.transform.DOLocalMoveY(-32, .25f).From().SetLink(buttonImage.gameObject);
        }

        private void ButtonNotClickable()
        {
            IsOpen = false;
            buttonImage.transform.DOComplete();
            buttonImage.transform.DOLocalMoveY(-32, .25f).SetLink(buttonImage.gameObject);
        }

        private void ButtonClickable()
        {
            if (IsOpen)
            {
                return;
            }

            IsOpen = true;
            buttonImage.transform.DOComplete();
            buttonImage.transform.DOLocalMoveY(0, .25f).SetLink(buttonImage.gameObject);
        }

        #endregion

        #region ButtonEncapsulationMethods

        protected virtual void ClickFunc()
        {
            if (IsOpen)
            {
                ButtonClickAnimation();
            }
        }

        protected void DisableButton()
        {
            overlay.gameObject.SetActive(true);
            ButtonNotClickable();
        }

        private void NotClickAnimation()
        {
            HapticManager.Haptic(HapticType.Warning);
            transform.DOComplete();
            transform.DOPunchRotation(Vector3.forward * 1.05f, .25f).SetLink(gameObject);
        }

        private void EnableButton()
        {
            overlay.gameObject.SetActive(false);
            ButtonClickable();
        }

        protected enum IncrementalType
        {
            Income,
        }

        private (float, float) ChooseIncremental(IncrementalType incrementalType)
        {
            return incrementalType switch
            {
                IncrementalType.Income => (IncrementalPrices.IncomeBasePrice, IncrementalPrices.IncomeIncreaseAmount),
                _ => throw new ArgumentOutOfRangeException(nameof(incrementalType), incrementalType, null)
            };
        }


        protected void SetPrice(int index, IncrementalType incrementalType)
        {
            var clickIndex = index;
            var (basePrice, increasePrice) = ChooseIncremental(incrementalType);
            currentPrice = basePrice;
            for (int i = 0; i < clickIndex; i++)
            {
                currentPrice += (i + 1) * increasePrice;
            }


            SetPriceText();
            ButtonState();
        }

        private void SetPriceText()
        {
            priceText.text = currentPrice == 0 ? "FREE" : currentPrice.FormatMoney();
        }

        public virtual void ButtonState()
        {
            if (ButtonStateBool())
            {
                EnableButton();
            }
            else
            {
                DisableButton();
            }
        }


        protected virtual bool ButtonStateBool()
        {
            return GameEconomy.HasPlayerEnoughMoney(currentPrice);
        }

        public void DisableManuel()
        {
            B.interactable = false;
        }

        public void EnableManuel()
        {
            B.interactable = true;
        }

        #endregion
    }
}