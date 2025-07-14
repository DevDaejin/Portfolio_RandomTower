public class InGameUIHandler
{

    public InGameUI UI => GameManager.Instance.UI.InGame;

    public InGameUIHandler()
    {
        GameManager.Instance.UI.Initialize(UIManager.UIType.InGame);
    }
}
