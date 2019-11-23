using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor; //assetdatabase

public class Car : MonoBehaviour
{
    private CharacterController controller;
    public float speed, minSpeed = 10f, maxSpeed = 30f; //can be modified within Unity
    public float collisionTime;
    public GameObject model, CoinReminder, CollideReminder, ControlReminder, SpeedReminder;
    public int currentLife = 3; //maximum number of lives; can be modified within Unity, but only the last three lives will be shown in the upper left corner
    private bool collided = false, firstcoin = true;
    static int collidedValue;
    private UIManager uiManager;
    private double controlsTimer = 0, collideReminderTimer = 0;
    private int money;
    AudioSource tickSource;
    AudioSource carRev;
    public AudioClip audio1, audio2, audio3;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        speed = minSpeed; //car starts off slow
        collidedValue = Shader.PropertyToID("_CollidedValue");
        uiManager = FindObjectOfType<UIManager>();
        ControlReminder.SetActive(true);
        SpeedReminder.SetActive(true);
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Tutorial")) //tutorial mode doesn't have a coin reminder
        {
            CoinReminder.SetActive(true);
        }
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Sandbox")) //sandbox uses the currentLife as a barrels counter
        {
            currentLife = 0;
        }
        AudioSource[] ticksources = GetComponents<AudioSource>();
        tickSource = ticksources[0];
        audio1 = ticksources[0].clip;
        audio2 = ticksources[1].clip;
        audio3 = ticksources[2].clip;
        carRev = ticksources[3];
    }

    // Update is called once per frame
    void Update()
    {
        if (CollideReminder.activeSelf) //only show collide reminder for five seconds
        {
            collideReminderTimer += Time.deltaTime;
            if (collideReminderTimer > 5)
            {
                CollideReminder.SetActive(false);
                if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Tutorial")) //in tutorial mode, the collision reminder is shown after every collision (which is achieved by resetting the timer)
                {
                    collideReminderTimer = 0;
                }
            }
        }
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Tutorial")) //in tutorial mode, control and speed reminders are shown permanently
        {
            controlsTimer += Time.deltaTime;
            if (controlsTimer > 5)
            {
                ControlReminder.SetActive(false);
                SpeedReminder.SetActive(false);
            }
        }

        if (collided || uiManager.gameOverPanel.activeSelf) //don't update location (move car) or speed while collided or during the game over panel
        {
            return;
        }

        Vector3 carMove = new Vector3(Input.GetAxisRaw("Horizontal") * minSpeed * Time.deltaTime, 0, speed * Time.deltaTime); //z is forward
        if (this.transform.localPosition.x + carMove.x > 5) //setting left and right boundaries
        {
            carMove.x = 5 - this.transform.localPosition.x;
        }
        else if (this.transform.localPosition.x + carMove.x < -5)
        {
            carMove.x = -5 - this.transform.localPosition.x;
        }
        if (this.transform.localPosition.y != 0.7)
        {
            carMove.y = 0.7f - this.transform.localPosition.y;
        }
        controller.Move(carMove);

        if (Input.GetButton("Fire1") && speed > minSpeed) //ctrl
        {
            speed = speed - 1f;
        }
        if (Input.GetButton("Fire2") && speed < maxSpeed) //Alt; not else if because if both ctrl and alt are pressed, it is assumed the user wants to leave speed as is
        {
            speed = speed + 1f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Money")
        {
            if (firstcoin && SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Tutorial")) //tutorial mode doesn't have a coin reminder
            {
                CoinReminder.SetActive(false);
                firstcoin = false;
            }
            tickSource.PlayOneShot(audio1, 0.2f);
            money++;
            uiManager.UpdateMoney(money);
            //other.transform.parent.gameObject.SetActive(false); //if the money prefab had a parent object (money holder)
            other.transform.gameObject.SetActive(false);

            {
                /*
                if (other.CompareTag("Money"))
                {
                    if (firstcoin && SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Tutorial")) //tutorial mode doesn't have a coin reminder
                    {
                        CoinReminder.SetActive(false);
                        firstcoin = false;
                    }
                    tickSource.PlayOneShot(audio1, 0.2f);
                    money++;
                    uiManager.UpdateMoney(money);
                    //other.transform.parent.gameObject.SetActive(false); //if the money prefab had a parent object (money holder)
                    other.transform.gameObject.SetActive(false);
                }
                if (collided)
                {
                    return;
                }
                if (other.CompareTag("Obstacle")) //not else if because it is possible to collide with both a coin and barrel at the same time
                {
                    tickSource.PlayOneShot(audio2, 0.6f);
                    if (collideReminderTimer == 0) //only display the collider reminder once
                    {
                        CollideReminder.SetActive(true);
                    }
                    currentLife--;
                    uiManager.UpdateLives(currentLife); //display new life count
                    speed = 0; //briefly (or permanently) freeze the player so they can register they lost a life
                    if (currentLife <= 0 && SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Sandbox")) //can't lose in sandbox mode
                    {
                        carRev.mute = true;
                        tickSource.PlayOneShot(audio3, 2f);
                        uiManager.gameOverPanel.SetActive(true);
                        Invoke("GoBackToMenu", 3f); //upon losing all lives, display GameOverPanel for 3 seconds before returning to menu
                    }
                    else
                    {
                        StartCoroutine(Collided(collisionTime));
                    }
                }
                */
            }
        }
    }

    private void OnCollisionEnter(Collision collisionInfo) //collision detection
    {
        if (collided)
        {
            return;
        }
        if (collisionInfo.collider.tag == "Obstacle")
        {
            tickSource.PlayOneShot(audio2, 0.6f);
            if (collideReminderTimer == 0) //only display the collider reminder once
            {
                CollideReminder.SetActive(true);
            }
            currentLife--;
            uiManager.UpdateLives(currentLife); //display new life count
            if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Sandbox")) //sandbox doesn't even slow down for collisions, let alone lose the game
            {
                speed = 0; //briefly (or permanently) freeze the player so they can register they lost a life
                if (currentLife <= 0)
                {
                    carRev.mute = true;
                    tickSource.PlayOneShot(audio3, 2f);
                    uiManager.gameOverPanel.SetActive(true);
                    Invoke("GoBackToMenu", 3f); //upon losing all lives, display GameOverPanel for 3 seconds before returning to menu
                }
                else
                {
                    StartCoroutine(Collided(collisionTime));
                }
            }
            else //in sandbox mode
            {
                //speed = minSpeed; //could set speed back to the lower bound
                speed -= 5; //could decrement the speed by a constant (arbitrary) amount
                //could leave the speed alone
            }
        }
    }

    IEnumerator Collided (double time) //collision detection
    {
        collided = true;
        //double timer = 0;
        //float currentCollision = 1f;
        //float lastCollision = 0;
        //float collisionPeriod = 0.1f;
        //bool enabled = false;

        yield return new WaitForSeconds(1f); //pause for a second
        speed = minSpeed; //when the car resumes motion, its speed is returned to the minimum/starting speed
        /*while (timer < time && collided)
        {
            model.SetActive(enabled);
            yield return null;
            timer += Time.deltaTime;
            lastCollision += Time.deltaTime;
            if (collisionPeriod < lastCollision)
            {
                lastCollision = 0;
                currentCollision = 1f - currentCollision;
                enabled = !enabled;
            }
        }
        model.SetActive(true);*/
        collided = false;
    }

    public void IncreaseSpeed() //called every 80 or so units whenever the car reaches the end of a section of track
    {
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Game"))
        {
            speed += 1.15f; //tutorial and sandbox difficulties are linear
        }
        else
        {
            speed *= 1.15f; //full game difficulty is exponential
        }
        if (speed > maxSpeed) //enforce speed limit
        {
            speed = maxSpeed;
        }
    }
    
    public void updateCarModel(int carChoice)
    {
        for (int i = 1; i < 9; ++i) //ensures only one car model is active at a time
        {
            this.transform.Find("Car" + i.ToString()).gameObject.SetActive(false);
        }
        this.transform.Find("Car" + carChoice.ToString()).gameObject.SetActive(true);
        this.GetComponent<MeshCollider>().sharedMesh = (Mesh)AssetDatabase.LoadAssetAtPath("Assets/Racing Cars Pack 1/FBXs/Car" + carChoice.ToString() + ".fbx", typeof(Mesh)); //get the mesh (for the mesh collider) corresponding to the chosen car
    }

    public void GoBackToMenu()
    {
        GameManager.gameManager.GameEnd(); //return to main menu once all lives are lost
    }
}
