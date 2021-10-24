using System;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    [SerializeField] private float turnSpeed;
    public GameObject ball;

    private void Start() {
        ball = GameObject.Find("Sphere");
    }

    void Update()
    {
        
        
        // if (Input.GetKey(KeyCode.A))
        // {
        //     transform.Rotate(Vector3.forward * (turnSpeed * Time.deltaTime));
        // }
        //
        // if (Input.GetKey(KeyCode.D))
        // {
        //     transform.Rotate(Vector3.back * (turnSpeed * Time.deltaTime));
        // }
    }

    void FixedUpdate() 
    {
        transform.RotateAround(ball.transform.position, Vector3.forward, GravityController.Instance.angle * turnSpeed * Time.fixedDeltaTime);
    }
}
