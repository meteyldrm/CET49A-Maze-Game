using System;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Serialized From Inspector")] 
    public Animator playerAnimator;
    public Rigidbody2D playerRigidbody2D;
    
    [Header("Attributes")]
    public float maxHealth;
    private float _currentHealth;
    public float moveSpeed;
    
    // Input Checks
    private int _inputDirection = 0;



    private int _playerDirection = -1;
    private static readonly int Speed = Animator.StringToHash("Speed");

    private void Start()
    {
        _currentHealth = maxHealth;
    }


    private void Update()
    {
        playerAnimator.SetFloat(Speed,Math.Abs(playerRigidbody2D.velocity.x));

        if (Input.GetKeyDown(KeyCode.A))
        {
            _inputDirection -= 1;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            _inputDirection += 1;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            _inputDirection += 1;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            _inputDirection -= 1;
        }
    }

    private void FixedUpdate()
    {
        playerRigidbody2D.AddForce(moveSpeed * _inputDirection * Time.fixedDeltaTime * Vector2.right);
        if (playerRigidbody2D.velocity.x > 0 && _playerDirection == -1)
        {
            transform.Rotate(0,180,0);
            _playerDirection = 1;
        }
        else if (playerRigidbody2D.velocity.x < 0 && _playerDirection == 1)
        {
            transform.Rotate(0,-180,0);
            _playerDirection = -1;
        }

        playerRigidbody2D.velocity *= Vector2.one * 0.95f;
    }
}
