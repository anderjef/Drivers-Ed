using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEditor; //assetdatabase //assetdatabase cannot be built

public class Car : MonoBehaviour
{
    private CharacterController controller;
    public float speed, minSpeed = 10f, maxSpeed = 30f; //can be modified within Unity
    public float collisionTime = 0; //can be modified within Unity
    public GameObject model, CoinReminder, CollideReminder, ControlReminder, SpeedReminder;
    public int currentLife = 3; //total number of lives; can be modified within Unity, but only the last three lives will be shown in the upper left corner
    private bool collided = false;
    static int collidedValue;
    private UIManager uiManager;
    private double controlsTimer = 0, collideReminderTimer = 0;
    private int money;
    AudioSource tickSource;
    AudioSource carRev;
    private AudioClip audio1, audio2, audio3;
    [HideInInspector] public float sensitivity = 1;

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
        AudioSource[] ticksources = GetComponents<AudioSource>();
        tickSource = ticksources[0];
        audio1 = ticksources[0].clip;
        audio2 = ticksources[1].clip;
        audio3 = ticksources[2].clip;
        carRev = ticksources[3];
        this.updateCarModel((int)uiManager.carModel.value);
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

        if (uiManager.gameOverPanel.activeSelf) //don't update location (move car) or speed during the game over panel
        {
            return;
        }

        Vector3 carMove = new Vector3(Input.GetAxisRaw("Horizontal") * sensitivity * maxSpeed * Time.deltaTime / 3, 0.7f - this.transform.localPosition.y, speed * Time.deltaTime); //y is kept at 0.7, z is forward
        if (this.transform.localPosition.x + carMove.x > 5) //setting left and right boundaries
        {
            carMove.x = 5 - this.transform.localPosition.x;
        }
        else if (this.transform.localPosition.x + carMove.x < -5)
        {
            carMove.x = -5 - this.transform.localPosition.x;
        }
        controller.Move(carMove);

        if (Input.GetButton("Fire1") && speed > minSpeed) //ctrl
        {
            speed--;
        }
        if (Input.GetButton("Fire2") && speed < maxSpeed) //Alt; not else if because if both ctrl and alt are pressed, it is assumed the user wants to leave speed as is
        {
            speed++;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Money"))
        {
            if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Tutorial")) //tutorial mode doesn't have a coin reminder
            {
                CoinReminder.SetActive(false);
            }
            tickSource.PlayOneShot(audio1, 0.2f);
            money++;
            uiManager.UpdateMoney(money);
            if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Experiment")) //FIXME: temporary while experiment is separate
            {
                //other.transform.parent.gameObject.SetActive(false); //if the money prefab had a parent object (money holder)
                other.transform.gameObject.SetActive(false);
            }
        }
    }

    private void OnCollisionEnter(Collision collisionInfo) //collision detection
    {
        if (collided) //while collided, don't register another collision; grant the player temporary invincibility so they can register that they lost a life
        {
            return;
        }
        if (collisionInfo.collider.CompareTag("Obstacle"))
        {
            tickSource.PlayOneShot(audio2, 0.6f);
            if (collideReminderTimer == 0) //only display the collider reminder once; collideReminderTimer is updated in the Update function
            {
                CollideReminder.SetActive(true);
            }
            else if (CollideReminder.activeSelf && SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Tutorial")) //a second collision resets the countdown in tutorial mode
            {
                collideReminderTimer = 0;
            }
            currentLife--;
            uiManager.UpdateLives(currentLife); //display new life count
            if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Sandbox")) //sandbox doesn't even slow down for collisions, let alone lose the game
            {
                if (currentLife <= 0)
                {
                    if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Game"))
                    {
                        uiManager.SaveSettingsAndHighScoreToTextFile(money); //score and settings are saved upon death
                    }
                    carRev.mute = true;
                    tickSource.PlayOneShot(audio3, 2f);
                    uiManager.gameOverPanel.SetActive(true);
                    CoinReminder.SetActive(false);
                    CollideReminder.SetActive(false);
                    ControlReminder.SetActive(false);
                    SpeedReminder.SetActive(false);
                    Invoke("GoBackToMenu", 3f); //upon losing all lives, display GameOverPanel for 3 seconds before returning to menu
                }
                else
                {
                    speed = minSpeed; //reset speed upon a collision
                    StartCoroutine(Collided());
                }
            }
            else //in sandbox mode
            {
                //speed = minSpeed; //could set speed back to the lower bound
                speed -= 3; //could decrement the speed by a constant (arbitrary) amount
                //speed -= 0.1f * (speed - minSpeed); //could decrement the speed by some function
                if (speed < minSpeed)
                {
                    speed = minSpeed;
                }
                //could leave the speed alone
            }
        }
    }

    IEnumerator Collided () //collision response
    {
        double time = this.collisionTime;
        collided = true;
        double timer = 0;
        float lastCollision = 0;
        float collisionPeriod = 0.1f;
        bool enabled = false;

        while (timer < time && collided) //flash the car to show the player that they have lost a life and (temporarily) have invincibility
        {
            model.SetActive(enabled);
            yield return null;
            timer += Time.deltaTime;
            lastCollision += Time.deltaTime;
            if (collisionPeriod < lastCollision)
            {
                lastCollision = 0;
                enabled = !enabled;
            }
        }
        model.SetActive(true);
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
        model = this.transform.Find("Car" + carChoice.ToString()).gameObject;
        //this.GetComponent<MeshCollider>().sharedMesh = (Mesh)AssetDatabase.LoadAssetAtPath("Assets/Racing Cars Pack 1/FBXs/Car" + carChoice.ToString() + ".fbx", typeof(Mesh)); //get the mesh (for the mesh collider) corresponding to the chosen car //assetdatabase cannot be built
    }

    public int getMoney()
    {
        return this.money;
    }

    public void GoBackToMenu()
    {
        GameManager.gameManager.GameEnd(); //return to main menu once all lives are lost
    }
}
