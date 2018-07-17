using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPositionUI : MonoBehaviour {

    [SerializeField]
    Slider player1, player2, player3, player4;

    Coroutine player1_routine, player2_routine, player3_routine, player4_routine;

    public void SetPosition(int player_number, float target_fill) {
        if (player_number == 0) {
            SetPlayer1(target_fill);
        } else if (player_number == 1) {
            SetPlayer2(target_fill);
        } else if (player_number == 2) {
            SetPlayer3(target_fill);
        } else if (player_number == 3) {
            SetPlayer4(target_fill);
        }
    }

    public void Disable(int player_number) {
        if (player_number == 0) {
            player1.enabled = false;
        } else if (player_number == 1) {
            player2.enabled = false;
        } else if (player_number == 2) {
            player3.enabled = false;
        } else if (player_number == 3) {
            player4.enabled = false;
        }
    }

    public void SetColor(int player_number, Color color) {
        Slider s = null;
        if (player_number == 0) {
            s = player1;
        } else if (player_number == 1) {
            s = player2;
        } else if (player_number == 2) {
            s = player3;
        } else if (player_number == 3) {
            s = player4;
        }
        foreach (Image i in s.GetComponentsInChildren<Image>()) {
            i.color = color;
        }
    }

    void SetPlayer1(float target) {
        if (player1_routine != null) {
            StopCoroutine(player1_routine);
        }
        player1_routine = StartCoroutine(LerpFill(player1, target));
    }
    void SetPlayer2(float target) {
        if (player2_routine != null) {
            StopCoroutine(player2_routine);
        }
        player2_routine = StartCoroutine(LerpFill(player2, target));
    }
    void SetPlayer3(float target) {
        if (player3_routine != null) {
            StopCoroutine(player3_routine);
        }
        player3_routine = StartCoroutine(LerpFill(player3, target));
    }
    void SetPlayer4(float target) {
        if (player4_routine != null) {
            StopCoroutine(player4_routine);
        }
        player4_routine = StartCoroutine(LerpFill(player4, target));
    }

    IEnumerator LerpFill(Slider s, float target) {
        float difference = s.value - target;

        float start_time = 0.5f, current_time = 0;

        while (current_time < start_time) {
            current_time += Time.deltaTime;

            s.value = target + (difference * (1 - current_time / start_time));

            yield return null;
        }

        s.value = target;
    }
}
