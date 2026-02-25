using TMPro;
using UnityEngine;

public class ScoreText : UIElement
{
    [SerializeField] private TMP_Text scoreText;

    public override void Initialize()
    {
        gameObject.SetActive(false);
    }

    public override void Show()
    {
        gameObject.SetActive(true);
    }

    public void SetScore(int score)
    {
        scoreText.text = score.ToString();
    }
}
