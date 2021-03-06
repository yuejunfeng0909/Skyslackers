using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using WebSocketSharp;
using System;

public class SongManager : MonoBehaviour
{
    static public TextAsset musicMap;
    static public AudioClip musicFile;

    static public string scenename = "game";
    static public string songname;

    WebSocket ws;
    
    public GameObject LoadingAlert;

    void Start()
    {
        songname = "";
        if (musicMap == null) {
            musicMap = Resources.Load<TextAsset>("Music/Imperial March (Keyzee Trap Remix)_csv");
        }
        if (musicFile == null) {
            musicFile = Resources.Load<AudioClip>("Music/Imperial March (Keyzee Trap Remix)");
        }
        ws = new WebSocket("ws://localhost:18080");
        Debug.Log("songmanager online");
        ws.ConnectAsync();
        ws.OnMessage += (sender, message) =>
        {
            if (message.Data.Substring(0, 13) == "musicselected")
            {   
                Debug.Log("loading music");
                LoadingAlert.SetActive(true);

                string URLinfo = message.Data.Substring(15);
                string mp3URL = URLinfo.Split(' ')[0];
                string csvURL = URLinfo.Split(' ')[1];

                string source = mp3URL.Substring(mp3URL.IndexOf("musicFile%2F")+12,
                mp3URL.IndexOf("_song.mp3")-mp3URL.IndexOf("musicFile%2F")-12); //extract the exact name of the song from the firebase song
                string[] stringSeparators = new string[] {"%20"};
                string[] result;
                result = source.Split(stringSeparators, StringSplitOptions.None);
                // Debug.Log(result);
                foreach (String i in result) {
                    // Debug.Log(i);
                    songname+=(i+' ');
                }
                songname = songname.Remove(songname.Length - 1, 1);  
                Debug.Log(songname);
                StartCoroutine(LoadGameFiles(mp3URL, csvURL));
            }
        };
    }

    IEnumerator LoadGameFiles(string pathToMusic, string pathToMap)
    {
        Debug.Log("loading music & map");
        using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(pathToMusic, AudioType.MPEG)) {
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.ConnectionError ||
                uwr.result == UnityWebRequest.Result.ProtocolError) {
                Debug.Log(uwr.error);
            }
            else {
                //Debug.Log("File://"+Applicaion.persistentDataPath + pathToMusic);
                // Get downloaded asset bundle
                musicFile = DownloadHandlerAudioClip.GetContent(uwr);
                // musicFile.LoadAudioData();
                // Something with the texture e.g.
                // store it to later access it by fileName
                // musicFile = music_file;
                Debug.Log(musicFile);
                Debug.Log(musicFile.length);
                ws.Send("Loaded "+songname);
            }
        }
        using (UnityWebRequest uwr = UnityWebRequest.Get(pathToMap)) {
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.ConnectionError ||
                uwr.result == UnityWebRequest.Result.ProtocolError) {
                Debug.Log(uwr.error);
            }
            else {
                Debug.Log(uwr);
                // Get downloaded asset bundle
                // Something with the texture e.g.
                // store it to later access it by fileName
                musicMap = new TextAsset(uwr.downloadHandler.text);;
                Debug.Log(musicMap);
            }
        }
        while (musicMap == null || musicFile == null)
        {
            yield return null;
        }
        ws.Close();
        SceneManager.LoadScene(scenename);
    }

    public void setSong(string musicName)
    {
        musicMap = Resources.Load<TextAsset>("Music/" + musicName + "_csv");
        musicFile = Resources.Load<AudioClip>("Music/" + musicName);
        SceneManager.LoadScene(scenename);
        // SceneManager.LoadScene("Game2",LoadSceneMode.Additive);
        songname = musicName;
    }
}
