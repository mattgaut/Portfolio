using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PlayerSelectManager : MonoBehaviour {


    public void LoadGame()
    {
        SceneManager.LoadScene("TrackGeneratorScene2");
        string selectedButton = EventSystem.current.currentSelectedGameObject.name;
        if (selectedButton.Equals("OnePlayerButton"))
        {
            OnePlayer();
        }
        else if (selectedButton.Equals("TwoPlayerButton"))
        {
            TwoPlayer();
        }
        else if (selectedButton.Equals("ThreePlayerButton"))
        {
            ThreePlayer();
        }
        else if(selectedButton.Equals("FourPlayerButton"))
        {
            FourPlayer();
        }
    }

    void OnePlayer()
    {
        PlayerPrefs.SetInt("Player Count", 1);
    }

    void TwoPlayer()
    {
        PlayerPrefs.SetInt("Player Count", 2);
    }

    void ThreePlayer()
    {
        PlayerPrefs.SetInt("Player Count", 3);
    }

    void FourPlayer()
    {
        PlayerPrefs.SetInt("Player Count", 4);
    }
}
