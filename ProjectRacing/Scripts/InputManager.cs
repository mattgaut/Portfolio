using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class InputManager : MonoBehaviour {

    public static GameManager instance {
        get; private set;
    }

    [SerializeField]
    string keyboard, ps4, xbox360;

    public enum ControllerType { Keyboard = 1, Xbox360 = 0 }

    Dictionary<int, ControllerType> selected_player_inputs;
    Dictionary<int, string> selected_gas_inputs, selected_steer_inputs, selected_brake_inputs, selected_item_inputs, selected_reset_inputs, selected_tags_input;

    void Awake() {
        selected_player_inputs = new Dictionary<int, ControllerType>();

        selected_brake_inputs = new Dictionary<int, string>();
        selected_gas_inputs = new Dictionary<int, string>();
        selected_steer_inputs = new Dictionary<int, string>();
        selected_item_inputs = new Dictionary<int, string>();
        selected_reset_inputs = new Dictionary<int, string>();
        selected_tags_input = new Dictionary<int, string>();
    }

    public float GetGas(int player_number) {
        if (!selected_gas_inputs.ContainsKey(player_number)) {
            return Input.GetAxis("XboxGas1");
        }
        return Input.GetAxis(selected_gas_inputs[player_number]);
    }

    public float GetSteer(int player_number) {
        if (!selected_gas_inputs.ContainsKey(player_number)) {
            return Input.GetAxis("XboxHorizontal1");
        }
        return Input.GetAxis(selected_steer_inputs[player_number]);
    }

    public float GetHandbrake(int player_number) {
        if (!selected_gas_inputs.ContainsKey(player_number)) {
            return Input.GetAxis("XboxHandbrake1");
        }
        return Input.GetAxis(selected_brake_inputs[player_number]);
    }

    public bool GetItemInput(int player_number) {
        if (!selected_item_inputs.ContainsKey(player_number)) {
            return Input.GetButtonDown("XboxItem1");
        }
        return Input.GetButtonDown(selected_item_inputs[player_number]);
    }
    public bool GetResetInput(int player_number) {
        if (!selected_item_inputs.ContainsKey(player_number)) {
            return Input.GetButtonDown("XboxReset1");
        }
        return Input.GetButtonDown(selected_reset_inputs[player_number]);
    }
    public bool GetDisplayTags(int player_number) {
        if (!selected_tags_input.ContainsKey(player_number)) {
            return Input.GetButtonDown("XboxTags1");
        }
        return Input.GetButtonDown(selected_tags_input[player_number]);
    }

    public void SelectInputStyle(int player_number, ControllerType keyboard) {
        if (!selected_player_inputs.ContainsKey(player_number)) {
            selected_player_inputs.Add(player_number, keyboard);
        } else {
            selected_player_inputs[player_number] = keyboard;
        }
    }

    public string GetInputStyle(int player_number)
    {
        Assert.IsTrue(player_number < selected_player_inputs.Count, "player_number out of range in InputManager.cs");

        string ans = "nothing";
        switch(selected_player_inputs[player_number])
        {
            case ControllerType.Keyboard:
                ans = "Player " + player_number.ToString() + " Keyboard";
                return ans;
            case ControllerType.Xbox360:
                ans = "Player " + player_number.ToString() + " Xbox360";
                return ans;
            default:
                return ans;
        }
    }

    public void LoadInputs(int player_count) {


        selected_steer_inputs.Clear();
        selected_gas_inputs.Clear();
        selected_brake_inputs.Clear();
        selected_item_inputs.Clear();
        selected_reset_inputs.Clear();
        selected_tags_input.Clear();

        int controller_count = 0;

        for (int i = 0; i < player_count; i++) {
            if (!selected_player_inputs.ContainsKey(i)) continue;

            if (selected_player_inputs[i] == ControllerType.Keyboard) {

                selected_steer_inputs.Add(i, keyboard + "KeyHorizontal");
                selected_gas_inputs.Add(i, keyboard + "Gas");
                selected_brake_inputs.Add(i, keyboard + "Handbrake");
                selected_item_inputs.Add(i, keyboard + "Item");
                selected_reset_inputs.Add(i, keyboard + "Reset");
                selected_tags_input.Add(i, keyboard + "Tags");

            } else if (selected_player_inputs[i] == ControllerType.Xbox360) {
                controller_count = 0;
                selected_steer_inputs.Add(i, xbox360 + "Horizontal" + (controller_count + 1));
                selected_gas_inputs.Add(i, xbox360 + "Gas" + (controller_count + 1));
                selected_brake_inputs.Add(i, xbox360 + "Handbrake" + (controller_count + 1));
                selected_item_inputs.Add(i, xbox360 + "Item" + (controller_count + 1));
                selected_reset_inputs.Add(i, xbox360 + "Reset" + (controller_count + 1));
                selected_tags_input.Add(i, xbox360 + "Tags" + (controller_count + 1));

                controller_count++;
            }
        }
    }
}
