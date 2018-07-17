using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour {

    public void LoadSelectedScene(string scene) {
        SceneManager.LoadScene(scene);
    }
    public void LoadCurrentScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
