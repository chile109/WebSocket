using UnityEngine;
using System.Collections;
using WebSocketSharp;
using System;

public class client : MonoBehaviour {

    WebSocket _ws;

    void Start () {
	
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
        Send("Online,Theater;");
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
