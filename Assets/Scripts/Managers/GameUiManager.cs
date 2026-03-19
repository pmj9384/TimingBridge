using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameUIManager : InGameManager
{
    public List<UIElement> uiElements;

    public override void Initialize()
    {
        base.Initialize();
        foreach (var element in uiElements)
        {
            element.SetUIManager(GameManager, this);
        }

        GameManager.AddGameStateEnterAction(GameManager.GameState.GameReady, () =>
        {
            ShowUIElement(UIElementEnums.TitlePanel);
        });

        GameManager.AddGameStateEnterAction(GameManager.GameState.GamePlay, () =>
        {
            HideUIElement(UIElementEnums.TitlePanel);
            ShowUIElement(UIElementEnums.ScoreText);
            ShowUIElement(UIElementEnums.PauseButton);
        });

        GameManager.AddGameStateEnterAction(GameManager.GameState.GameStop, () =>
        {
            HideUIElement(UIElementEnums.ScoreText);
            HideUIElement(UIElementEnums.PauseButton);
            ShowUIElement(UIElementEnums.PausePanel);
        });

        GameManager.AddGameStateExitAction(GameManager.GameState.GameStop, () =>
        {
            ShowUIElement(UIElementEnums.ScoreText);
            ShowUIElement(UIElementEnums.PauseButton);
            HideUIElement(UIElementEnums.PausePanel);
        });

        GameManager.AddGameStateEnterAction(GameManager.GameState.GameOver, () =>
        {
            HideUIElement(UIElementEnums.PauseButton);
            StartCoroutine(ShowDelayed(UIElementEnums.GameOverPanel, 1.5f));
        });
    }

    public void InitializedUIElements()
    {
        foreach (var element in uiElements)
        {
            element.Initialize();
        }
    }

    public void ShowUIElement(UIElementEnums type)
    {
        uiElements[(int)type].Show();
    }

    public void HideUIElement(UIElementEnums type)
    {
        uiElements[(int)type].Hide();
    }

    private IEnumerator ShowDelayed(UIElementEnums type, float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowUIElement(type);
    }

}
