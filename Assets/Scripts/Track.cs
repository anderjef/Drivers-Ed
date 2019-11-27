using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Track : MonoBehaviour
{
    public GameObject[] obstacles; //there are nine obstacle (barrel) prefabs hence an array
    public GameObject money; //there is only one money prefab
    public Vector2 numObstacles, amountOfMoney; //can be modified within Unity; note that a numObstacles.y > trackLength / diameter of barrel (which is approximately 0.8f) creates the chance for barrels to be too close to each other (such that they merge)
    public int tutorialNum = 0;
    private UIManager uiManager;
    private Car car; //used to set the car's speed at game over in tutorial mode

    [HideInInspector]
    public List<GameObject> nObstacles, newMoney;

    private const float trackLength = 80f; //to be updated as required

    // Start is called before the first frame update
    void Start()
    {
        car = FindObjectOfType<Car>();
        uiManager = FindObjectOfType<UIManager>();
        uiManager.ObjectiveInstruction.SetActive(false); //(tutorial mode) starts off with no objective instructions

        for (int i = 0; i < (int)Random.Range(numObstacles.x, numObstacles.y); ++i) //initialize the array of obstacles objects
        {
            nObstacles.Add(Instantiate(obstacles[Random.Range(0, obstacles.Length)], transform));
            nObstacles[i].SetActive(false);
        }

        for (int i = 0; i < (int)Random.Range(amountOfMoney.x, amountOfMoney.y); ++i) //initialize the array of money objects
        {
            newMoney.Add(Instantiate(money, transform));
            newMoney[i].SetActive(false);
        }

        //LayoutObstacles(); //not laying out the obstacles here leaves the first two tracks devoid of obstacles so the user can react to the game starting
        //LayoutMoney(); //not laying out the money here leaves the first two tracks devoid of money so the user can react to the game starting
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LayoutObstacles() //spawn obstacles
    {
        for (int i = 0; i < nObstacles.Count; ++i)
        {
            nObstacles[i].transform.eulerAngles = new Vector3(270, 0, 0); //set the barrel standing up
            nObstacles[i].GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0); //set the barrel's velocity to zero
            nObstacles[i].transform.localPosition = new Vector3(Random.Range(-5.75f, 5.75f), 0.05f, Random.Range(i * (trackLength / nObstacles.Count) + 0.4f, (i + 1) * (trackLength / nObstacles.Count) - 0.4f)); //x position is random on the road, y position is ground level, z position is within the length of a piece of track (0.4f is the approximate radius of a barrel)
            nObstacles[i].SetActive(true);
        }
    }

    void LayoutMoney()  //spawn money //FIXME: money spawns too far off track and don't have money collide with barrels if that's not too much to ask
    {
        float minZP = 10f, randomZP;
        for (int i = 0; i < newMoney.Count; ++i)
        {
            randomZP = Random.Range(minZP, minZP + 5f);
            if (randomZP > trackLength) //FIXME: temporary fix of money spawning outside of track's range
            {
                break;
            }
            newMoney[i].transform.localPosition = new Vector3(Random.Range(-5.75f, 5.75f), 1, randomZP); //x position is random on the road, y position is just above ground level, z position is within the length of a piece of track
            newMoney[i].SetActive(true);
            minZP = randomZP + 3;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car")) //there is a box collider at the end of each section of track, that when it detects a collision with the car, it moves the section of track just completed to after the other piece of track and reinitializes the obstacles and money on it
        {
            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Tutorial"))
            {
                tutorialNum++;
                if (tutorialNum == 1) //after the car reaches the end of the first piece of track
                {
                    uiManager.MovementInstruction.SetActive(false);
                    uiManager.ObjectiveInstruction.SetActive(true);
                }
                else if (tutorialNum == 3) //the third time the car reaches the end of the first track
                {
                    uiManager.gameOverPanel.SetActive(true);
                    //Time.timeScale = 0 doesn't work here because Invoke needs to count down
                    car.minSpeed = 0; //so car won't resume motion
                    car.maxSpeed = 0; //so car won't resume motion
                    car.speed = 0; //stop car
                    Invoke("BackToMenu", 3f); //upon finishing the tutorial, display GameOverPanel for 3 seconds before returning to menu
                }
            }
            other.GetComponent<Car>().IncreaseSpeed(); //the car speeds up after reaching the end of a piece of track
            transform.position = new Vector3(0, 0, transform.position.z + trackLength * 2); //move the track section to the end after the player has traversed it so they can keep driving forever
            LayoutObstacles(); //re-randomize the location of the barrel obstacles
            LayoutMoney(); //re-randomize the location of the coins
        }
    }
}
