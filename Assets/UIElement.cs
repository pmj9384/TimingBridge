using UnityEngine;

public class UIElement : MonoBehaviour
{
    [SerializeField] private UIElementEnums elementType;
    public UIElementEnums ElementType => elementType;

    protected GameUIManager gameUIManager;
    protected GameManager gameManager;

    public void SetUIManager(GameManager gameManager, GameUIManager uIManager)
    {
        gameUIManager = uIManager;
        this.gameManager = gameManager;
    }

    public virtual void Initialize() { }

    public virtual void Show() { }
}