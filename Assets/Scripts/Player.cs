using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Serialized From Inspector")] 
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Rigidbody2D playerRigidbody2D;
    [SerializeField] private GameObject playerBullet;
    [SerializeField] private Transform playerRightHand;
    
    [Header("Attributes")]
    public float maxHealth;
    private float _currentHealth;
    
    public float moveSpeed;
    public float attackRate;
    private float _lastAttackTime;
    
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

        if (Input.GetMouseButton(0) && Time.time > _lastAttackTime + attackRate)
        {
            _lastAttackTime = Time.time;
            GameObject bullet = Instantiate(playerBullet, playerRightHand.position,Quaternion.identity);
            Vector3 aimPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            aimPoint.z = 0;
            bullet.transform.right = aimPoint - bullet.transform.position;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(15);
        }
    }

    private void FixedUpdate()
    {
        playerRigidbody2D.AddForce(moveSpeed * _inputDirection * Time.fixedDeltaTime * Vector2.right);
        if (playerRigidbody2D.velocity.x > 0.05f && _playerDirection == -1)
        {
            transform.Rotate(0,180,0);
            _playerDirection = 1;
        }
        else if (playerRigidbody2D.velocity.x < -0.05f && _playerDirection == 1)
        {
            transform.Rotate(0,-180,0);
            _playerDirection = -1;
        }
    }

    public void Heal(float healAmount)
    {
        if (_currentHealth + healAmount > maxHealth)
        {
            _currentHealth = maxHealth;
        }
        else
        {
            _currentHealth += healAmount;
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (_currentHealth - damageAmount < 0)
        {
            _currentHealth = 0;
            // DEATH
        }
        else
        {
            _currentHealth -= damageAmount;
        }
    }

    public void AttackRateBoost(float boostTime, float boostAmount)
    {
        StartCoroutine(AttackRateBoostCoroutine(boostTime, boostAmount));
    }

    private IEnumerator AttackRateBoostCoroutine(float boostTime, float boostAmount)
    {
        attackRate -= boostAmount;
        yield return new WaitForSeconds(boostTime);
        attackRate = +boostAmount;
    }
}
