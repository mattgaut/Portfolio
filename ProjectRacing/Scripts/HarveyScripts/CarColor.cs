using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CarColor : MonoBehaviour {

    [SerializeField]
    private CarMaterials carMaterials;


    //always under Resources Folder
    public string path = "MyColors/";
    public int player_number = 0;

    private const string guardColor = "GuardColor";
    private const string headlightColor = "HeadlightColor";
    private const string primaryColor = "PrimaryColor";
    private const string wheelColor = "WheelColor";
    private const string windowTintColor = "WindowTintColor";

    private GameObject SkyCarModel;
    private GameObject SkyCarBody;
    private GameObject SkyCarComponents;//window tint
    private GameObject[] SkyCarMudguards = new GameObject[2];//front right and front left
    private GameObject[] SkyCarWheels = new GameObject[4];

    void Awake()
    {
        Player player = GetComponent<Player>();
        player_number = player.player_number;

        string carColorFolder = GameManager.instance.ColorHolder.GetCarColor(player_number);
        path += carColorFolder + "/";

        carMaterials.mudguardColor = Resources.Load<Material>(path + guardColor);
        carMaterials.headlightColor = Resources.Load<Material>(path + headlightColor);
        carMaterials.primaryColor = Resources.Load<Material>(path + primaryColor);
        carMaterials.wheelColor = Resources.Load<Material>(path + wheelColor);
        carMaterials.windowTint = Resources.Load<Material>(path + windowTintColor);

        Transform modelTransform = transform.Find("SkyCarModel");
        Assert.IsNotNull(modelTransform, "SkyCarModel in CarColor.cs is null");
        SkyCarModel = modelTransform.gameObject;

        SkyCarBody = SkyCarModel.transform.Find("SkyCarBody").gameObject;
        SkyCarComponents = SkyCarModel.transform.Find("SkyCarComponents").gameObject;
        SkyCarMudguards[0] = SkyCarModel.transform.Find("SkyCarMudGuardFrontLeft").gameObject;
        SkyCarMudguards[1] = SkyCarModel.transform.Find("SkyCarMudGuardFrontRight").gameObject;
        SkyCarWheels[0] = SkyCarModel.transform.Find("SkyCarWheelFrontLeft").gameObject;
        SkyCarWheels[1] = SkyCarModel.transform.Find("SkyCarWheelFrontRight").gameObject;
        SkyCarWheels[2] = SkyCarModel.transform.Find("SkyCarWheelRearLeft").gameObject;
        SkyCarWheels[3] = SkyCarModel.transform.Find("SkyCarWheelRearRight").gameObject;
    }

    void Start()
    {
        SkyCarBody.GetComponent<MeshRenderer>().material = carMaterials.primaryColor;
        SkyCarComponents.GetComponent<MeshRenderer>().material = carMaterials.windowTint;
        
        foreach(GameObject mudguard in SkyCarMudguards)
        {
            mudguard.GetComponent<MeshRenderer>().material = carMaterials.mudguardColor;
        }

        foreach(GameObject wheel in SkyCarWheels)
        {
            wheel.GetComponent<MeshRenderer>().material = carMaterials.wheelColor;
        }

        GeneralUIController.instance.SetColor(player_number, CarStatics.StringToColor(GameManager.instance.ColorHolder.GetCarColor(player_number)));
    }

}
