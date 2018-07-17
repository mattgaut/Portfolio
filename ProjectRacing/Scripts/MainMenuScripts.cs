using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class MainMenuScripts : MonoBehaviour {

    [SerializeField]
    GameObject number_select, controls;

    [SerializeField]
    Dropdown player_1_dropdown, player_2_dropdown, player_3_dropdown, player_4_dropdown;

    [SerializeField]
    ColorDropdown color_1_dropdown, color_2_dropdown, color_3_dropdown, color_4_dropdown;

    [SerializeField]
    PlayerTagIndicator player_1_tag, player_2_tag, player_3_tag, player_4_tag;

    [SerializeField]
    GameObject player_1_selection_panel, player_2_selection_panel, player_3_selection_panel, player_4_selection_panel;

    [SerializeField]
    Button start_button;

    [SerializeField]
    List<string> scenes, sprint_scenes;

    int player_count = 0;

    //get button components from panel
    void Awake()
    {
        Assert.IsNotNull(player_1_selection_panel, "player_1_selection_panel is null in MainMenuScripts.cs");
        Assert.IsNotNull(player_2_selection_panel, "player_2_selection_panel is null in MainMenuScripts.cs");
        Assert.IsNotNull(player_3_selection_panel, "player_3_selection_panel is null in MainMenuScripts.cs");
        Assert.IsNotNull(player_4_selection_panel, "player_4_selection_panel is null in MainMenuScripts.cs");

        player_1_dropdown = player_1_selection_panel.transform.Find("ControlDropdown").GetComponent<Dropdown>();
        color_1_dropdown = player_1_selection_panel.GetComponentInChildren<ColorDropdown>();
        player_1_tag = player_1_selection_panel.GetComponentInChildren<PlayerTagIndicator>();

        player_2_dropdown = player_2_selection_panel.transform.Find("ControlDropdown").GetComponent<Dropdown>();
        color_2_dropdown = player_2_selection_panel.GetComponentInChildren<ColorDropdown>();
        player_2_tag = player_2_selection_panel.GetComponentInChildren<PlayerTagIndicator>();

        player_3_dropdown = player_3_selection_panel.transform.Find("ControlDropdown").GetComponent<Dropdown>();
        color_3_dropdown = player_3_selection_panel.GetComponentInChildren<ColorDropdown>();
        player_3_tag = player_3_selection_panel.GetComponentInChildren<PlayerTagIndicator>();

        player_4_dropdown = player_4_selection_panel.transform.Find("ControlDropdown").GetComponent<Dropdown>();
        color_4_dropdown = player_4_selection_panel.GetComponentInChildren<ColorDropdown>();
        player_4_tag = player_4_selection_panel.GetComponentInChildren<PlayerTagIndicator>();

        Assert.IsNotNull(player_1_dropdown, "player_1_dropdown is null in MainMenuScripts.cs");
        Assert.IsNotNull(player_2_dropdown, "player_2_dropdown is null in MainMenuScripts.cs");
        Assert.IsNotNull(player_3_dropdown, "player_3_dropdown is null in MainMenuScripts.cs");
        Assert.IsNotNull(player_4_dropdown, "player_4_dropdown is null in MainMenuScripts.cs");
    }


    public void LoadSelectControlsScene(int number_players) {
        GameManager.instance.input.SelectInputStyle(0, InputManager.ControllerType.Xbox360);
        GameManager.instance.input.SelectInputStyle(1, InputManager.ControllerType.Xbox360);
        GameManager.instance.input.SelectInputStyle(2, InputManager.ControllerType.Xbox360);
        GameManager.instance.input.SelectInputStyle(3, InputManager.ControllerType.Xbox360);

        player_count = number_players;

        number_select.gameObject.SetActive(false);

        controls.gameObject.SetActive(true);

        //player_1_dropdown.gameObject.SetActive(number_players > 0);
        //player_2_dropdown.gameObject.SetActive(number_players > 1);
       // player_3_dropdown.gameObject.SetActive(number_players > 2);
       // player_4_dropdown.gameObject.SetActive(number_players > 3);

        player_1_selection_panel.SetActive(number_players > 0);
        player_2_selection_panel.SetActive(number_players > 1);
        player_3_selection_panel.SetActive(number_players > 2);
        player_4_selection_panel.SetActive(number_players > 3);   
    }

    public void LoadGame() {
        GameManager.instance.input.LoadInputs(player_count);
        LoadPlayerColors();
        LoadPlayerTags();
        SceneManager.LoadScene(scenes[player_count - 1]);
    }
    public void LoadSprintGame() {
        GameManager.instance.input.LoadInputs(player_count);
        LoadPlayerColors();
        LoadPlayerTags();
        SceneManager.LoadScene(sprint_scenes[player_count - 1]);
    }

    public void Player1Controls() {
        GameManager.instance.input.SelectInputStyle(0, (InputManager.ControllerType)player_1_dropdown.value);
    }
    public void Player2Controls() {
        GameManager.instance.input.SelectInputStyle(1, (InputManager.ControllerType)player_2_dropdown.value);
    }
    public void Player3Controls() {
        GameManager.instance.input.SelectInputStyle(2, (InputManager.ControllerType)player_3_dropdown.value);
    }
    public void Player4Controls() {
        GameManager.instance.input.SelectInputStyle(3, (InputManager.ControllerType)player_4_dropdown.value);
    }

    void LoadPlayerColors()
    {
        switch (player_count)
        {
            case 1:
                Assert.IsNotNull(GameManager.instance.ColorHolder, "color holder is null in MainMenuScripts.cs");
                GameManager.instance.ColorHolder.AddPair(color_1_dropdown.player_index, color_1_dropdown.SelectedColor());
                break;
            case 2:
                GameManager.instance.ColorHolder.AddPair(color_1_dropdown.player_index, color_1_dropdown.SelectedColor());
                GameManager.instance.ColorHolder.AddPair(color_2_dropdown.player_index, color_2_dropdown.SelectedColor());
                break;
            case 3:
                GameManager.instance.ColorHolder.AddPair(color_1_dropdown.player_index, color_1_dropdown.SelectedColor());
                GameManager.instance.ColorHolder.AddPair(color_2_dropdown.player_index, color_2_dropdown.SelectedColor());
                GameManager.instance.ColorHolder.AddPair(color_3_dropdown.player_index, color_3_dropdown.SelectedColor());
                break;
            case 4:
                GameManager.instance.ColorHolder.AddPair(color_1_dropdown.player_index, color_1_dropdown.SelectedColor());
                GameManager.instance.ColorHolder.AddPair(color_2_dropdown.player_index, color_2_dropdown.SelectedColor());
                GameManager.instance.ColorHolder.AddPair(color_3_dropdown.player_index, color_3_dropdown.SelectedColor());
                GameManager.instance.ColorHolder.AddPair(color_4_dropdown.player_index, color_4_dropdown.SelectedColor());
                break;
            default:
                print("Number of players: " + player_count);
                break;
        }

    }

    void LoadPlayerTags()
    {
        switch (player_count)
        {
            case 1:
                print("LoadPlayerTags: " + player_1_tag.PlayerName);
                GameManager.instance.PlayerTags.AddPair(player_1_tag.player_index, player_1_tag.PlayerName);
                break;
            case 2:
                GameManager.instance.PlayerTags.AddPair(player_1_tag.player_index, player_1_tag.PlayerName);
                GameManager.instance.PlayerTags.AddPair(player_2_tag.player_index, player_2_tag.PlayerName);
                break;
            case 3:
                GameManager.instance.PlayerTags.AddPair(player_1_tag.player_index, player_1_tag.PlayerName);
                GameManager.instance.PlayerTags.AddPair(player_2_tag.player_index, player_2_tag.PlayerName);
                GameManager.instance.PlayerTags.AddPair(player_3_tag.player_index, player_3_tag.PlayerName);
                break;
            case 4:
                GameManager.instance.PlayerTags.AddPair(player_1_tag.player_index, player_1_tag.PlayerName);
                GameManager.instance.PlayerTags.AddPair(player_2_tag.player_index, player_2_tag.PlayerName);
                GameManager.instance.PlayerTags.AddPair(player_3_tag.player_index, player_3_tag.PlayerName);
                GameManager.instance.PlayerTags.AddPair(player_4_tag.player_index, player_4_tag.PlayerName);
                break;
            default:
                print("Number of players: " + player_count);
                break;
        }
    }

}
