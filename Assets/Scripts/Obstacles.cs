using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacles : MonoBehaviour
{
    public GameObject[] barrelPrefabs; //there are nine obstacle (barrel) prefabs hence an array
    public GameObject moneyPrefab; //there is only one money prefab hence no array
    public float distBetweenObjects = 2.8f; //can be modified within Unity; note that the diameter of a barrel is approximately 2.8 so values less than that may lead to barrels and coins spawning inside each other
    public int numObstacles = 40, numMoney = 60; //can be modified within Unity
    private Car car;
    public float dist = 80; //initialized to the distance to the first obstacle; can be modified within Unity; note that the limitations of the size of a float variable does mean the game is not truly endless (as eventually the variable will overflow)

    [HideInInspector]
    public List<GameObject> obstacles, money;
    
    private const float trackLength = 80f; //to be updated as required

    // Start is called before the first frame update
    void Start()
    {
        car = FindObjectOfType<Car>();
        for (int i = 0; i < numObstacles; ++i) //initialize the array of obstacles objects
        {
            obstacles.Add(Instantiate(barrelPrefabs[Random.Range(0, barrelPrefabs.Length)], transform));
            obstacles[i].transform.localPosition = new Vector3(Random.Range(-5.75f, 5.75f), 0.05f, dist); //x position is random on the road, y position is ground level, z position is at the back
            obstacles[i].SetActive(true);
            dist += distBetweenObjects; //the next object to spawn will do so slightly further back
        }

        for (int i = 0; i < numMoney; ++i) //initialize the array of money objects
        {
            money.Add(Instantiate(moneyPrefab, transform));
        }
    }

    // Update is called once per frame
    void Update()
    {
        //FIXME: run in background (don't update every frame)
        //FIXME: allow for multiple barrels to spawn in the same x line
        //FIXME: optimize by not searching the entire array every frame
        for (int i = 0; i < obstacles.Count; ++i)
        {
            if (obstacles[i].transform.localPosition.y < -5 || obstacles[i].transform.localPosition.z < car.transform.localPosition.z - 5)
            {
                obstacles[i].SetActive(false);
                obstacles[i] = Instantiate(barrelPrefabs[Random.Range(0, barrelPrefabs.Length)], transform); //re-randomize the barrel's prefab model
                obstacles[i].transform.eulerAngles = new Vector3(270, 0, 0); //set the barrel standing upright
                obstacles[i].GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0); //set the barrel's velocity to zero
                obstacles[i].transform.localPosition = new Vector3(Random.Range(-5.75f, 5.75f), 0.05f, dist); //x position is random on the road, y position is ground level, z position is at the back
                obstacles[i].SetActive(true);
                dist += distBetweenObjects; //the next object to spawn will do so slightly further back
            }
            if (obstacles[i].transform.localPosition.z > car.transform.localPosition.z + trackLength)
            {
                obstacles[i].SetActive(false); //otherwise the barrel will fall through the floor as it may not be generated there yet
            }
            else
            {
                obstacles[i].SetActive(true);
            }
        }
        float minZP = 10f, randomZP;
        for (int i = 0; i < money.Count; ++i)
        {
            /*if (money[i].transform.localPosition.z < car.transform.localPosition.z - 5)
            {
                
            }
            randomZP = Random.Range(minZP, minZP + 5f);
            //if (randomZP > trackLength) //FIXME: temporary fix of money spawning outside of track's range
            //{
                //break;
            //}
            money[i].transform.localPosition = new Vector3(Random.Range(-5.75f, 5.75f), 1, randomZP); //x position is random on the road, y position is just above ground level, z position is within the length of a piece of track
            money[i].SetActive(true);
            minZP = randomZP + 3;
            //if (money[i].transform.localPosition.z < car.transform.localPosition.z - 5)
            //{
                //FIXME
            //}*/
        }
    }

    void LayoutObstacles() //spawn obstacles
    {
        for (int i = 0; i < obstacles.Count; ++i)
        {
            obstacles[i].transform.eulerAngles = new Vector3(270, 0, 0); //set the barrel standing up
            obstacles[i].GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0); //set the barrel's velocity to zero
            //obstacles[i].transform.localPosition = new Vector3(Random.Range(-5.75f, 5.75f), 0.05f, Random.Range(i * (trackLength / obstacles.Count) + 0.4f, (i + 1) * (trackLength / obstacles.Count) - 0.4f)); //x position is random on the road, y position is ground level, z position is within the length of a piece of track (0.4f is the approximate radius of a barrel)
            obstacles[i].SetActive(true);
        }
    }

    void LayoutMoney()  //spawn money //FIXME: money spawns too far off track and don't have money collide with barrels if that's not too much to ask
    {
        float minZP = 10f, randomZP;
        for (int i = 0; i < money.Count; ++i)
        {
            randomZP = Random.Range(minZP, minZP + 5f);
            //if (randomZP > trackLength) //FIXME: temporary fix of money spawning outside of track's range
            //{
            //    break;
            //}
            money[i].transform.localPosition = new Vector3(Random.Range(-5.75f, 5.75f), 1, randomZP); //x position is random on the road, y position is just above ground level, z position is within the length of a piece of track
            money[i].SetActive(true);
            minZP = randomZP + 3;
        }
    }
}
