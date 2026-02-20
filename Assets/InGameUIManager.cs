using UnityEngine;
using TMPro; // 텍스트 매시 프로 사용
using UnityEngine.UI;
using System.Collections;

public class InGameUIManager : InGameManager
{
    // 어디서든 접근 가능하게 싱글톤 처리
    public static InGameUIManager Instance;

    [Header("UI Panels")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("Text Elements")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text finalScoreText;

    private int currentScore = 0;

    private void Awake()
    {
        Instance = this;
        // 시작할 때 게임오버 창은 꺼두기
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    public override void Initialize()
    {
        base.Initialize();
        GameManager.Instance.AddGameStateEnterAction(GameManager.GameState.GameOver, () =>
        {
            StartCoroutine(ShowGameOverDelayed());
        });
    }

    // 점수 업데이트 (다리 건너기 성공 시 호출)
    public void UpdateScore(int amount)
    {
        currentScore += amount;
        scoreText.text = currentScore.ToString();
    }

    private IEnumerator ShowGameOverDelayed()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        ShowGameOver();
    }

    // 게임 오버 창 띄우기
    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        finalScoreText.text = "Final Score: " + currentScore;
    }

    // 재시작 버튼용 (버튼에 연결)
    public void OnRetryButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0); // 현재 씬 재시작
    }
}