using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WebSocketSharp;
using System;


public class theater : MonoBehaviour {

    WebSocket _ws;

    public AVProWindowsMediaMovie _movieA;
    public AVProWindowsMediaMovie _movieB;
    public string _folder;
    public List<string> _filenames;

    private AVProWindowsMediaMovie[] _movies;
    private int _moviePlayIndex;
    private int _movieLoadIndex;
    private int _index = -1;
    private bool _loadSuccess = true;
    private int _playItemIndex = -1;
    private bool _playBool = false;

    public AVProWindowsMediaMovie PlayingMovie { get { return _movies[_moviePlayIndex]; } }
    public AVProWindowsMediaMovie LoadingMovie { get { return _movies[_movieLoadIndex]; } }
    public int PlayingItemIndex { get { return _playItemIndex; } }
    public bool IsPaused { get { if (PlayingMovie.MovieInstance != null) return !PlayingMovie.MovieInstance.IsPlaying; return false; } }

    void Start()
    {
        Connect();

        _movieA._loop = true;
        _movieB._loop = true;
        _movies = new AVProWindowsMediaMovie[2];
        _movies[0] = _movieA;
        _movies[1] = _movieB;
        _moviePlayIndex = 0;
        _movieLoadIndex = 1;

        NextMovie();
    }

    void Update()
    {
       
        if (PlayingMovie.MovieInstance != null)
        {
            if (_playBool)
            {
                _movieA._loop = false;
                _movieB._loop = false;
                NextMovie();
                _playBool = false;
                
            }

            if ((int)PlayingMovie.MovieInstance.PositionFrames >= (PlayingMovie.MovieInstance.DurationFrames - 1))
            {
                Send("PlayCompleted,Client1;");
                _index = -1;
                print("idle");
                NextMovie();
                _movieA._loop = true;
                _movieB._loop = true;
            }
        }

        if (!_loadSuccess)
        {
            _loadSuccess = true;
            NextMovie();
        }
    }
    void OnGUI()
    {
        AVProWindowsMediaMovie activeMovie = PlayingMovie;
        if (activeMovie.OutputTexture == null)
            activeMovie = LoadingMovie; // Display the previous video until the current one has loaded the first frame

        Texture texture = activeMovie.OutputTexture;

        if (texture != null)
        {
            Rect rect = new Rect(0, 0, Screen.width, Screen.height);

            if (activeMovie.MovieInstance.RequiresFlipY)
            {
                GUIUtility.ScaleAroundPivot(new Vector2(1f, -1f), new Vector2(0, rect.y + (rect.height / 2)));
            }

            GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit, false);
        }
    }
    public void Pause()
    {
        if (PlayingMovie != null)
        {
            PlayingMovie.Pause();
        }
    }

    private void NextMovie()
    {
        Pause();

        if (_filenames.Count > 0)
        {
            _index = (Mathf.Max(0, _index + 1)) % _filenames.Count;
        }
        else
            _index = -1;

        if (_index < 0)
            return;


        LoadingMovie._folder = _folder;
        LoadingMovie._filename = _filenames[_index];
        LoadingMovie._useStreamingAssetsPath = true;
        LoadingMovie._playOnStart = true;
        _loadSuccess = LoadingMovie.LoadMovie(true);
        _playItemIndex = _index;

        _moviePlayIndex = (_moviePlayIndex + 1) % 2;
        _movieLoadIndex = (_movieLoadIndex + 1) % 2;
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

        switch (e.Data)
        {
            case "Play,1;":
                _index = 0;
                print(_index);
                _playBool = true;
                break;
            case "Play,2;":
                _index = 1;
                print(_index);
                _playBool = true;
                break;
            case "Cancel;":
                _index = -1;
                print(_index);
                _playBool = true;
                break;
        }
            

        /*if (e.Data == "Play,1;")
        {
            try
            {
                _index = 0;
                print(_index);
                _playBool = true;
            }
            catch (Exception ex)
            {
                Debug.Log("_playItemIndex:" + ex.Message);
            }
            
            
        }

        if (e.Data == "Play,2;")
        {
            try
            {
                _index = 1;
                print(_index);
                _playBool = true;
            }
            catch (Exception ex)
            {
                Debug.Log("_playItemIndex:" + ex.Message);
            }


        }*/

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
