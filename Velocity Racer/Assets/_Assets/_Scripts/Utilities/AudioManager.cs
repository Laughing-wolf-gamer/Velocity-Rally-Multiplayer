using System;
using UnityEngine;
using UnityEngine.Audio;
namespace GamerWolf.Utils{
    public class AudioManager : MonoBehaviour{
        public static AudioManager Current{get;private set;}
        [SerializeField] private AudioMixer mainMixer;
        [SerializeField] private Sounds[] sounds;
        [SerializeField] private AudioSource musicAudioSource,sfxAudioSource;
        [SerializeField] private GameDataSO gameData;
        private void Awake(){
            if(Current == null){
                Current = this;
            }else{
                Destroy(Current.gameObject);
            }
            DontDestroyOnLoad(Current.gameObject);
        }
        private void Update() {
            mainMixer.SetFloat("Music Volume",Mathf.Log10(gameData.GetMusicVolumeAmount()) * 20);
            mainMixer.SetFloat("SFX Volume",Mathf.Log10(gameData.GetSfxVolumeAmount()) * 20);
            // sfxAudioSource.volume = gameData.GetSfxVolumeAmount();
        }
        public void PlayMusic(Sounds.SoundType soundType){
            Sounds s = Array.Find(sounds ,s => s.soundType == soundType);
            if(s != null){
                if(!s.isSfx){
                    if(musicAudioSource.isPlaying) return;
                    SetAudioSettings(soundType);
                    musicAudioSource.clip = s.audioClip;
                    musicAudioSource.Play();
                }
            }
        }
        public void PauseMusic(Sounds.SoundType soundType){
            Sounds s = Array.Find(sounds ,s => s.soundType == soundType);
            if(s != null){
                if(musicAudioSource.isPlaying) return;
                SetAudioSettings(soundType);
                if(musicAudioSource.clip != null){
                    musicAudioSource.Pause();
                }
            }
        }
        public void PlayOneShotMusic(Sounds.SoundType soundType){
            Sounds s = Array.Find(sounds ,s => s.soundType == soundType);
            if(s != null){
                if(sfxAudioSource.isPlaying) return;
                SetAudioSettings(soundType);
                sfxAudioSource.PlayOneShot(s.audioClip);
            }
        }
        public void PlaySFXAudio(Sounds.SoundType soundType){
            Sounds s = Array.Find(sounds ,s => s.soundType == soundType);
            if(s.isSfx){
                if(sfxAudioSource.isPlaying) return;
                SetAudioSettings(soundType);
                sfxAudioSource.Play();
            }
        }
        public void StopAudio(Sounds.SoundType soundType){
            Sounds s = Array.Find(sounds ,s => s.soundType == soundType);
            SetAudioSettings(soundType);
            if(s.isSfx){
                sfxAudioSource.Stop();
            }else{
                musicAudioSource.Stop();
            }
        }
        public void SetAudioSettings(Sounds.SoundType soundType){
            Sounds s = Array.Find(sounds ,s => s.soundType == soundType);
            if(s.isSfx){
                sfxAudioSource.pitch = s.pitchSlider;
                sfxAudioSource.loop = s.isLooping;
                sfxAudioSource.volume = gameData.GetSfxVolumeAmount();
            }else{
                musicAudioSource.pitch = s.pitchSlider;
                musicAudioSource.loop = s.isLooping;
                musicAudioSource.volume = gameData.GetMusicVolumeAmount();
            }
        }

        public float GetSfxVolumeAmount() {
            return gameData.GetSfxVolumeAmount();
        }

        public void PlayAudioAtPos(AudioSource boosterAudioSource, Sounds.SoundType soundType,bool playOneShot = false) {
            Sounds s = Array.Find(sounds ,s => s.soundType == soundType);
            if(s.isSfx){
                boosterAudioSource.pitch = s.pitchSlider;
                boosterAudioSource.loop = s.isLooping;
                // boosterAudioSource.volume = gameData.GetSfxVolumeAmount();
            }else{
                boosterAudioSource.pitch = s.pitchSlider;
                boosterAudioSource.loop = s.isLooping;
                // boosterAudioSource.volume = gameData.GetMusicVolumeAmount();
            }
            if(playOneShot){
                boosterAudioSource.PlayOneShot(s.audioClip);
            }else{
                boosterAudioSource.clip = s.audioClip;
                boosterAudioSource.Play();
            }
        }
    }

}