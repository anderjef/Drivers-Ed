using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    private UIMenuManager uiMenuManager;

    // Start is called before the first frame update
    void Start()
    {
        uiMenuManager = FindObjectOfType<UIMenuManager>();
        uiMenuManager.mainMenuPanel.SetActive(true);
        uiMenuManager.optionsPanel.SetActive(false);
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
        //SceneManager.LoadScene("Options"); //was causing problems
        uiMenuManager.optionsPanel.SetActive(true); //show the options panel
        uiMenuManager.mainMenuPanel.SetActive(false); //hide the main menu panel
    }

    public void OpenMenu()
    {
        //SceneManager.LoadScene("Menu"); //was causing problems
        uiMenuManager.mainMenuPanel.SetActive(true); //show the main menu panel
        uiMenuManager.optionsPanel.SetActive(false); //hide the options panel
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
