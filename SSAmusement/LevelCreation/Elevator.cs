using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class Elevator : MonoBehaviour {

    [SerializeField] string level_to_load;
    bool loading = false;
    bool button_pressed, in_bounds;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerBoundBox")) {
            OnEnter();
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerBoundBox")) {
            OnLeave();
        }
    }
    private void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerBoundBox")) {
            if (button_pressed && !loading) {
                StartCoroutine(FadeToBlackLoadLevel());
                loading = true;
            }
        }
    }
    private void Update() {
        button_pressed = in_bounds && (button_pressed || Input.GetButtonDown("Interact"));
    }

    IEnumerator FadeToBlackLoadLevel() {
        yield return UIHandler.FadeToBlack(1f);
        SceneManager.LoadScene(level_to_load);
    }

    void OnEnter() {
        in_bounds = true;
    }
    void OnLeave() {
        in_bounds = false;
    }
}
