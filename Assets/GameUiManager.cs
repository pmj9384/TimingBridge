using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameUIManager : InGameManager
{
    // [보편적 기술] 인스펙터에서 관리할 UI 요소 리스트
    public List<UIElement> uiElements;

    // 자주 쓰는 공통 UI는 명시적으로 두면 편합니다.
    [Header("Common UI")]
    //    [SerializeField] private ResultPanelUI resultPanelUI;
    [SerializeField] private GameObject pausePanel;

    public override void Initialize()
    {
        base.Initialize();

        // [실무 기술] 게임 상태(GameManager)와 UI 연동
        //     GameManager.AddGameStateStartAction(GameManager.GameState.GameOver, ShowGameOverPanel);

        // UI 요소들에게 매니저 참조 세팅 및 초기화
        foreach (var element in uiElements)
        {
            element.SetUIManager(GameManager, this);
            element.Initialize();
        }
    }

    // 인덱스나 열거형으로 UI를 켜는 범용 함수
    public void ShowUIElement(int index)
    {
        if (index >= 0 && index < uiElements.Count)
            uiElements[index].Show();
    }

    // public void ShowGameOverPanel()
    // {
    //     if (resultPanelUI != null)
    //         resultPanelUI.Show();

    //     // [보편적 기술] 게임 결과 데이터 적용 로직 등을 여기에 연결
    //     Debug.Log("게임 오버 UI 표시");
    // }

    public void OnRestartButtonClicked()
    {
        // GameManager를 통해 재시작 로직 실행 (상태 제어)
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    // private void  Clear()
    // {
    //     // 필요한 메모리 정리
    //     uiElements.Clear();
    // }
}