using UnityEngine.EventSystems;

public class PauseButton : UIElement, IPointerDownHandler
{
    public override void Initialize()
    {
        gameObject.SetActive(false);
    }

    public override void Show()
    {
        gameObject.SetActive(true);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySfx(SfxClipId.ButtonTouch);
        gameManager.SetGameState(GameManager.GameState.GameStop);
    }
}
