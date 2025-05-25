using UnityEngine;

public class Lobby : MonoBehaviour
{
    private LobbyUI _ui;

    void Awake()
    {
        GameManager.Instance.UI.Initialize(UIManager.UIType.Lobby);
        _ui = GameManager.Instance.UI.Lobby;
    }
}
 