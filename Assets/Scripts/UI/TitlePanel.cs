using UnityEngine;
using UnityEngine.UI;

public class TitlePanel : UIElement
{
    [SerializeField] private Button startButton;

    public override void Initialize()
    {
        gameObject.SetActive(false);
        startButton.onClick.AddListener(OnStartClicked);
    }

    public override void Show()
    {
        gameObject.SetActive(true);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnStartClicked()
    {
        gameManager.SetGameState(GameManager.GameState.GamePlay);
    }
}
