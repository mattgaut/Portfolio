using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;
using UnityEngine.Assertions;

[RequireComponent(typeof(CarController))]
[RequireComponent(typeof(Player))]
public class CarPlayerController : MonoBehaviour {
    private CarController m_Car; // the car controller we want to use
    private Rigidbody m_Car_body;
    Player player;

    float speed_adjustment_ratio = 0.6f;

    private void Awake() {
        // get the car controller
        m_Car = GetComponent<CarController>();
        m_Car_body = m_Car.GetComponent<Rigidbody>();
        player = GetComponent<Player>();
    }


    private void FixedUpdate() {
        // pass the input to the car!
        float h = GameManager.instance.input.GetSteer(player.player_number);
        float v = GameManager.instance.input.GetGas(player.player_number);
        float handbrake = GameManager.instance.input.GetHandbrake(player.player_number);

        // adjust steering to be less sharp at higher speeds
        h *= (1 - m_Car.CurrentSpeed / m_Car.MaxSpeed) * speed_adjustment_ratio + (1 - speed_adjustment_ratio);


        if (player.has_control && !GeneralUIController.instance.paused) m_Car.Move(h, v, v, handbrake);

    }
}
