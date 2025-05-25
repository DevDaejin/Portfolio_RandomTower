using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    //[SerializeField] private string _roomID;
    private string _roomName = "room id";
    private string _address = "127.0.0.1";
    private string _port = "8765";

    private NetworkClient _client;

    public async void Connect(string ip, string port)
    {
        _address = ip;
        _port = port;
        _client = new($"{_address}:{_port}");
        await _client.Connect();
    }

    private void Update()
    {
        _client?.DispatchMessages();

        if (Input.GetKeyDown(KeyCode.F1))
        {
            _client.CreateRoom(_roomName);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            //_client.JoinRoom(_roomID);
        }
        if (Input.GetKeyDown(KeyCode.F3))
        { 
            _client.RequestRoomList();
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            _client.LeaveRoom();
        }
    }


    private async void OnApplicationQuit()
    {
        await _client.Disconnect();
    }
}