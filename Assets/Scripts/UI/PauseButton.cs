using UnityEngine;
using UnityEngine.UI;

public class PauseButton : UIElement
{
    private Button button;

    public override void Initialize()
    {
        gameObject.SetActive(false);
        button = GetComponent<Button>();
        button.onClick.AddListener(OnPauseClicked);
    }

    public override void Show()
    {
        gameObject.SetActive(true);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnPauseClicked()
    {
        gameManager.SetGameState(GameManager.GameState.GameStop);
    }
}
