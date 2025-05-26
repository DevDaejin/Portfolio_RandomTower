using UnityEngine;

public class Main : MonoBehaviour
{
    private UIManager _ui;
    void Start()
    {
        _ui = GameManager.Instance.UI;
        _ui.Initialize(UIManager.UIType.Main);

        _ui.Main.SinglePlayButton = () => GameManager.Instance.LoadScene(GameManager.Scenes.Lobby);
        _ui.Main.QuitButton = () => Application.Quit();
        _ui.Main.OnMultiConfirm += GameManager.Instance.Network.Connect;
        _ui.Main.OnConnectingCancel += GameManager.Instance.Network.CancelConnect;

        GameManager.Instance.Network.OnConnected += LoadNextScene;

        GameManager.Instance.Network.Disconnect();
    }

    private void LoadNextScene()
    {
        GameManager.Instance.LoadScene(GameManager.Scenes.Lobby);
    }

    private void OnDestroy()
    {
        GameManager.Instance.Network.OnConnected -= LoadNextScene;
    }
}
