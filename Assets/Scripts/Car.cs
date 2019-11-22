using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private double controls = 0;
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
        controls += Time.deltaTime;
        if((SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Tutorial")) && (controls > 5)) //checking for tutorial mode is first because with the double ampersand operator, if the first condition already gives the answer to the full condition, the computer will know not to check the second condition, and in this case it's more likely that it will not be tutorial mode than controls < 5
        {
            ControlReminder.SetActive(false);
            SpeedReminder.SetActive(false);
        }
        Vector3 carMove = new Vector3(Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime, 0, speed * Time.deltaTime); //z is forward
        if ((this.transform.localPosition.x < -5 && carMove.x < 0) || (this.transform.localPosition.x > 5 && carMove.x > 0)) //setting left and right boundaries
        {
            carMove.x = 0;
            if (this.transform.localPosition.x < -5)
            {
                //FIXME: get the car back on the road
            }
            else if (this.transform.localPosition.x > 5)
            {
                //FIXME: get the car back on the road
            }
        }
        if (Input.GetButton("Fire1")) //ctrl
        {
            if (speed > minSpeed)
            {
                speed = speed - 1f;
            }
        }
        if (Input.GetButton("Fire2")) //Alt
        {
            if(speed < maxSpeed)
            {
                speed = speed + 1f;
            }
        }
        controller.Move(carMove);
    }

    private void OnTriggerEnter(Collider other)
    {
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
            CollideReminder.SetActive(true);
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
    }

    IEnumerator Collided (double time) //collision detection
    {
        collided = true;
        double timer = 0;
        float currentCollision = 1f;
        float lastCollision = 0;
        float collisionPeriod = 0.1f;
        bool enabled = false;

        yield return new WaitForSeconds(1f); //pause for a second
        speed = minSpeed; //when the car resumes motion, its speed is returned to the minimum/starting speed
        while (timer < time && collided)
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

    public void GoBackToMenu()
    {
        GameManager.gameManager.GameEnd(); //return to main menu once all lives are lost
    }
}
