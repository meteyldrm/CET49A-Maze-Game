using System;
using System.Collections;
using Cinemachine;
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
    private bool _shrunk;
    
    private GameObject _drone;
    private Rigidbody2D _droneRigidbody2D;
    private bool _isDrone = false;
    private Vector2 inputVector = Vector2.zero;

    private int _playerDirection = -1;
    private static readonly int Speed = Animator.StringToHash("Speed");

    private CinemachineVirtualCamera cmCamera; 
    private CinemachineFramingTransposer cmFramingTransposer;
    private float cmLookaheadTime;
    private float cmLookaheadSmoothing;
    private float cmLensSize;
    [SerializeField] private float maxLensSize = 1.8f;
    private float minLensSize;

    [SerializeField]
    private GameObject dronePanel;

    private Coroutine _zoomCameraCoroutine;

    private void Start()
    {
        _currentHealth = maxHealth;
        _drone = gameObject.transform.Find("DroneTarget").gameObject;
        _droneRigidbody2D = _drone.GetComponent<Rigidbody2D>();
        cmCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        cmFramingTransposer = cmCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        cmLookaheadTime = cmFramingTransposer.m_LookaheadTime;
        cmLookaheadSmoothing = cmFramingTransposer.m_LookaheadSmoothing;
        cmLensSize = cmCamera.m_Lens.OrthographicSize;
        minLensSize = cmLensSize;

        dronePanel.gameObject.GetComponent<Image>().gameObject.SetActive(false);
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

        #region KeyInput

        if (Input.GetKeyDown(KeyCode.A))
        {
            inputVector.x -= 1;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            inputVector.x += 1;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            inputVector.x += 1;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            inputVector.x -= 1;
        }

        if (Input.GetKeyDown(KeyCode.W)) {
            inputVector.y += 1;
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
            inputVector.y -= 1;
        }

        if (Input.GetKeyDown(KeyCode.S)) {
            inputVector.y -= 1;
        }
        if (Input.GetKeyUp(KeyCode.S)) {
            inputVector.y += 1;
        }

        if (Input.GetKeyDown(KeyCode.V)) {
            dronePanel.gameObject.GetComponent<Image>().gameObject.SetActive(true);
            _isDrone = true;
            cmFramingTransposer.m_LookaheadTime = 0;
            cmFramingTransposer.m_LookaheadSmoothing = 0;
            if(_zoomCameraCoroutine != null) StopCoroutine(_zoomCameraCoroutine);
            _zoomCameraCoroutine = StartCoroutine(ZoomCamera(cmLensSize, maxLensSize, 0.5f, 20));
        }
        if (Input.GetKeyUp(KeyCode.V)) {
            dronePanel.gameObject.GetComponent<Image>().gameObject.SetActive(false);
            _isDrone = false;
            _drone.transform.localPosition = Vector3.zero;
            cmFramingTransposer.m_LookaheadTime = cmLookaheadTime;
            cmFramingTransposer.m_LookaheadSmoothing = cmLookaheadSmoothing;
            if(_zoomCameraCoroutine != null) StopCoroutine(_zoomCameraCoroutine);
            _zoomCameraCoroutine = StartCoroutine(ZoomCamera(cmLensSize, minLensSize, 0.5f, 20));
        }

        #endregion
        
        //smoothly reset drone position every frame 
        if (!_isDrone) _droneRigidbody2D.velocity = playerRigidbody2D.velocity;
    }

    private void FixedUpdate()
    {
        playerRigidbody2D.velocity *= Vector2.one * 0.95f;
        if (_isDrone) {
            var velocity = _droneRigidbody2D.velocity;
            velocity += Vector2.Lerp(velocity, inputVector, 2.5f);
            velocity  *= 0.75f;
            _droneRigidbody2D.velocity = velocity;
        } else {
            if (onGround)
            {
                playerRigidbody2D.AddForce(moveSpeed * inputVector.x * Time.fixedDeltaTime * Vector2.right);
                if (inputVector.y < 0) {
                    if (!_shrunk) {
                        var transform1 = transform;
                        var localScale = transform1.localScale;
                        localScale = new Vector3(localScale.x,localScale.y / 1.5f,transform1.localPosition.z);
                        transform1.localScale = localScale;
                        _shrunk = true;
                    }
                } else {
                    if (_shrunk) {
                        var transform1 = transform;
                        var localScale = transform1.localScale;
                        localScale = new Vector3(localScale.x,localScale.y * 1.5f,transform1.localPosition.z);
                        transform1.localScale = localScale;
                        _shrunk = false;
                    }
                }
            }
            else
            {
                playerRigidbody2D.AddForce(moveSpeed * inputVector.x * Time.fixedDeltaTime * Vector2.right / 2);
            }
        
        
            if (playerRigidbody2D.velocity.x > 0.05f && _playerDirection == -1)
            {
                transform.Rotate(0,180,0);
                _drone.transform.Rotate(0,180,0);
                _playerDirection = 1;
                healthSlider.direction = Slider.Direction.RightToLeft;
            }
            else if (playerRigidbody2D.velocity.x < -0.05f && _playerDirection == 1)
            {
                transform.Rotate(0,-180,0);
                _drone.transform.Rotate(0,-180,0);
                _playerDirection = -1;
                healthSlider.direction = Slider.Direction.LeftToRight;
            }
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

    #region Boosters

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
    
    #endregion

    IEnumerator ZoomCamera(float from, float to, float time, float steps)
    {
        float f = 0;

        cmCamera.m_Lens.OrthographicSize = cmLensSize;
 
        while (f <= 1)
        {
            float size = Mathf.Lerp(from, to, f);
            cmCamera.m_Lens.OrthographicSize = size;
            cmLensSize = size;
 
            f += 1f/steps;
 
            yield return new WaitForSeconds(time/steps);
        }
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
