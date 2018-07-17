using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class UIScript : MonoBehaviour {

    private CarController carController;
    private Text speedText;
    //in MPH ~ formatted to read up to 2 decimal places.
    private float readable_speed;

    void Start()
    {
        speedText = GetComponent<Text>();
        Assert.IsNotNull(speedText, "SpeedText reference is null in UIScript");
        carController = FindObjectOfType<CarController>();
        Assert.IsNotNull(carController, "carController reference is null in UIScript");
        readable_speed = carController.CurrentSpeed;
    }

    void Update()
    {
        print(carController.CurrentSpeed);
        readable_speed = carController.CurrentSpeed;
        string speedFormat = string.Format("{0:0.##}", readable_speed);
        speedText.text = "Speed: " + speedFormat + " MPH";
    }
}
