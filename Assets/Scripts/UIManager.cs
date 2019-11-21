using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image[] hitPoints; //three hearts are displayed in the UI representing player lives
    public Text moneyTxt;
    public GameObject gameOverPanel, pausePanel; //one overlay for when the player dies, and one for when the game is paused
    public bool isPaused;

    // Start is called before the first frame update
    void Start()
    {
        pausePanel.SetActive(false);
        isPaused = false;
    }

    // Update is called once per frame
    void Update()
    {

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
        pausePanel.SetActive(true); //show the pause panel when the pause button is pressed
        Time.timeScale = 0;
        isPaused = true;
        //FIXME: save speed
    }

    public void ClosePause()
    {
        pausePanel.SetActive(false); //hide the pause panel when the resume button is pressed
        Time.timeScale = 1;
        isPaused = false;
    }
}
