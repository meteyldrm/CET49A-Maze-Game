using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Serialized From Inspector")] 
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Rigidbody2D playerRigidbody2D;
    [SerializeField] private GameObject playerBullet;
    [SerializeField] private Transform playerRightHand;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image sliderFillImage;
    [SerializeField] private Image sliderBgImage;
    [SerializeField] private Color fillColor;
    [SerializeField] private Color bgColor;
    
    [Header("Attributes")]
    public float maxHealth;
    private float _currentHealth;
    
    public float moveSpeed;
    public float jumpHeight;
    public bool onGround;
    
    public float attackRate;
    private float _lastAttackTime;

    private bool _isInvulnerable;

    private int _hasJumped = 0;
    
    // Input Checks
    private int _inputDirection = 0;

    private GameObject _drone;
    private Rigidbody2D _droneRigidbody2D;
    private bool _isDrone = false;
    private Vector2 _droneVector = Vector2.zero;


    private int _playerDirection = -1;
    private static readonly int Speed = Animator.StringToHash("Speed");

    private void Start()
    {
        _currentHealth = maxHealth;
        _drone = gameObject.transform.Find("DroneTarget").gameObject;
        _droneRigidbody2D = _drone.GetComponent<Rigidbody2D>();
    }


    private void Update()
    {
        healthSlider.value = _currentHealth / maxHealth;
        playerAnimator.SetFloat(Speed,Math.Abs(playerRigidbody2D.velocity.x));
        
        if (Input.GetMouseButton(0) && Time.time > _lastAttackTime + attackRate)
        {
            _lastAttackTime = Time.time;
            GameObject bullet = Instantiate(playerBullet, playerRightHand.position,Quaternion.identity);
            Vector3 aimPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            aimPoint.z = 0;
            bullet.transform.right = aimPoint - bullet.transform.position;
        }
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            _inputDirection -= 1;
            _droneVector.x -= 1;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            _inputDirection += 1;
            _droneVector.x += 1;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            _inputDirection += 1;
            _droneVector.x += 1;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            _inputDirection -= 1;
            _droneVector.x -= 1;
        }

        if (Input.GetKeyDown(KeyCode.W)) {
            _droneVector.y += 1;
            if (!_isDrone) {
                if (onGround) {
                    playerRigidbody2D.AddForce(Vector2.up * jumpHeight,ForceMode2D.Force);
                    _hasJumped += 1;
                } else {
                    if (_hasJumped <= 2) {
                        playerRigidbody2D.velocity *= new Vector2(1, 0);
                        playerRigidbody2D.AddForce(Vector2.up * jumpHeight,ForceMode2D.Force);
                        _hasJumped += 1;
                    }
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.W)) {
            _droneVector.y -= 1;
        }

        if (Input.GetKeyDown(KeyCode.S)) {
            _droneVector.y -= 1;
            if (!_isDrone) {
                var transform1 = transform;
                var localScale = transform1.localScale;
                localScale = new Vector3(localScale.x,localScale.y / 1.5f,transform1.localPosition.z);
                transform1.localScale = localScale;
            }
        }

        if (Input.GetKeyUp(KeyCode.S)) {
            _droneVector.y += 1;
            if (!_isDrone) {
                var transform1 = transform;
                var localScale = transform1.localScale;
                localScale = new Vector3(localScale.x,localScale.y * 1.5f,transform1.localPosition.z);
                transform1.localScale = localScale;
            }
        }

        if (Input.GetKeyDown(KeyCode.V)) {
            _isDrone = true;
            _droneVector = Vector2.zero;
            _inputDirection = 0;
        }
        
        if (Input.GetKeyUp(KeyCode.V)) {
            _isDrone = false;
            _drone.transform.localPosition = Vector3.zero;
            _droneVector = Vector2.zero;
            _inputDirection = 0;
        }
        
        //smoothly reset drone position every frame 
        if (!_isDrone) _droneRigidbody2D.velocity = playerRigidbody2D.velocity;
    }

    private void FixedUpdate()
    {
        if (_isDrone) {
            //X component has a bounceback for some reason
            var velocity = _droneRigidbody2D.velocity;
            velocity += Vector2.Lerp(velocity, _droneVector * 2, 5);
            print(velocity + " " + _droneVector);
            velocity  *= 0.75f;
            _droneRigidbody2D.velocity = velocity;
        } else {
            if (onGround)
            {
                playerRigidbody2D.AddForce(moveSpeed * _inputDirection * Time.fixedDeltaTime * Vector2.right);
            }
            else
            {
                playerRigidbody2D.AddForce(moveSpeed * _inputDirection * Time.fixedDeltaTime * Vector2.right / 2);
            }
            playerRigidbody2D.velocity *= Vector2.one * 0.95f;
        
        
            if (playerRigidbody2D.velocity.x > 0.05f && _playerDirection == -1)
            {
                transform.Rotate(0,180,0);
                _playerDirection = 1;
                healthSlider.direction = Slider.Direction.RightToLeft;
            }
            else if (playerRigidbody2D.velocity.x < -0.05f && _playerDirection == 1)
            {
                transform.Rotate(0,-180,0);
                _playerDirection = -1;
                healthSlider.direction = Slider.Direction.LeftToRight;
            }
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
        if (_isInvulnerable) return;
        if (_currentHealth - damageAmount <= 0)
        {
            _currentHealth = 0;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

    public void GainInvulnerability(float boostTime)
    {
        StopCoroutine(GainInvulnerabilityCoroutine(0));
        StartCoroutine(GainInvulnerabilityCoroutine(boostTime));
    }

    private IEnumerator GainInvulnerabilityCoroutine(float boostTime)
    {
        _isInvulnerable = true;

        sliderFillImage.color = Color.yellow;
        sliderBgImage.color = Color.yellow;
        
        yield return new WaitForSeconds(boostTime);

        sliderFillImage.color = fillColor;
        sliderBgImage.color = bgColor;
        
        _isInvulnerable = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            onGround = true;
            _hasJumped = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            onGround = false;
        }
    }
}
