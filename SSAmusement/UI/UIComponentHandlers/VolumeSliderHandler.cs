using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeSliderHandler : MonoBehaviour {

    Slider slider;

    private void Awake() {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener( (value) => SoundManager.SetVolume(value) );
    }
    // Use this for initialization
    void Start () {
        slider.minValue = 0;
        slider.maxValue = 1;
        slider.value = SoundManager.GetVolume();
	}
}
