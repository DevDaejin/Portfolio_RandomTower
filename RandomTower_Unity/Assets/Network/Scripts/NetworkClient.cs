
using Google.Protobuf;
using NativeWebSocket;
using Net;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class NetworkClient
{
    private WebSocket _socket;
    private readonly string _url;
    private readonly Dictionary<string, Action<byte[]>> _envelopeHandlers = new();

    public string ClientID { get; set; }
    public string RoomID { get; set; }

    public Action OnConnected;
    public Action OnConnectFailed;
    public Action OnError;
    public Action OnClose;

    public NetworkClient(string address)
    {
        _url = $"ws://{address}";
    }

    public async Task Connect()
    {
        _socket = new WebSocket(_url);

        _socket.OnOpen += () =>
        {
            Debug.Log("[WebSocket] Connected");
            OnConnected?.Invoke();
        };

        _socket.OnError += error =>
        {
            Debug.Log($"[WebSocket] Error - {error}");
            OnError?.Invoke();
        };

        _socket.OnClose += code =>
        {
            Debug.Log($"[WebSocket] close - {code.ToString()}");

            if (code != WebSocketCloseCode.Normal)
            {
                OnClose?.Invoke();
            }
        };


        _socket.OnMessage += OnMessageReceived;

        try
        {
            await _socket.Connect();
        }
        catch (Exception)
        {
            OnConnectFailed?.Invoke();
        }
    }

    public void RegisterEnvelopeHandler(string type, Action<byte[]> handler)
    {
        _envelopeHandlers[type] = handler;
    }

    private void OnMessageReceived(byte[] bytes)
    {
        var env = Envelope.Parser.ParseFrom(bytes);
        if (_envelopeHandlers.TryGetValue(env.Type, out var handler))
        {
            handler.Invoke(env.Payload.ToByteArray());
        }
    }

    public async Task SendEnvelope(string type, IMessage payload)
    {
        var envelope = new Envelope
        {
            Type = type,
            Payload = ByteString.CopyFrom(payload.ToByteArray())
        };

        byte[] bytes = envelope.ToByteArray();
        await _socket?.Send(bytes);
    }

    public async Task Send<T>(string type, T packet) where T : IMessage
    {
        var envelope = new Envelope
        {
            Type = type,
            Payload = packet.ToByteString()
        };
        await _socket.Send(envelope.ToByteArray());
    }

    public void CancelConnect() => _socket?.CancelConnection();
    public void DispatchMessages() => _socket?.DispatchMessageQueue();
    public void Disconnect() => _socket?.Close();
}