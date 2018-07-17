using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour {

    [SerializeField]
    float music_volume;
    [SerializeField]
    float sfx_volume;
    bool paused;

    [SerializeField]
    AudioSource music_source;

    public void Awake() {
        SceneManager.sceneLoaded += (a, b) => { FindAllSourcesSetVolume(); paused = false; };
    }

    public float GetMusicVolume() {
        return music_volume;
    }
    public float GetSFXVolume() {
        return paused ? 0 : sfx_volume;
    }
    public float GetTrueSFXVolume() {
        return sfx_volume;
    }
    public void SetMusicVolume(float val) {
        music_source.volume = val;
        music_volume = val;
    }
    public void SetSFXVolume(float val) {
        foreach (AudioSource source in FindObjectsOfType<AudioSource>()) {
            if (source != music_source) {
                source.volume = paused ? 0 : val;
            }
        }
        sfx_volume = val;
    }

    public void PauseSFX() {
        paused = true;
        foreach (AudioSource source in FindObjectsOfType<AudioSource>()) {
            if (source != music_source) {
                source.volume = 0;
            }
        }
    }
    public void UnPauseSFX() {
        paused = false;
        foreach (AudioSource source in FindObjectsOfType<AudioSource>()) {
            if (source != music_source) {
                source.volume = sfx_volume;
            }
        }
    }

    void FindAllSourcesSetVolume() {
        if (GameManager.instance.sound_manager == this) {
            music_source = GetComponent<AudioSource>();
            music_source.volume = music_volume;
            foreach (AudioSource source in FindObjectsOfType<AudioSource>()) {
                if (source != music_source) {
                    source.volume = paused ? 0 : sfx_volume;
                }
            }
        }
    }
}
