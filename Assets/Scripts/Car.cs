using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 carMove;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        carMove = Vector3.zero;
        carMove.x = Input.GetAxisRaw("Horizontal") * speed;
        carMove.z = speed;
        controller.Move(carMove * Time.deltaTime);
    }
}
