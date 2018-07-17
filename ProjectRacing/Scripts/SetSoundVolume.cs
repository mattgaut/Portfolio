using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetSoundVolume : MonoBehaviour {


    public void SetMusic(Slider s) {
        GameManager.instance.sound_manager.SetMusicVolume(s.value);
    }
    public void SetFX(Slider s) {
        GameManager.instance.sound_manager.SetSFXVolume(s.value);
    }
}
