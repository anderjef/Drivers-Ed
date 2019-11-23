using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    private GameObject canvas, mainMenuPanel, optionsPanel, accessibilityPanel; //these don't require linking because the program will find the objects in the Start function

    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("Canvas");
        for (int i = 0; i < canvas.gameObject.transform.childCount; ++i)
        {
            if (canvas.gameObject.transform.GetChild(i).name == "MainMenuPanel")
            {
                mainMenuPanel = canvas.gameObject.transform.GetChild(i).gameObject;
            }
            else if (canvas.gameObject.transform.GetChild(i).name == "OptionsPanel")
            {
                optionsPanel = canvas.gameObject.transform.GetChild(i).gameObject;
            }
            else if (canvas.gameObject.transform.GetChild(i).name == "AccessibilityPanel")
            {
                accessibilityPanel = canvas.gameObject.transform.GetChild(i).gameObject;
            }
        }
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
        accessibilityPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = this;
        }
        //else if (gameManager != this)
        //{
            //Destroy(gameObject); //was causing issues with reverting to a previous scene; the problem is now that every scene change keeps a GameManager object around
        //}
        DontDestroyOnLoad(gameManager);
    }

    public void GameStart()
    {
        SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
    }

    public void OpenOptions()
    {
        //SceneManager.LoadSceneAsync("Options"); //was causing problems
        optionsPanel.SetActive(true); //show the options panel
        mainMenuPanel.SetActive(false); //hide the main menu panel
    }

    public void OpenMenu()
    {
        //SceneManager.LoadSceneAsync("Menu"); //was causing problems
        mainMenuPanel.SetActive(true); //show the main menu panel
        optionsPanel.SetActive(false); //hide the options panel
        accessibilityPanel.SetActive(false);
    }
    public void OpenAccessibility()
    {
        accessibilityPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    public void TutorialStart()
    {
        SceneManager.LoadSceneAsync("Tutorial", LoadSceneMode.Single);
    }

    public void SandboxStart()
    {
        SceneManager.LoadSceneAsync("Sandbox", LoadSceneMode.Single);
    }

    public void GameEnd()
    {
        SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
    }

    public void Exit()
    {
        Application.Quit(); //kill the application when the exit button is pressed
    }
}
