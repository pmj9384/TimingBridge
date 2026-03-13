using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : UIElement
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text bestScoreText;
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
        int score = gameManager.BridgeManager.Score;
        int best = GameDataManager.Instance.PlayerAccountData.BestScore;

        scoreText.text = score.ToString();
        bestScoreText.text = best.ToString();

        gameObject.SetActive(true);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnRestartClicked() => gameManager.RestartGame();

    private void OnHomeClicked() => gameManager.GoToTitle();
}
