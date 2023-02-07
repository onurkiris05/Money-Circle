using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VP.Nest.Haptic;

    public class SettingsUI : UIPanel
    {
        [Header("SETTINGS UI SETTINGS")]
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button closeButton;

        [SerializeField] private SwitchButton vibrationSwitch;
        [SerializeField] private SwitchButton soundSwitch;
        
        public UnityAction OnSettingsLayoutDisplay;
        public UnityAction OnSettingsLayoutConceal;

        #region ImplementationMethods

        internal override void Initialize()
        {
            base.Initialize();
            
            settingsButton.onClick.AddListener(HandleSettingsButtonClick);
            closeButton.onClick.AddListener(HandleCloseButtonClick);
            
            vibrationSwitch.AddListener(HandleVibrationSliderClick);
            soundSwitch.AddListener(HandleSoundSliderClick);
            
            vibrationSwitch.Switch(HapticManager.IsHapticActive);
            //soundSwitch.Switch(AudioManager.IsFxActive);
        }

        #endregion

        #region SettingsUIMethods

        private void HandleSettingsButtonClick()
        {
            UIManager.FocusPanel(typeof(SettingsUI));
            OnSettingsLayoutDisplay?.Invoke();
        }

        private void HandleCloseButtonClick()
        {
            UIManager.UnfocusPanel();
            OnSettingsLayoutConceal?.Invoke();
        }

        private void HandleVibrationSliderClick(bool isOn)
        {
            HapticManager.IsHapticActive = isOn;
        }

        private void HandleSoundSliderClick(bool isOn)
        {
            //AudioManager.IsFxActive = isOn;
        }

        #endregion
    }
