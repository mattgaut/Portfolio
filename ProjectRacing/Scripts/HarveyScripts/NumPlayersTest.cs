using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumPlayersTest : MonoBehaviour {

    [SerializeField]
    public int playerCount;

	// Use this for initialization
	void Start ()
    {
        playerCount = PlayerPrefs.GetInt("Player Count");
        print("Number of players: " + playerCount);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
