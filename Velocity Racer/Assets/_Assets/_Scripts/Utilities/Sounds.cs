using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Audio",menuName = "Gamer Wolf Utilities/Audios")]
public class Sounds : ScriptableObject {
    public enum SoundType{
        IntroSongs_1,
        IntroSongs_2,
        TrackDay_Loop_1,
        TrackDay_Loop_2,
        TrackDay_Loop_Standerd_1,
        TrackDay_Loop_Standerd_2,
        TrackDay_End_1,
        TrackDay_End_2,
        Start_Count_Down_Beep,
        Engine_Sound,
        Skid_Sound,
        Booster,
        Menu_BGM,
        BtnClick,
        BoosterPickUp
    }
    
    public SoundType soundType;
    public AudioClip audioClip;
    public bool isLooping;
    public bool playOnAwake;
    public bool playonShot;
    [Range(0f,1f)]
    public float volumeSlider = 1f;
    [Range(-3f,3f)]
    public float pitchSlider = 1f;
    public bool isMute;
    public bool isSfx;
}
