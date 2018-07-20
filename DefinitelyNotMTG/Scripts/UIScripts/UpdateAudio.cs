using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateAudio : MonoBehaviour {

    [SerializeField]
    AudioSource source;
    [SerializeField]
    Slider volume;
    [SerializeField]
    List<AudioClip> acs;

    public void Start() {
        source = FindObjectOfType<AudioSource>();
    }

    public void UpdateSlider() {
        source.volume = volume.value;
    }

    public void PlaySong(Dropdown drop) {
        source.clip = acs[drop.value];
        source.Play();
    }
}
