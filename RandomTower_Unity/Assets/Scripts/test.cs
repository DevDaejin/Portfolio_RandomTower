using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    public Button btn;
    public TMP_Text txt;

    private Queue<Action> queue = new();

    ClientWebSocket socket = new ClientWebSocket();
    CancellationTokenSource cts = new CancellationTokenSource();

    async void Start()
    {
        await socket.ConnectAsync(new Uri("ws://localhost:8765"), cts.Token);
        Debug.Log("connected");

        _ = Receive();
        btn.onClick.AddListener(Send);
    }

    private void Update()
    {
        if(queue.Count != 0)
        {
            queue.Dequeue().Invoke();
        }
    }

    async void Send()
    {
        int rand = Mathf.RoundToInt(UnityEngine.Random.value * 100);
        string msg = rand.ToString();

        byte[] buffer = Encoding.UTF8.GetBytes(msg);
        await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, cts.Token);
    }

    async Task Receive()
    {
        var buffer = new byte[1024];

        while(socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);
            string received = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Debug.Log($"received {received}");

            queue.Enqueue(() =>
            {
                txt.text = received;
            });
        }
    }
}
