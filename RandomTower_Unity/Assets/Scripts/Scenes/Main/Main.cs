using UnityEngine;

public class Main : MonoBehaviour
{
    private MainUI _ui => GameManager.Instance.UI.Main;
    private NetworkManager _network => GameManager.Instance.Network;

    void Start()
    {
        GameManager.Instance.UI.Initialize(UIManager.UIType.Main);

        _ui.SinglePlayButton = LoadNextScene;
        _ui.OnMultiConfirm = Main_OnMultiConfirm;
        _ui.OnConnectingCancel = _network.CancelConnect;

        _network.OnSceneLoad = LoadNextScene;
        _network.Disconnect();

        GameManager.Instance.Sound.StopBGM();
    }

    private void Main_OnMultiConfirm(string ip, string port)
    {
        _network.Connect(ip, port);
    }

    private void LoadNextScene()
    {
        if(_network.IsConnect) _ui.DeactiveConnectPanel();
        GameManager.Instance.LoadScene(GameManager.Scenes.Lobby);
    }
}
