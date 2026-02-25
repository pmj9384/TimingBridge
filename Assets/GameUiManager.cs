using System.Collections.Generic;
using UnityEngine;

public class GameUIManager : InGameManager
{
    [SerializeField] private List<UIElement> uiElements;

    private Dictionary<UIElementEnums, UIElement> _elementMap;

    public override void Initialize()
    {
        base.Initialize();
        foreach (var element in uiElements)
            element.SetUIManager(GameManager, this);
    }

    public void InitializedUIElements()
    {
        _elementMap = new Dictionary<UIElementEnums, UIElement>();
        foreach (var element in uiElements)
        {
            _elementMap[element.ElementType] = element;
            element.Initialize();
        }
    }

    public void ShowUIElement(UIElementEnums type)
    {
        if (_elementMap.TryGetValue(type, out var element))
            element.Show();
        else
            Debug.LogWarning($"[GameUIManager] {type} 요소를 찾을 수 없습니다.");
    }

    public T GetUIElement<T>(UIElementEnums type) where T : UIElement
    {
        if (_elementMap.TryGetValue(type, out var element))
            return element as T;
        return null;
    }

    public void OnRestartButtonClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
