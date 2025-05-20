using UnityEngine;

public class Lobby : MonoBehaviour
{
    private LobbyUI _ui;

    void Awake()
    {
        GameManager.Instance.UIManager.Initialize(UIManager.UIType.Lobby);
        _ui = GameManager.Instance.UIManager.Lobby;
    }
}
