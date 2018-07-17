using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


//A component to the singleton: GameManager
public class ColorHolder : MonoBehaviour {

    public static GameManager instance
    {
        get; private set;
    }

    Dictionary<int, CarStatics.ColorEnum> selected_player_colors;

    void Awake()
    {
        selected_player_colors = new Dictionary<int, CarStatics.ColorEnum>();
    }

    
    public void AddPair(int player_index,CarStatics.ColorEnum color)
    {
        if (!selected_player_colors.ContainsKey(player_index))
            selected_player_colors.Add(player_index, color);
    }

    public string GetCarColor(int player_index)
    {
        Assert.IsTrue(player_index < selected_player_colors.Count && player_index > -1, "Player Index: " + player_index.ToString() + " is out of bounds in ColorHolder.cs");
        return CarStatics.ColorToString(selected_player_colors[player_index]);
    }

    //debug
    public void PrintCarColors()
    {
        int i = 0;
        foreach(CarStatics.ColorEnum color in selected_player_colors.Values)
        {
            print("Player " + i.ToString() + CarStatics.ColorToString(color));
        }
    }
}
