using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayManaPool : MonoBehaviour {

    [SerializeField]
    DisplayMana red, green, blue, black, yellow;

    public void Display(ManaPool mp) {
        red.Display(mp.GetCurrent(ManaType.red), mp.GetMax(ManaType.red));
        green.Display(mp.GetCurrent(ManaType.green), mp.GetMax(ManaType.green));
        blue.Display(mp.GetCurrent(ManaType.blue), mp.GetMax(ManaType.blue));
        black.Display(mp.GetCurrent(ManaType.black), mp.GetMax(ManaType.black));
        yellow.Display(mp.GetCurrent(ManaType.yellow), mp.GetMax(ManaType.yellow));
    }
}
