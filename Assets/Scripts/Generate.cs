using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Generate : MonoBehaviour
{
    public static int myScore1 = 0;
    public static int myCombo1 = 0;
    public static int myScore2 = 0;
    public static int myCombo2 = 0;

    Song currentSong;

    static public AudioSource musicFile;
    static public float music_current_time, totalMusicLength;

    public int player;
    public GameObject beam;
    public float max_bolt_x, max_bolt_y;
    public float origin_x;
    static public float bolt_x_offset = 0, bolt_y_offset = 5, bolt_z_offset = 20;
    public GameObject combo_num;
    public GameObject combo;
    public GameObject score;

    public int layer;
    float StartTime, NextBoltTime;
    string NextBoltType;

    void Start()
    {
        currentSong = new Song(SongManager.musicMap, SettingsController.bolt_speed);
        Debug.Log(SongManager.musicMap);
        musicFile = GetComponent<AudioSource>();
        musicFile.clip = SongManager.musicFile;
        musicFile.Play();
        totalMusicLength = musicFile.clip.length;
        InstantiateAllBolts();
    }

    void Update()
    {
        // alter game settings
        musicFile.volume = SettingsController.music_volume;
        music_current_time = musicFile.time;
        if (player == 1) {
            score.GetComponent<Text>().text = " " + myScore1 + " ";
            if (myCombo1 > 0) {
                combo.GetComponent<Text>().text = "Combo";
                combo_num.GetComponent<Text>().text = " " + myCombo1;
            }
            else {
                combo.GetComponent<Text>().text = "";
                combo_num.GetComponent<Text>().text = "";
            }
        }
        else {
            score.GetComponent<Text>().text = " " + myScore2 + " ";
            if (myCombo2 > 0) {
                combo.GetComponent<Text>().text = "Combo";
                combo_num.GetComponent<Text>().text = " " + myCombo2;
            }
            else {
                combo.GetComponent<Text>().text = "";
                combo_num.GetComponent<Text>().text = "";
            }
        }
        
    }

    void InstantiateAtPosition(char pos, float music_timing)
    {
        // assume 123QEASD cooresponds to 8 possible positions on screen.
        float x, y;
        if (pos == 'Q' || pos == 'A' || pos == 'Z')
            x = -max_bolt_x+origin_x;
        else if (pos == 'W' || pos == 'X')
            x = origin_x;
        else
            x = max_bolt_x+origin_x;

        if (pos == 'Q' || pos == 'W' || pos == 'E')
            y = max_bolt_y;
        else if (pos == 'A' || pos == 'D')
            y = 0;
        else
            y = -max_bolt_y;

        GameObject generatedBolt = Instantiate(beam, new Vector3(bolt_x_offset + x, bolt_y_offset + y,
            bolt_z_offset + (music_timing + SettingsController.music_delay) * SettingsController.bolt_speed),
            Quaternion.identity);
        // generatedBolt.layer = layer;
        generatedBolt.GetComponent<BeamControl>().spawn_position = bolt_z_offset + music_timing * SettingsController.bolt_speed;
        generatedBolt.GetComponent<BeamControl>().player = player;
    }

    void InstantiateAllBolts()
    {
        while (currentSong.NotFinished())
        {
            InstantiateAtPosition(currentSong.GetBoltType().ToCharArray()[0],
                currentSong.GetHitTime());
            currentSong.PrepareNext();
        }
    }
}

public class Song
{
    public string raw_file;
    string[] each_line;
    int line_count;
    int current_line = 0;
    public float bolt_speed;

    public Song(TextAsset songFile, float boltSpeed)
    {
        raw_file = songFile.text;
        each_line = raw_file.Split('\n');
        line_count = each_line.Length;
        bolt_speed = boltSpeed;
    }

    public bool NotFinished()
    {
        return current_line < line_count;
    }

    public string NextLine()
    {
        if (current_line < line_count)
            return each_line[current_line++];
        else
            return "";
    }

    public float GetHitTime()
    {
        if (current_line < line_count)
        {   
            if (each_line[current_line].Contains("/")) {
                string[] timeComponent = each_line[current_line].Split(',')[0].Split('/');
                // assume time data follow the following convention:
                //                   7394761/720000
                // which corresponds to seconds
                float generationTime = float.Parse(timeComponent[0])
                    / float.Parse(timeComponent[1]);
                return generationTime;
            }
            else {
                return float.Parse(each_line[current_line].Split(',')[0]);
            }
            
        }
        else
            return 10000;
    }

    public string GetBoltType()
    {
        if (current_line < line_count)
        {
            // Debug.Log(each_line[current_line].Split(',')[1]);
            return each_line[current_line].Split(',')[1];
        }
        else
            return "end";
    }

    public void PrepareNext()
    {
        if (current_line < line_count)
            current_line++;
    }
}