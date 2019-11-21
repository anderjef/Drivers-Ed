using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public Image[] hitPoints; //three hearts are displayed in the UI representing player lives
    public Text moneyTxt;
    public GameObject gameOverPanel, pausePanel, startPanel; //one overlay for when the player dies, one for when the game is paused, and one for initially setting difficulty
    private Car car;
    public Slider startDifficulty, difficulty; //used to modify the car's speed (and hence the game's difficulty)

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Tutorial")) //tutorial mode has no starting difficulty setting (automatically set to easiest difficulty)
        {
            Time.timeScale = 0;
            startPanel.SetActive(true);
        }
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        car = FindObjectOfType<Car>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updateDifficulty()
    {
        car.minSpeed = difficulty.value * 5 + 10;
    }

    public void startUpdateDifficulty()
    {
        car.minSpeed = startDifficulty.value * 5 + 10;
    }

    public void UpdateLives(int hp)
    {
        for (int i = 0; i < hitPoints.Length; ++i)
        {
            if (hp > i)
            {
                hitPoints[i].color = Color.white; //a white heart in the upper left corner of the UI represents a life lost
            }
            else
            {
                hitPoints[i].color = Color.black; //a black heart in the upper left corner of the UI represents a life not lost
            }
        }
    }

    public void UpdateMoney(int money)
    {
        moneyTxt.text = money.ToString(); //displaying how many coins the player has collected
    }

    public void OpenPause()
    {
        difficulty.value = (car.minSpeed - 10) / 5;
        pausePanel.SetActive(true); //show the pause panel when the pause button is pressed
        Time.timeScale = 0;
    }

    public void ClosePause()
    {
        if (car.speed < car.minSpeed)
        {
            car.speed = car.minSpeed;
        }
        pausePanel.SetActive(false); //hide the pause panel when the resume button is pressed
        Time.timeScale = 1;
    }

    public void CloseStart()
    {
        car.speed = car.minSpeed;
        startPanel.SetActive(false); //hide the start panel when the start button is pressed
        Time.timeScale = 1;
    }

    public void QuitToMenu()
    {
        GameManager.gameManager.GameEnd(); //quit button returns to menu
    }
}
