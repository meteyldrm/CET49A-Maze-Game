using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Defined From Inspector")] [SerializeField]
    private GameObject enemyBullet;


    [Header("Attributes")] 
    public float maxHealth;
    private float _currentHealth;
    
    public float attackRate;
    private float _lastAttackTime;
    
    public bool playerIsInSight;

    private GameObject _player;

    private void Start()
    {
        _currentHealth = maxHealth;
        _player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        if (playerIsInSight && Time.time > _lastAttackTime + attackRate)
        {
            _lastAttackTime = Time.time;
            GameObject bullet = Instantiate(enemyBullet, transform.position,Quaternion.identity);
            bullet.transform.right = _player.transform.position - bullet.transform.position;
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (_currentHealth - damageAmount <= 0)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _currentHealth -= damageAmount;
        }
    }
}
