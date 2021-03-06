using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class WS_Client : MonoBehaviour
{
    public GameObject lightsaber;
    private float x = 0, y = 0, z = 0;
    private float raw_x = 0, raw_y = 0;
    private float raw_z = 0;
    private float pos_x = 0, pos_y = 0;
    public WebSocket ws;
    public int portNum;
    public float origin_x;
    public int player;
    static public string UID1;
    static public string UID2;

    // calculate max angular velocity over past 10 frames
    float[] past_av = new float[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    int past_av_ind = 0;
    Vector2 deltaRotation;
    Vector2 last_axy = new Vector2(0, 0);
    float av_last_update_time;
    static public float blade_av1; // take max over past 10 frame
    static public float blade_av2;

    // Show WS data rate
    public Text WS_Datarate_display;
    int dataCount;
    float dataCountStartTime;
    Quaternion last_frame_data;

    public GameObject pause_panel;

    void Start()
    {
        av_last_update_time = Time.time;

        dataCount = 0;
        dataCountStartTime = Time.time;
        last_frame_data = Quaternion.identity;

        ws = new WebSocket("ws://localhost:" + portNum.ToString());
        ws.ConnectAsync();
        ws.OnMessage += (sender, message) =>
        {
            // Debug.Log("data received: "+message.Data);
            if(message.Data.Substring(0, 4) == "gyro")
            {
                string gyroInfo = message.Data.Substring(5);
                raw_x = float.Parse(gyroInfo.Split(' ')[1]);
                raw_y = float.Parse(gyroInfo.Split(' ')[0]);
                raw_z = float.Parse(gyroInfo.Split(' ')[2]);
                y = -raw_y;
                x = 90 - raw_x;
                //z = -float.Parse(e.Data.Split(' ')[2]);
            }else if (message.Data.Substring(0, 3) == "UID")
            {
                if (player == 1)
                    UID1 = message.Data.Substring(4);
                else 
                    UID2 = message.Data.Substring(4);
            }
        };
    }
    void Update()
    {
        pos_x = (raw_y < 180)? ((raw_y > 90)? raw_y - 180.0f:-raw_y): ((raw_y < 270)? raw_y - 180.0f :360.0f - raw_y);
        pos_y = (raw_x > 90)? 180.0f-raw_x:((raw_x<-90)? -180.0f - raw_x:raw_x);
        pos_x = (raw_x > 90 || raw_x < -90)? -pos_x:pos_x;
        new_av(pos_x, pos_y);
        if (pause_panel == null || !pause_panel.activeSelf)
        {
            lightsaber.transform.rotation = Quaternion.Euler(x, y, z);
            lightsaber.transform.position = new Vector3(pos_x / 9.0f + origin_x, pos_y / 9.0f, 0);
        }

        // show data change rate (as a representation of how smooth the blade is running)
        Quaternion new_frame_data = lightsaber.transform.rotation;
        if (last_frame_data != new_frame_data)
        {
            dataCount++;
            last_frame_data = new_frame_data;
        }
        //Debug.Log(dataCount);
        if (Time.time - dataCountStartTime > 0.5f)
        {
            if (WS_Datarate_display != null)
                WS_Datarate_display.text = (((float)dataCount) / (Time.time - dataCountStartTime)).ToString("N") + " DPS";
            dataCount = 0;
            dataCountStartTime = Time.time;
        }
    }


    void new_av(float _x, float _y)
    {
        if (Time.time - av_last_update_time < 1.0/60.0) return;
        deltaRotation = (new Vector2(_x, _y) - last_axy) / (Time.time - av_last_update_time);
        last_axy = new Vector2(_x, _y);
        av_last_update_time = Time.time;

        past_av[past_av_ind] = deltaRotation.magnitude;
        past_av_ind = (past_av_ind == 9) ? 0 : past_av_ind + 1;

        float temp = 0;
        foreach (float val in past_av)
            temp = (val > temp) ? val : temp;

        if(player == 1)
            blade_av1 = temp;
        else 
            blade_av2 = temp;
    }
}
