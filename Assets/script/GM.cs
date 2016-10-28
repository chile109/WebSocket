using UnityEngine;
using System.Collections;

using WebSocketSharp;
using System;

public class GM : MonoBehaviour {
    
    WebSocket _ws;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    // Show buttons: Connect, Send...etc
    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 70, 150, 50), "Connect"))
        {
            Debug.Log("Connecting...");
            Connect();
        }

        if (GUI.Button(new Rect(10, 270, 150, 50), "Query"))
        {
            Send("Query,ClientList;");
        }
        if (GUI.Button(new Rect(10, 170, 150, 50), "Greeting"))
        {
            Send("Good moring;");
        }

        if (GUI.Button(new Rect(250, 70, 150, 50), "theater1"))
        {
            Send("Play,1;");
        }

        if (GUI.Button(new Rect(250, 270, 150, 50), "theater2"))
        {
            Send("Play,2;");
        }
        if (GUI.Button(new Rect(250, 170, 150, 50), "Cancel"))
        {
            Send("Cancel;");
        }

    }

    void Connect()
    {
        try
        {
            _ws = new WebSocket("ws://192.168.2.96:5000"); // right one
            //_ws = new WebSocket("ws://192.168.2.66:5000");
            //_ws = new WebSocket("ws://localhost:5001"); // test wrong port
            //_ws = new WebSocket("ws://192.168.0.99:5000"); // a random pick wrong addr
            //Debug.Log("Connecting " + _ws.Url);

            _ws.OnOpen += _ws_OnOpen;
            _ws.OnMessage += _ws_OnMessage;
            _ws.OnError += _ws_OnError;
            _ws.OnClose += _ws_OnClose;


            _ws.ConnectAsync();
        }
        catch (Exception ex)
        {
            Debug.Log("Connect, ex = " + ex.Message);
        }
    }

    private void _ws_OnOpen(object sender, EventArgs e)
    {
        Debug.Log("OnOpen");
        Send("Online,Console;");
    }

    private void _ws_OnMessage(object sender, MessageEventArgs e)
    {
        Debug.Log("Recv: " + e.Data);
    }

    private void _ws_OnClose(object sender, CloseEventArgs e)
    {
        Debug.Log("OnClose, " + e.Reason);
    }

    private void _ws_OnError(object sender, ErrorEventArgs e)
    {
        Debug.Log("OnError, " + e.Message);
    }

    void Send(string msg)
    {
        Debug.Log("Send: " + msg);

        try
        {
            _ws.SendAsync(msg, OnSendCompleted);
        }
        catch (Exception ex)
        {
            Debug.Log("Send, ex = " + ex.Message);
        }
    }

    private void OnSendCompleted(bool result)
    {
        Debug.Log("OnSendCompleted: " + result);
    }
}
