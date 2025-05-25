using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public enum UIType {None, Main, InGame, Lobby}

    public MainUI Main => _mainUI;
    [SerializeField] private MainUI _mainUI;

    public InGameUI InGame => _inGameUI;
    [SerializeField] private InGameUI _inGameUI;

    public LobbyUI Lobby => _lobbyUI;
    [SerializeField] private LobbyUI _lobbyUI;

    private Dictionary<UIType, GameObject> _uis = new();
    
    private void Start()
    {
        _uis.Add(UIType.Main, Main.gameObject);
        _uis.Add(UIType.Lobby, Lobby.gameObject);
        _uis.Add(UIType.InGame, InGame.gameObject);

        Initialize(UIType.None);
    }

    public void Initialize(UIType type)
    {
        foreach( KeyValuePair<UIType, GameObject> pair in _uis )
        {
            pair.Value.SetActive(pair.Key == type);
        }
    }
}
