using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetSliderInitialValue : MonoBehaviour {

    [SerializeField]
    Slider music, sfx;

    void Start() {
        music.value = GameManager.instance.sound_manager.GetMusicVolume();
        sfx.value = GameManager.instance.sound_manager.GetTrueSFXVolume();
    }
}
