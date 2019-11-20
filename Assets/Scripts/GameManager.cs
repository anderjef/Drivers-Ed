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
        else if (gameManager != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void GameStart()
    {
        SceneManager.LoadScene("Game");
    }

    public void OpenOptions()
    {
        //SceneManager.LoadScene("Options"); //was causing errors
        uiMenuManager.optionsPanel.SetActive(true); //show the options panel
        uiMenuManager.mainMenuPanel.SetActive(false); //hide the main menu panel
    }

    public void OpenMenu()
    {
        //SceneManager.LoadScene("Menu"); //was causing errors
        uiMenuManager.mainMenuPanel.SetActive(true); //show the main menu panel
        uiMenuManager.optionsPanel.SetActive(false); //hide the options panel
    }

    public void TutorialStart()
    {
        SceneManager.LoadScene("Tutorial"); //FIXME: implement (same as Game scene but lives don't matter and there are instructions?)
    }

    public void GameEnd()
    {
        SceneManager.LoadScene("Menu");
    }

    public void Exit()
    {
        Application.Quit(); //kill the application when the exit button is pressed
    }
}
