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
        _ui.Main.OnMultiConfirm += Main_OnMultiConfirm;
        _ui.Main.OnConnectingCancel += GameManager.Instance.Network.CancelConnect;
        GameManager.Instance.Network.OnConnected += LoadNextScene;
        GameManager.Instance.Network.Disconnect();
    }

    private void Main_OnMultiConfirm(string ip, string port)
    {
        GameManager.Instance.Network.Connect(ip, port);
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
