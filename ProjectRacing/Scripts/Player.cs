using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

[RequireComponent(typeof(CarPlayerController))]
public class Player : MonoBehaviour {

    Rigidbody body;

    [SerializeField]
    int _lives;
    [SerializeField]
    Animator anim;
    [SerializeField]
    List<Collider> bumpers;
    [SerializeField]
    CarUiController car_ui_controller;
    [SerializeField]
    int _player_number;
    [SerializeField] bool deactivate_on_startup;
    [SerializeField] TMPro.TextMeshPro text;
    [SerializeField] Camera attached_camera;

    Item held_item;

    public int player_number {
        get { return _player_number; }
    }
    public int lives {
        get { return _lives; }
        private set { _lives = value; }
    }
    public CarUiController attached_ui {
        get { return car_ui_controller; }
    }
    public Camera connected_camera {
        get { return attached_camera; }
    }
    public CarController car {
        get; private set;
    }
    public bool has_control {
        get; private set;
    }
    public bool is_active {
        get; private set;
    }
    public bool finished {
        get; private set;
    }
    int intersecting_another_player;

    Coroutine respawn_routine, boost_routine, wait_for_gas;

    
    public void Awake() {
        body = GetComponent<Rigidbody>();
        has_control = true;
        is_active = true;

        car = GetComponent<CarController>();

        if (deactivate_on_startup)
            Active(false, false);

        SetName(GameManager.instance.PlayerTags.GetPlayerTag(player_number), CarStatics.StringToColor(GameManager.instance.ColorHolder.GetCarColor(player_number)));
    }

    public void Start() {
        if (car_ui_controller != null)
            car_ui_controller.SetLivesRemaining(lives);
        GameManager.instance.AddPlayer(this);
    }

    public void Update() {

        if (!GeneralUIController.instance.paused) {
            if (car_ui_controller != null)
                car_ui_controller.SetSpeed(car.CurrentSpeed);
            if (GameManager.instance.input.GetItemInput(player_number)) {
                UseItem();
            }
            if (GameManager.instance.input.GetResetInput(player_number) && lives > 0 && !finished && car.CurrentSpeed < 15 && is_active) {
                LoseLife();
            }
            if (GameManager.instance.input.GetDisplayTags(player_number)) {
                Debug.Log("NameTag");
                attached_camera.cullingMask ^= 1 << LayerMask.NameToLayer("NameTag");
            }
        }

    }

    void OnTriggerEnter(Collider coll) {
        if (coll.gameObject.CompareTag("Player")) {
            intersecting_another_player++;
        } else if (coll.gameObject.CompareTag("Obstacle")) {
            intersecting_another_player++;
        }
    }

    void OnTriggerExit(Collider coll) {
        if (coll.gameObject.CompareTag("Player")) {
            intersecting_another_player--;
        } else if (coll.gameObject.CompareTag("Obstacle")) {
            intersecting_another_player--;
        }
    }

    public void LoseLife() {
        if (lives > 0 && !finished) {
            lives--;
            GameManager.instance.PrepRespawnPlayer(this);
            if (lives > 0) {
                if (car_ui_controller != null)
                    car_ui_controller.SetLivesRemaining(lives);
            } else {
                has_control = false;
                if (car_ui_controller != null)
                    car_ui_controller.GameOver(GameManager.instance.GetPlayersLeft() + 1);
                GeneralUIController.instance.player_positions.Disable(player_number);
            }
        }
    }

    public void SetName(string name, Color color = default(Color)) {
        if (color == default(Color)) {
            text.color = Color.white;
        } else
            text.color = color;
        text.text = name;
    }

    public bool AquireItem(Item item) {
        if (held_item != null) {
            return false;
        }
        held_item = item;
        car_ui_controller.SetItem(held_item);
        return true;
    }

    public void ClearItem() {
        held_item = null;
        car_ui_controller.SetItem(held_item);
    }

    void UseItem() {
        if (held_item != null && is_active) {
            held_item.UseItem(this);
        }
    }

    public void CrossFinish() {
        GameManager.instance.CrossFinish(this);

        attached_ui.GameOver(GameManager.instance.GetPlayersFinished());
        GeneralUIController.instance.player_positions.SetPosition(player_number, 1);

        finished = true;
    }

    public void Active(bool active, bool respawning = true) {
        is_active = active;
        has_control = active;
        if (active) {
            if (body == null) {
                Debug.Log("GotNULLBody");
                Debug.Log(GameManager.instance.GetPlayers().Count);
                Debug.Log(this);
                body = GetComponent<Rigidbody>();
            }
            body.constraints = RigidbodyConstraints.None;
            if (respawning) {
                if (respawn_routine != null) {
                    StopCoroutine(respawn_routine);
                }
                respawn_routine = StartCoroutine(CollidesAfter(3f));
            }
            if (wait_for_gas != null) {
                StopCoroutine(wait_for_gas);
            }
            wait_for_gas = StartCoroutine(WaitForGasBoost());
        } else {
            body.velocity = Vector3.zero;
            body.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

            if (respawning) {
                foreach (Collider c in bumpers) {
                    c.enabled = false;
                }
                anim.SetBool("Respawning", true);
            }

        }

    }

    IEnumerator WaitForGasBoost() {
        while (GameManager.instance.input.GetGas(player_number) <= 0) {
            yield return null;
        }
        if (boost_routine != null) {
            StopCoroutine(boost_routine);
        }
        boost_routine = StartCoroutine(Boost.BoostOverTime(this, 1f, 0f, 500f));
    }

    IEnumerator CollidesAfter(float time) {
        while (time > 0) {
            time -= Time.deltaTime;
            yield return null;
        }

        while (intersecting_another_player != 0) {
            yield return null;
        }

        foreach (Collider c in bumpers) {
            c.enabled = true;
        }
        anim.SetBool("Respawning", false);
    }
}
