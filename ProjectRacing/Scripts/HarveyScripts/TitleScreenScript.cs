using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class TitleScreenScript : MonoBehaviour {

    [SerializeField]
    private List<Button> menuButtons;

    [SerializeField]
    private int selectedButtonIndex = 0;

    private const int PLAY = 0;
    private const int INSTRUCTIONS = 1;
    private const int QUIT = 2;

    [SerializeField]
    private GameObject instructionsPanel;
    private GameObject optionsPanel;

	// Use this for initialization
	void Start ()
    {
        instructionsPanel = GameObject.Find("InstructionsPanel");
        instructionsPanel.SetActive(false);
        optionsPanel = GameObject.Find("Options");

        menuButtons.Add(optionsPanel.transform.Find("PlayButton").GetComponent<Button>());
        menuButtons.Add(optionsPanel.transform.Find("Instructions").GetComponent<Button>());
        menuButtons.Add(optionsPanel.transform.Find("QuitButton").GetComponent<Button>());
        menuButtons[selectedButtonIndex].Select();

    }
	
	// Update is called once per frame
	//void Update()
 //   {        
 //       if (optionsPanel.activeSelf)
 //       {
 //           if (Input.GetKeyDown(KeyCode.UpArrow))
 //           {
 //               if (--selectedButtonIndex < PLAY)
 //                   selectedButtonIndex = QUIT;
 //               menuButtons[selectedButtonIndex].Select();
 //           }
 //           else if (Input.GetKeyDown(KeyCode.DownArrow))
 //           {
 //               if (++selectedButtonIndex > QUIT)
 //                   selectedButtonIndex = PLAY;
 //               menuButtons[selectedButtonIndex].Select();
 //           }


 //           if (Input.GetButtonDown("Submit"))
 //           {
 //               switch (selectedButtonIndex)
 //               {
 //                   case PLAY:
 //                       SceneManager.LoadScene("PlayerSelectCopy");
 //                       break;
 //                   case INSTRUCTIONS:
 //                       instructionsPanel.SetActive(true);
 //                       optionsPanel.SetActive(false);
 //                       print("instructions");
 //                       break;
 //                   case QUIT:
 //                       Application.Quit();
 //                       print("quit");
 //                       break;
 //                   default:
 //                       print("Invalid button index: " + selectedButtonIndex);
 //                       break;
 //               }
 //           }
 //       }

 //       else if (instructionsPanel.activeSelf)
 //       {
 //           if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
 //               instructionsPanel.transform.GetComponentInChildren<Button>().Select();
 //           if(Input.GetButtonDown("Submit"))
 //           {
 //               optionsPanel.SetActive(true);
 //               instructionsPanel.SetActive(false);
 //           }
 //       }

        
 //   }

    public void PlayGame()
    {
        SceneManager.LoadScene("PlayerSelectCopy");
    }

    public void Instructions()
    {
        optionsPanel.SetActive(false);
        instructionsPanel.SetActive(true);
        instructionsPanel.GetComponentInChildren<Button>().Select();
    }

    public void BackToMenu()
    {
        optionsPanel.SetActive(true);
        instructionsPanel.SetActive(false);
        menuButtons[PLAY].Select();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
