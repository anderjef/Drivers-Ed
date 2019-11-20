using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Track : MonoBehaviour
{
    public GameObject[] obstacles; //there are five obstacle (barrel) prefabs hence an array
    public Vector2 numObstacles; //can be modified within Unity
    public GameObject money; //there is only one money prefab
    public Vector2 amountOfMoney; //can be modified within Unity
    public int tutorialNum = 0;
    private UIManager uiManager;

    [HideInInspector]
    public List<GameObject> newMoney;

    [HideInInspector]
    public List<GameObject> nObstacles;

    // Start is called before the first frame update
    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        int newNumMoney = (int)Random.Range(amountOfMoney.x, amountOfMoney.y);
        int newNumObstacles = (int)Random.Range(numObstacles.x, numObstacles.y);

        for (int i = 0; i < newNumObstacles; ++i) //initialize the array of obstacles objects
        {
            nObstacles.Add(Instantiate(obstacles[Random.Range(0, obstacles.Length)], transform));
            nObstacles[i].SetActive(false);
        }
        
        for (int i = 0; i < newNumMoney; ++i) //initialize the array of money objects
        {
            newMoney.Add(Instantiate(money, transform));
            newMoney[i].SetActive(false);
        }

        LayoutObstacles(); //spawn the obstacles
        LayoutMoney(); //spawn money
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LayoutObstacles()
    {
        for (int i = 1; i < nObstacles.Count + 1; ++i)
        {
            float pZMin = i * (80f / nObstacles.Count) + 5f; //80f is the Z position of track 2
            float pZMax = i * (80f / nObstacles.Count) + 6f; //80f is the Z position of track 2
            nObstacles[i - 1].transform.localPosition = new Vector3(Random.Range(-5, 6), 0, Random.Range(pZMin, pZMax)); //x position is random on the road, y position is ground level, z position is within the limits of the piece of track
            nObstacles[i - 1].SetActive(true);
        }
    }

    void LayoutMoney()
    {
        float minZP = 10f;
        for (int i = 0; i < newMoney.Count; ++i)
        {
            float maxZP = minZP + 5f;
            float randomZP = Random.Range(minZP, maxZP);
            newMoney[i].transform.localPosition = new Vector3(Random.Range(-5, 6), 1, randomZP); //x position is random on the road, y position is just above ground level, z position is within the limits of the piece of track
            newMoney[i].SetActive(true);
            minZP = randomZP + 1;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car")) //there is a box collider at the end of each section of track, that when it detects a collision with the car, it moves the section of track just completed to after the other piece of track and reinitializes the obstacles and money on it
        {
            tutorialNum++;
            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Tutorial"))
            {

                if (tutorialNum == 1)
                {
                    uiManager.MovementInstruction.SetActive(false);
                    uiManager.ObjectiveInstruction.SetActive(true);
                }
                else if (tutorialNum == 3)
                {
                    uiManager.ObjectiveInstruction.SetActive(false);
                    uiManager.gameOverPanel.SetActive(true);
                    Invoke("GoBackToMenu", 3f);
                }
            }
            other.GetComponent<Car>().IncreaseSpeed(); //the car speeds up after reaching the end of a piece of track
            transform.position = new Vector3(0, 0, transform.position.z + 80 * 2); //80 is the Z position of track 2; move the track section to the end so the player can keep driving forever
            LayoutObstacles(); //re-randomize the location of the barrel obstacles
            LayoutMoney(); //re-randomize the location of the coins
        }
    }
}
