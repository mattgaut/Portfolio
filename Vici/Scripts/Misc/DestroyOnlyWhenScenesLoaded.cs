using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyOnlyWhenScenesLoaded : MonoBehaviour {

    [SerializeField]
    List<int> to_destroy;

    void Awake() {
        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) => OnLevelFinishedLoading(scene, mode);
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
        if (to_destroy.Contains(scene.buildIndex)) {
            Destroy(gameObject);
        }
    }
}
