using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image[] hitPoints;
    public Text moneyTxt;
    public GameObject gameOverPanel, pausePanel;

    // Start is called before the first frame update
    void Start()
    {
        pausePanel.SetActive(false);
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
                hitPoints[i].color = Color.white;
            }
            else
            {
                hitPoints[i].color = Color.black;
            }
        }
    }

    public void UpdateMoney(int money)
    {
        moneyTxt.text = money.ToString();
    }

    public void OpenPause()
    {
        pausePanel.SetActive(true);
        //FIXME: save speed and freeze game
    }

    public void ClosePause()
    {
        pausePanel.SetActive(false);
        //FIXME: resume game
    }
}
