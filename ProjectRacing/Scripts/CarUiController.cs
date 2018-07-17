using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarUiController : MonoBehaviour {

    [SerializeField]
    Image game_over_screen, item_display, splotch, speed_slider, live_1, live_2, live_3;
    [SerializeField]
    Text lives_remaining, speed, place_text;
    [SerializeField]
    ItemTimerHolder _item_timer;
    public ItemTimerHolder item_timer {
        get { return _item_timer; }
    }

    public void SetLivesRemaining(int number) {
        live_1.enabled = (number > 0 && GameManager.instance.IsSurvivalMode());
        live_2.enabled = (number > 1 && GameManager.instance.IsSurvivalMode());
        live_3.enabled = (number > 2 && GameManager.instance.IsSurvivalMode());
    }

    public void SetSpeed(float number) {
        number = (float)System.Math.Round(number, 1);
        speed.text = number + "";
        speed_slider.fillAmount = number / 150f;
    }

    public void SetItem(Item item) {
        if (item == null) {
            item_display.enabled = false;
        } else {
            item_display.enabled = true;
            item_display.sprite = item.sprite;
        }
    }

    public void GameOver(int place) {
        place_text.text = GetPlacementString(place);
        StartCoroutine(FadeGameOverScreen(1f));
    }

    public string GetPlacementString(int place) {
        if (place == 1) {
            return "1st";
        } else if (place == 2) {
            return "2nd";
        } else if (place == 3) {
            return "3rd";
        } else if (place == 4) {
            return "4th";
        }
        return "";
    }

    public void ShowSplotch(bool active) {
        splotch.enabled = active;
    }

    IEnumerator FadeGameOverScreen(float time) {
        float start_time = time;
        game_over_screen.enabled = true;
        game_over_screen.color = new Color(0, 0, 0, 0);
        place_text.enabled = true;
        place_text.color = new Color(1, 1, 1, 0);
        while (time > 0) {
            time -= Time.deltaTime;
            game_over_screen.color = new Color(0,0,0, 1 - time/start_time);
            place_text.color = new Color(1, 1, 1, 1 - time / start_time);
            yield return null;
        }
    }
}
