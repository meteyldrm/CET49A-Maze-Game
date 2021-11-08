using System;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    bool isballintheground = true;

    [SerializeField] private float moveSpeed = 0.1f;
    [SerializeField] private float jumpSpeed = 200;

    private Rigidbody playerRB;

    private bool jumpOnce;
    
    private void Start() {
        playerRB = this.GetComponent<Rigidbody>();
        jumpOnce = false;
    }

    void Update() {
        //Camera.main.transform.position = new Vector3(this.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (isballintheground && !jumpOnce) {
                playerRB.AddForce(jumpSpeed * Vector2.up);
                jumpOnce = true;
            }
        }

        if (Input.GetKey(KeyCode.LeftArrow)) {
            playerRB.AddForce(moveSpeed * Vector2.left * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.UpArrow)) {
            playerRB.AddForce(moveSpeed * Vector2.up * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.RightArrow)) {
            playerRB.AddForce(moveSpeed * Vector2.right * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.DownArrow)) {
            playerRB.AddForce(moveSpeed * Vector2.down * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision other) {
        isballintheground = true;
        jumpOnce = true;
    }

    private void OnCollisionExit(Collision other) {
        isballintheground = false;
    }
}
