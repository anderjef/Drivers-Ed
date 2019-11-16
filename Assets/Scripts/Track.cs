using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
    public GameObject[] obstacles;
    public Vector2 numObstacles;
    
    [HideInInspector]
    public List<GameObject> nObstacles;

    // Start is called before the first frame update
    void Start()
    {
        int newNumObstacles = (int)Random.Range(numObstacles.x, numObstacles.y);
        for (int i = 0; i < newNumObstacles; ++i)
        {
            nObstacles.Add(Instantiate(obstacles[Random.Range(0, obstacles.Length)], transform));
            nObstacles[i].SetActive(false);
        }
        LayoutObstacles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LayoutObstacles()
    {
        for (int i = 1; i < nObstacles.Count + 1; ++i)
        {
            float pZMin = i * (0f / nObstacles.Count); //FIXME: replace the zero (in 0f) with Z position of track 2
            float pZMax = i * (0f / nObstacles.Count) + 1; //FIXME: replace the zero (in 0f) with Z position of track 2
            nObstacles[i].transform.localPosition = new Vector3(0, 0, Random.Range(pZMin, pZMax));
            nObstacles[i].SetActive(true);
        }
    }
}
