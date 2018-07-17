using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour {

    public void PlayGame()
    {
        //SceneManager.LoadScene("PlayerSelect");
       SceneManager.LoadScene("PlayerSelectCopy");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
