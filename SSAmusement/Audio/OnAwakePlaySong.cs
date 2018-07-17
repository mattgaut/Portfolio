using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnAwakePlaySong : MonoBehaviour {

    [SerializeField] AudioClip on_awake_song;
	// Use this for initialization
	void Start () {
        SoundManager.PlaySong(on_awake_song);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
