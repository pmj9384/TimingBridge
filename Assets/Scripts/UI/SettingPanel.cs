using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : UIElement
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button closeButton;

    public static event Action<float, float> onBgmVolumeChanged;

    public override void Initialize()
    {
        gameObject.SetActive(false);
        bgmSlider.onValueChanged.AddListener(OnBgmSliderChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);
        restartButton.onClick.AddListener(OnRestartClicked);
        closeButton.onClick.AddListener(OnCloseClicked);
    }

    public override void Show()
    {
        bgmSlider.SetValueWithoutNotify(SoundManager.Instance.bgmVolume);
        sfxSlider.SetValueWithoutNotify(SoundManager.Instance.sfxVolume);
        gameObject.SetActive(true);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnBgmSliderChanged(float value)
    {
        onBgmVolumeChanged?.Invoke(value, SoundManager.Instance.sfxVolume);
    }

    private void OnSfxSliderChanged(float value)
    {
        onBgmVolumeChanged?.Invoke(SoundManager.Instance.bgmVolume, value);
    }

    private void OnRestartClicked() => gameManager.RestartGame();

    private void OnCloseClicked()
    {
        Hide();
        gameUIManager.ShowUIElement(UIElementEnums.PausePanel);
    }
}
