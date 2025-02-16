using System;
using System.Collections;
using System.Collections.Generic;
using SocketIOClient;
using UnityEngine;

public class SSApi : MonoBehaviour
{
    // Start is called before the first frame update
    public string DEVELOPMENT_API_URL = "http://127.0.0.1:3001";
    public string PRODUCTION_API_URL = "https://ssapi.yuzhes.com";
    SocketIOUnity socket;
    IEnumerator Start()
    {
#if UNITY_EDITOR
        var uri = new Uri(DEVELOPMENT_API_URL);
#else
        var uri = new Uri(PRODUCTION_API_URL);
#endif

        socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Query = new Dictionary<string, string>
                {
                    {"token", "UNITY" }
                }
            ,
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });

        socket.OnConnected += (sender, e) =>
        {
            Debug.Log("Connected");

            socket.Emit("message", "Hello from Unity");
        };

        socket.OnError += (sender, e) =>
        {
            Debug.Log("Error: " + e);
        };

        socket.OnAny((eventName, data) =>
        {
            Debug.Log("Event: " + eventName + " Data: " + data);
        });

        socket.OnReconnectError += (sender, e) =>
        {
            Debug.Log("Reconnect Error: " + e);
        };

        socket.OnReconnectFailed += (sender, e) =>
        {
            Debug.Log("Reconnect Failed: " + e);
        };

        yield return socket.ConnectAsync();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
