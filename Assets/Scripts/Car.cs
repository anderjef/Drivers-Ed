using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 carMove;
    public float speed;
    public int maxLife = 3;
    public float minSpeed = 10f;
    public float maxSpeed = 30f;
    public float collisionTime;
    public GameObject model;
    private int currentLife;
    private bool collided = false;
    static int collidedValue;
    private UIManager uiManager;
    private int money;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentLife = maxLife;
        speed = minSpeed;
        collidedValue = Shader.PropertyToID("_CollidedValue");
        uiManager = FindObjectOfType<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        carMove = Vector3.zero;
        carMove.x = Input.GetAxisRaw("Horizontal") * speed;
        carMove.z = speed;
        controller.Move(carMove * Time.deltaTime);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Money"))
        {
            money++;
            uiManager.UpdateMoney(money);
            other.transform.parent.gameObject.SetActive(false);
        }
        if (collided)
        {
            return;
        }
        if (other.CompareTag("Obstacle"))
        {
            currentLife--;
            uiManager.UpdateLives(currentLife);
            speed = 0;
            if (currentLife <= 0)
            {
                speed = 0;
                uiManager.gameOverPanel.SetActive(true);
                Invoke("GoBackToMenu", 3f); //upon losing all lives, display GameOverPanel for 3 seconds before returning to menu
            }
            else
            {
                StartCoroutine(Collided(collisionTime));
            }
        }
    }

    IEnumerator Collided (float time)
    {
        collided = true;
        float timer = 0;
        float currentCollision = 1f;
        float lastCollision = 0;
        float collisionPeriod = 0.1f;
        bool enabled = false;

        yield return new WaitForSeconds(1f);
        speed = minSpeed;
        while (timer > time && collided)
        {
            model.SetActive(enabled);
            yield return null;
            timer += Time.deltaTime;
            lastCollision += Time.deltaTime;
            if (collisionPeriod > lastCollision)
            {
                lastCollision = 0;
                currentCollision = 1f - currentCollision;
                enabled = !enabled;
            }
        }
        model.SetActive(true);
        collided = false;
    }

    public void IncreaseSpeed()
    {
        speed *= 1.15f;
        if (speed >= maxSpeed)
        {
            speed = maxSpeed;
        }
    }

    public void GoBackToMenu()
    {
        GameManager.gameManager.GameEnd();
    }
}
