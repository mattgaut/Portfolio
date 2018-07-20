using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : Slider {

    PlayerCharacter player;

    [SerializeField]
    GameObject heart;

    List<UIHeart> hearts;

    void Awake() {
        hearts = new List<UIHeart>();

    }

    protected override void Start() {
        player = GameManager.instance.player;
    }

    protected override void SetImage() {
        if (player == null)
            player = GameManager.instance.player;
        if (player != null)
            UpdateHearts();
        else Debug.Log("Player Null");
    }

    void UpdateHearts() {
        while ((int)player.max_health.value > hearts.Count * 2) {
            hearts.Add(Instantiate(heart).GetComponent<UIHeart>());
            hearts[hearts.Count - 1].transform.SetParent(transform);
            hearts[hearts.Count - 1].GetComponent<RectTransform>().localPosition = new Vector3(50 * (hearts.Count - 1), -50, 0);

        }
        if ((int)player.max_health.value % 2 == 1) {
                hearts[hearts.Count - 1].half_heart = true;
        }
        while ((int)player.max_health.value < hearts.Count) {
            Destroy(hearts[hearts.Count - 1].gameObject);
            hearts.RemoveAt(hearts.Count - 1);
        }
        for (int i = 0; i < hearts.Count; i++) {
            hearts[i].SetState((int)((player.health - 2*i)));
        }
    }
}
