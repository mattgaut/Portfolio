using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnPanelController : MonoBehaviour {

    [SerializeField]
    Text turn_text, waiting_text;
    [SerializeField]
    Image start, main1, attackers, blockers, damage, main2, end;
    Image current;
    [SerializeField]
    Button pass_button, cancel_button, confirm_button;
    [SerializeField]
    Text phase_text;

    [SerializeField]
    Player player;

    void Awake() {
        pass_button.onClick.AddListener(() => player.PerformAction(new PassPriority()));
        confirm_button.onClick.AddListener(() => player.PerformAction(new ConfirmAction()));
        cancel_button.onClick.AddListener(() => player.PerformAction(new CancelAction()));

        SetButtonSet(ButtonSet.none);
    }

    public void SetPhase(Phase phase) {
        if (current != null) {
            current.color = Color.black;
        }
        switch (phase) {
            case Phase.start:
                current = start;
                phase_text.text = "Current Phase: Start";
                break;
            case Phase.main1:
                current = main1;
                phase_text.text = "Current Phase: Main Phase 1";
                break;
            case Phase.attackers:
                current = attackers;
                phase_text.text = "Current Phase: Declare Attackers";
                break;
            case Phase.blockers:
                current = blockers;
                phase_text.text = "Current Phase: Declare Blockers";
                break;
            case Phase.damage:
                current = damage;
                phase_text.text = "Current Phase: Damage";
                break;
            case Phase.main2:
                current = main2;
                phase_text.text = "Current Phase: Main Phase 2";
                break;
            case Phase.end:
                phase_text.text = "Current Phase: End";
                current = end;
                break;
        }
        current.color = Color.green;
    }

    public void SetWaiting(bool players_priority) {
        if (players_priority) {
            //pass_button.interactable = true;
            waiting_text.text = "Waiting for you...";
        } else {
           // pass_button.interactable = false;
            waiting_text.text = "Waiting for opponent...";
        }
    }

    public void SetWaiting(string text) {
        waiting_text.text = text;
    }

    public void SetButtonSet(ButtonSet set) {
        if (set == ButtonSet.none) {
            cancel_button.gameObject.SetActive(false);
            confirm_button.gameObject.SetActive(false);
            pass_button.gameObject.SetActive(false);
        } else if (set == ButtonSet.confirm) {
            cancel_button.gameObject.SetActive(false);
            confirm_button.gameObject.SetActive(true);
            confirm_button.interactable = true;
            pass_button.gameObject.SetActive(false);
        } else if (set == ButtonSet.confirm_cancel) {
            cancel_button.gameObject.SetActive(true);
            confirm_button.gameObject.SetActive(true);
            confirm_button.interactable = true;
            pass_button.gameObject.SetActive(false);
        } else if (set == ButtonSet.pass) {
            cancel_button.gameObject.SetActive(false);
            confirm_button.gameObject.SetActive(false);
            pass_button.gameObject.SetActive(true);
        } else if (set == ButtonSet.cancel) {
            cancel_button.gameObject.SetActive(true);
            confirm_button.gameObject.SetActive(false);
            pass_button.gameObject.SetActive(false);
        }
    }

    public void ConfirmButtonInteractable(bool interactable) {
        confirm_button.interactable = interactable;
    }

    public void SetTurn(bool players_turn) {
        if (players_turn) {
            turn_text.text = "Your Turn";
        } else {
            turn_text.text = "Their Turn";
        }
    }
}
