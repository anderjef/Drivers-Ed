using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO; //read/write to file
using System; //exceptions

public class UIManager : MonoBehaviour
{
    public Image[] hitPoints; //three hearts are displayed in the UI representing player lives
    public Text moneyTxt;
    public GameObject gameOverPanel, pausePanel, startPanel; //one overlay for when the player dies, one for when the game is paused, and one for initially setting difficulty
    public GameObject MovementInstruction, ObjectiveInstruction; //one overlay for movement instructions, one for objective information for tutorial mode
    private Car car;
    public Slider startDifficulty, difficulty; //used to modify the car's speed (and hence the game's difficulty)
    public Slider startSensitivity, sensitivity;
    public Slider carModel; //used to pick the car model

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

        try { //load the previous settings
            string inputText = System.IO.File.ReadAllText(@"Assets/Text_Document.txt");
            if (inputText.Substring(0, 12) == "High score: ")
            {
                int index = 12; //get index past "High score: "
                for (; inputText[index] != '\n'; ++index) {} //ignore the high score value
                index++; //get past the newline character
                if (inputText.Substring(index, 12) == "Difficulty: ")
                {
                    index += 12; //get index past "Difficulty: "
                    car.minSpeed = 0;
                    for (; inputText[index] != '\n'; ++index)
                    {
                        if (inputText[index] != '.')
                        {
                            car.minSpeed *= 10; //because stored in decimal
                            car.minSpeed += inputText[index] - 48; //- 48 because 0 in ASCII is 48
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (inputText[index] == '.') //previous loop was broken out of
                    {
                        index++; //get past the decimal character
                        int mantissaDepth = 0;
                        for (; inputText[index] != '\n'; ++index)
                        {
                            mantissaDepth++;
                            car.minSpeed += (inputText[index] - 48) / (10 * mantissaDepth); //- 48 because 0 in ASCII is 48
                        }
                    }
                    startDifficulty.value = (car.minSpeed - 10) / 10; //set the starting slider (the slider in the pause menu is updated as that menu is opened)
                    index++; //get past the newline character
                    if (inputText.Substring(index, 11) == "Car model: ")
                    {
                        index += 11; //get index past "Car model: "
                        int carChoice = 0;
                        for (; inputText[index] != '\n'; ++index) //note that the text document of settings must end in a newline character
                        {
                            carChoice *= 10; //because stored in decimal
                            carChoice += inputText[index] - 48; //- 48 because 0 in ASCII is 48
                        }
                        carModel.value = carChoice; //set the slider
                        car.updateCarModel(carChoice); //set the car's model
                        if (index != inputText.Length - 1) //there's extra information in the file which shouldn't affect anything
                        {
                            Debug.Log("There's extra information in Text_Document.txt.");
                        }
                    }
                }
            }
        }
        catch (Exception e) //the file cannot be read or there's something wrong with inputText so use the current settings
        {
            Debug.Log(e.Message);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updateStartSensitivity()
    {
        car.sensitivity = startSensitivity.value * 10 + 10;
    }

    public void updateSensitivity()
    {
        car.sensitivity = sensitivity.value * 10 + 10;
    }

    public void pickModel()
    {
        car.updateCarModel((int)carModel.value);
    }

    public void updateDifficulty() //difficulty sliders change the car's minimum speed from 10-20
    {
        car.minSpeed = difficulty.value * 10 + 10;
    }

    public void startUpdateDifficulty() //difficulty sliders change the car's minimum speed from 10-20
    {
        car.minSpeed = startDifficulty.value * 10 + 10;
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
        difficulty.value = (car.minSpeed - 10) / 10;
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
        WriteHighScoreToTextFile(car.getMoney()); //so it is possible to achieve a high score then quit that game but have the high score saved
        GameManager.gameManager.GameEnd(); //quit button returns to menu
    }

    public void WriteHighScoreToTextFile(int score)
    {
        try
        {
            string inputText = System.IO.File.ReadAllText(@"Assets/Text_Document.txt");
            if (inputText.Substring(0, 12) == "High score: ")
            {
                int highScore = 0;
                for (int i = 12; inputText[i] != '\n'; ++i)
                {
                    highScore *= 10; //because stored in decimal
                    highScore += inputText[i] - 48; //- 48 because 0 in ASCII is 48
                }
                if (score < highScore)
                {
                    System.IO.File.WriteAllText(@"Assets/Text_Document.txt", "High score: " + highScore + "\nDifficulty: " + car.minSpeed + "\nCar model: " + carModel.value + "\n");
                    return;
                }
            }
        }
        catch (Exception e) //the file cannot be read or there's something wrong with inputText so whatever the settings are, they are written to the file
        {
            Debug.Log(e.Message);
        }
        System.IO.File.WriteAllText(@"Assets/Text_Document.txt", "High score: " + score + "\nDifficulty: " + car.minSpeed + "\nCar model: " + carModel.value + "\n");
    }
}
