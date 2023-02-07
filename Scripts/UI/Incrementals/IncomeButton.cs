using System;
using UnityEngine;

namespace _Main.Scripts.UI.Incrementals
{
    public class IncomeButton : ButtonHandler
    {
        public static Action OnButtonClick;

        private int IncomeClickCount
        {
            get => PlayerPrefs.GetInt("IncomeClickCount", 0);
            set => PlayerPrefs.SetInt("IncomeClickCount", value);
        }

        public override void Start()
        {
            base.Start();
            SetPrice(IncomeClickCount, IncrementalType.Income);
        }

        protected override void ClickFunc()
        {
            base.ClickFunc();
            if (!IsOpen) return;
            OnButtonClick?.Invoke();
            IncomeClickCount++;
            SetPrice(IncomeClickCount, IncrementalType.Income);
        }
    }
}