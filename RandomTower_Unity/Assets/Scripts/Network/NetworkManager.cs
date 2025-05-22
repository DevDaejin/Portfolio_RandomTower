using UnityEngine;
using NativeWebSocket;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    private WebSocket _socket;
    private SyncController _sync;
    public SyncController SyncController => _sync;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private async void Start()
    {
        _socket = new WebSocket("ws://localhost:8765");
        await _socket.Connect();
        _sync = new SyncController(_socket);
    }

    private void Update()
    {
        SyncController?.Update();
    }

    private async void OnApplicationQuit()
    {
        await _socket.Close();
    }
}
