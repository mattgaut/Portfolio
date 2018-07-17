using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TestScript : MonoBehaviour {

    public int player_index;

    void Update()
    {
        string inputStyle = GameManager.instance.input.GetInputStyle(player_index);
        print(inputStyle);
    }
}
