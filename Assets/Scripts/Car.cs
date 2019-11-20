using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    private CharacterController controller;
    public float speed, minSpeed = 10f, maxSpeed = 30f; //can be modified within Unity
    public float collisionTime;
    public GameObject model, CoinReminder, CollideReminder, ControlReminder;
    private int currentLife = 3;
    private bool collided = false, firstcoin = true, showcont = true;
    static int collidedValue;
    private UIManager uiManager;
    private double controls = 0.0;
    private int money;
    private double x = 0; //used to know where the car is currently (laterally) so as to prohibit it from running out of bounds

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        speed = minSpeed; //car starts off slow
        collidedValue = Shader.PropertyToID("_CollidedValue");
        uiManager = FindObjectOfType<UIManager>();
        CoinReminder.SetActive(true);
        ControlReminder.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        controls += Time.deltaTime;
        if((controls > 5.0)&&(showcont))
        {
            ControlReminder.SetActive(false);
            showcont = false;
        }
        Vector3 carMove = new Vector3(Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime, 0, speed * Time.deltaTime); //z is forward
        if ((x < -5 && carMove.x < 0) || (x > 5 && carMove.x > 0)) //setting left and right boundaries
        {
            carMove.x = 0;
        }
        if (Input.GetButton("Fire1")) //ctrl
        {
            if (speed >= minSpeed)
            {
                speed = speed - 1f;
            }
        }
        if (Input.GetButton("Fire2")) //Alt
        {
            if(speed <= maxSpeed)
            {
                speed = speed + 1f;
            }
        }
        x += carMove.x;
        controller.Move(carMove);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Money"))
        {
            if (firstcoin)
            {
                CoinReminder.SetActive(false);
                firstcoin = false;
            }
            money++;
            uiManager.UpdateMoney(money);
            //other.transform.parent.gameObject.SetActive(false);
            other.transform.gameObject.SetActive(false);
        }
        if (collided)
        {
            return;
        }
        if (other.CompareTag("Obstacle")) //not else if because it is possible to collide with both a coin and barrel at the same time
        {
            CollideReminder.SetActive(true);
            currentLife--;
            uiManager.UpdateLives(currentLife); //display new life count
            speed = 0; //briefly (or permanently) freeze the player so they can recognize they lost a life
            if (currentLife <= 0)
            {
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
        speed += 1.15f; //the *= operator would cause difficulty to be exponential instead of linear
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
