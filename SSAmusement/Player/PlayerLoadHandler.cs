using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLoadHandler : MonoBehaviour {

    [SerializeField] List<string> destroy_on;

    private void Start() {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene loaded, LoadSceneMode mode) {
        if (destroy_on.Contains(loaded.name)) {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(gameObject);
        } else {
            Tester tester = FindObjectOfType<Tester>();
            if (tester) {
                transform.position = tester.spawn_point;
            }
            foreach (Item i in GetComponent<Player>().inventory.items) {
                UIHandler.DisplayItem(i, false);
            }
        }
    }
}
