using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTagIndicator : MonoBehaviour {

    public int player_index;
    [SerializeField]
    private string defaultName;
    private InputField inputField;
    
    [SerializeField]
    private string playerName;
    public string PlayerName
    {
        get { return playerName; }
        set { playerName = value; }
    }

	void Start ()
    {
        inputField = transform.Find("PlayerTagField").GetComponent<InputField>();
        inputField.text = defaultName + (player_index + 1).ToString();
        playerName = inputField.text;
    }
	
    public void OnPlayerTagValueChanged()
    {
        playerName = inputField.text;
    }

}
