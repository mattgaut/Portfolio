using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerTagsHolder : MonoBehaviour
{
    //key - player_index
    //value - player tag (name seen in game)
    Dictionary<int, string> player_tags;

    void Awake()
    {
        player_tags = new Dictionary<int, string>();
    }

    public void AddPair(int player_index, string playerTag)
    {
        if (!player_tags.ContainsKey(player_index))
            player_tags.Add(player_index, playerTag);
    }

    public string GetPlayerTag(int player_index)
    {
        if (player_tags.ContainsKey(player_index)) {
            return player_tags[player_index];
        }
        return "P" + (player_index + 1);

    }

}
