using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    public static string background;
    public static float
        SFX_volume = 0.3f,
        bolt_speed = 150f,  // bolt speed = unit distance travelled per second
        music_delay = 0,
        music_volume = 1;
    public Slider
        SFX_slider,
        bolt_speed_slider,
        music_delay_slider;
    public Text
        SFX_display,
        bolt_speed_display,
        music_delay_display;
    public Button
        Button1,
        Button2,
        Button3;

    private void Start()
    {
        // Application.targetFrameRate = 75;
        // QualitySettings.vSyncCount = 1;

        if (PlayerPrefs.HasKey("background")){
            background = PlayerPrefs.GetString("background");
            if (background == "bg1") {
                Button1.Select();
                
            }
            else {
                    if (background == "bg2") {
                        Button2.Select();
                    }
                    else {
                        Button3.Select();
                    }
                }
            Debug.Log("Game data loaded!");
	    }
	    else {
            background = "bg1";
            Debug.Log("no data");
            Button1.Select();
        }
		   

        SFX_slider.value = SFX_volume;
        bolt_speed_slider.value = bolt_speed;
        music_delay_slider.value = music_delay;

        SFX_slider.onValueChanged.AddListener((v) => SFX_volume = v);
        bolt_speed_slider.onValueChanged.AddListener((v) => bolt_speed = v);
        music_delay_slider.onValueChanged.AddListener((v) => music_delay = v);
    }

    private void Update()
    {
        SFX_display.text = SFX_volume.ToString("N");
        bolt_speed_display.text = bolt_speed.ToString("N");
        music_delay_display.text = music_delay.ToString("N");
    }
}
