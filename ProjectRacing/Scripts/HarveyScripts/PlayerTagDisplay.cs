using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;


//attached to Car GameObject;
public class PlayerTagDisplay : MonoBehaviour
{
    private Player player;
    private Text playerTagDisplay;
    private GameObject carInstance;

    //must change this string to the name of the car gameobject in the editor!!
    public string CarName;

    void Start()
    {
        //hard coded for single player survival only
        carInstance = GameObject.Find(CarName);
        Assert.IsNotNull(carInstance, "carInstance is null in PlayerTagDisplay.cs");
        player = carInstance.GetComponent<Player>();
        playerTagDisplay = GetComponent<Text>();
        playerTagDisplay.text = GameManager.instance.PlayerTags.GetPlayerTag(player.player_number);
        playerTagDisplay.enabled = false;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            playerTagDisplay.enabled = (playerTagDisplay.enabled == true) ? false : true;
        }
    }
}
