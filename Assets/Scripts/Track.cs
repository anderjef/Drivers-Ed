using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
    public GameObject[] obstacles;
    public Vector2 numObstacles;
    public GameObject money;
    public Vector2 amountOfMoney;

    [HideInInspector]
    public List<GameObject> newMoney;

    [HideInInspector]
    public List<GameObject> nObstacles;

    // Start is called before the first frame update
    void Start()
    {
        int newNumMoney = (int)Random.Range(amountOfMoney.x, amountOfMoney.y);
        int newNumObstacles = (int)Random.Range(numObstacles.x, numObstacles.y);

        for (int i = 0; i < newNumObstacles; ++i)
        {
            nObstacles.Add(Instantiate(obstacles[Random.Range(0, obstacles.Length)], transform));
            nObstacles[i].SetActive(false);
        }

        for (int i = 0; i < newNumMoney; ++i)
        {
            newMoney.Add(Instantiate(money, transform));
            newMoney[i].SetActive(false);
        }

        LayoutObstacles();
        LayoutMoney();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LayoutObstacles()
    {
        for (int i = 1; i < nObstacles.Count + 1; ++i)
        {
            float pZMin = (i + 1) * (80f / nObstacles.Count); //80f is the Z position of track 2
            float pZMax = (i + 1) * (80f / nObstacles.Count); //80f is the Z position of track 2
            nObstacles[i].transform.localPosition = new Vector3(Random.Range(-5, 5), 0, Random.Range(pZMin, pZMax));
            nObstacles[i].SetActive(true);
            //FIXME: in part 7
            //if (nObstacles[i].GetComponent<RandomLane>() != null)
            //{
                //nObstacles[i].GetComponent<RandomLane>().LaneLayout();
            //}
        }
    }
    void LayoutMoney()
    {
        float minZP = 10f;
        for (int i = 0; i < newMoney.Count; ++i)
        {
            float maxZP = minZP + 5f;
            float randomZP = Random.Range(minZP, maxZP);
            newMoney[i].transform.localPosition = new Vector3(transform.position.x, transform.position.y, randomZP);
            newMoney[i].SetActive(true);
            minZP = randomZP + 1;
            //FIXME: in part 7
            //if (newMoney[i].GetComponent<RandomLane>() != null)
            //{
                //newMoney[i].GetComponent<RandomLane>().LaneLayout();
            //}
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            other.GetComponent<Car>().IncreaseSpeed();
            transform.position = new Vector3(0, 0, transform.position.z + 80 * 2); //80 is the Z position of track 2
            LayoutObstacles();
            LayoutMoney();
        }
    }
}
