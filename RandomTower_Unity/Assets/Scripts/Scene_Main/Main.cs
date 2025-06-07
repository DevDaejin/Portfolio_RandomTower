using UnityEngine;

public class Main : MonoBehaviour
{
    private MainUI _ui;
    void Start()
    {
        GameManager.Instance.UI.Initialize(UIManager.UIType.Main);
        _ui = GameManager.Instance.UI.Main;

        _ui.SinglePlayButton = () => GameManager.Instance.LoadScene(GameManager.Scenes.Lobby);
        _ui.QuitButton = () => Application.Quit();
        _ui.OnMultiConfirm += Main_OnMultiConfirm;
        _ui.OnConnectingCancel += GameManager.Instance.Network.CancelConnect;
        GameManager.Instance.Network.OnSceneLoad = LoadNextScene;
        GameManager.Instance.Network.Disconnect();
    }

    private void Main_OnMultiConfirm(string ip, string port)
    {
        GameManager.Instance.Network.Connect(ip, port);
    }

    private void LoadNextScene()
    {
        _ui.DeactiveConnectPanel();
        GameManager.Instance.LoadScene(GameManager.Scenes.Lobby);
    }

    private void OnDestroy()
    {
        GameManager.Instance.Network.OnSceneLoad -= LoadNextScene;
        _ui.OnConnectingCancel -= GameManager.Instance.Network.CancelConnect;
        _ui.OnMultiConfirm -= Main_OnMultiConfirm;
    }
}
