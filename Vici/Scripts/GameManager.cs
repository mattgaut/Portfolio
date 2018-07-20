using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    [SerializeField]
    GameObject player_obj;

    public static GameManager instance {
        get; private set;
    }

    public PlayerCharacter player {
        get; private set;
    }

    public void Restart() {
        StartCoroutine(FadeOutReloadScene());
    }

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }
        if (player == null) {
            player = FindObjectOfType<PlayerCharacter>();
            if (player == null) {
                player = Instantiate(player_obj, Vector3.zero, Quaternion.identity).GetComponent<PlayerCharacter>();
            }
        }
    }

    void Start() {
        player.SetSlider(UIController.instance.player_health_bar);
        StartCoroutine(FadeInGiveControl());
        SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) => OnLevelFinishedLoading(scene, mode);
    }

    IEnumerator FadeInGiveControl() {
        player.GetComponent<PlayerController>().player_has_control = false;

        yield return UIController.instance.FadeIn(0.5f);

        player.GetComponent<PlayerController>().player_has_control = true;
    }
    IEnumerator FadeOutReloadScene() {
        player.GetComponent<PlayerController>().player_has_control = false;

        yield return UIController.instance.FadeOut(0.5f);

        Destroy(player.gameObject);

        yield return null;
        yield return null;

        SceneManager.LoadScene(1);
    }

    void OnLevelFinishedLoading(Scene s, LoadSceneMode load) {
        if (player != null) {
            player.transform.position = Vector3.zero;
            player.OnLevelFinishedLoading(s, load);
        }
    }
}
