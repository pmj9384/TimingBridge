using UnityEngine;
using UnityEngine.UI;

public class PausePanel : UIElement
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button settingsButton;

    public override void Initialize()
    {
        gameObject.SetActive(false);
        resumeButton.onClick.AddListener(OnResumeClicked);
        restartButton.onClick.AddListener(OnRestartClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);
    }

    public override void Show()
    {
        gameObject.SetActive(true);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnResumeClicked() => gameManager.SetGameState(GameManager.GameState.GamePlay);
    private void OnRestartClicked() => gameManager.RestartGame();
    private void OnSettingsClicked()
    {
        Hide();
        gameUIManager.ShowUIElement(UIElementEnums.SettingPanel);
    }
}
