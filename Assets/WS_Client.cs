using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class WS_Client : MonoBehaviour
{
    public GameObject lightsaber;
    private float x = 0, y = 0, z = 0;
    WebSocket ws;
    private void Start()
    {
        ws = new WebSocket("ws://localhost:8080");
        ws.Connect();
        ws.OnMessage += (sender, e) =>
        {
            Debug.Log(e.Data);
            y = -float.Parse(e.Data.Split(' ')[0]);
            x = 90-float.Parse(e.Data.Split(' ')[1]);
            z = -float.Parse(e.Data.Split(' ')[2]);
        };
    }
    void Update()
    {
        // lightsaber.transform.Rotate(x, y, z, Space.World);
        lightsaber.transform.rotation = Quaternion.Euler(x, y, z);
    }
}