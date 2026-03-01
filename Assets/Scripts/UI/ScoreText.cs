using TMPro;
using UnityEngine;

public class ScoreText : UIElement
{
    [SerializeField] private TMP_Text scoreText;

    public override void Initialize()
    {
        gameObject.SetActive(false);
        BridgeManager.OnScoreChanged += OnScoreChanged;
    }

    private void OnDestroy()
    {
        BridgeManager.OnScoreChanged -= OnScoreChanged;
    }

    public override void Show()
    {
        scoreText.text = "0";
        gameObject.SetActive(true);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnScoreChanged(int score)
    {
        scoreText.text = score.ToString();
    }
}
