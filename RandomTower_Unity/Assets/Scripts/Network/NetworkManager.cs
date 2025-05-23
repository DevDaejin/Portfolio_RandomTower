using UnityEngine;
using NativeWebSocket;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    private WebSocket _socket;
    //private IDGenerator _id;
    //private SyncController _sync;
    //public SyncController SyncController => _sync;

    public SyncRegistry SyncRegistry { get; private set; }

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

        SyncRegistry = new SyncRegistry();
    }

    private async void Start()
    {
        _socket = new WebSocket("ws://localhost:8765");
        await _socket.Connect();
        //_sync = new SyncController(_socket, _id);
    }

    public async void Send(byte[] data)
    {
        if (_socket == null || _socket.State != WebSocketState.Open) return;
        
        await _socket.Send(data);
    }

    public void OnReceive(byte[] data)
    {
        SyncRegistry.Receive(data);
    }

    private async void OnApplicationQuit()
    {
        await _socket.Close();
    }
}
