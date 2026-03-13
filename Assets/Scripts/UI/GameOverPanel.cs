using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : UIElement
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button homeButton;

    public override void Initialize()
    {
        gameObject.SetActive(false);
        restartButton.onClick.AddListener(OnRestartClicked);
        homeButton.onClick.AddListener(OnHomeClicked);
    }

    public override void Show()
    {
        gameObject.SetActive(true);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnRestartClicked() => gameManager.RestartGame();

    private void OnHomeClicked() => gameManager.GoToTitle();
}
