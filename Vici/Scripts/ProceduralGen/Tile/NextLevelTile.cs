using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelTile : Tile {

    [SerializeField]
    string load_level;

    void OnTriggerEnter2D(Collider2D coll) {
        if ((1 << coll.gameObject.layer & 1 << LayerMask.NameToLayer("Player")) != 0) {
            coll.gameObject.GetComponent<PlayerController>().player_has_control = false;
            StartCoroutine(FadeOutThenLoad());   
        }
    }

    IEnumerator FadeOutThenLoad() {


        yield return UIController.instance.FadeOut(0.5f);

        SceneManager.LoadScene(load_level);
    }
}
