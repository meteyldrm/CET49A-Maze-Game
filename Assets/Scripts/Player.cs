using System;
using System.Collections;
using Cinemachine;
using Environment;
using TMPro;
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
    [SerializeField] private Vector2 droneStopDistance;

    private int _playerDirection = -1;
    private static readonly int Speed = Animator.StringToHash("Speed");

    [SerializeField] private CinemachineVirtualCamera playerCamera;
    [SerializeField] private CinemachineVirtualCamera droneCamera;

    private float cmLensSize;
    [SerializeField] private float maxLensSize = 1.8f;
    private float minLensSize;

    [SerializeField]
    private GameObject dronePanel;

    private Coroutine _zoomCameraCoroutine;

    [SerializeField] private GameObject Movable;
    
    [SerializeField]
    private TextMeshProUGUI tmp;

    private void Start()
    {
        _currentHealth = maxHealth;
        _drone = gameObject.transform.Find("DroneTarget").gameObject;
        _droneRigidbody2D = _drone.GetComponent<Rigidbody2D>();
        cmLensSize = playerCamera.m_Lens.OrthographicSize;
        minLensSize = cmLensSize;
        
        droneCamera.gameObject.SetActive(false);

        dronePanel.gameObject.GetComponent<Image>().gameObject.SetActive(false);
        
        StartCoroutine(moveCoroutine());
    }
    
    private void Update()
    {
        healthSlider.value = _currentHealth / maxHealth;
        playerAnimator.SetFloat(Speed,Math.Abs(playerRigidbody2D.velocity.x));
        
        if (Input.GetMouseButton(0) && Time.time > _lastAttackTime + attackRate && !_isDrone)
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
                    if (_hasJumped <= 1) {
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
            SetDrone(!_isDrone); //Toggle behavior
            //SetDrone(true) //for click behavior
        }
        if (Input.GetKeyUp(KeyCode.V)) {
            //SetDrone(false) //for click behavior
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
            
            //Limit magnitude for each component of InputVector based on distance
            var localPosition = _droneRigidbody2D.transform.localPosition;
            var transformVector = inputVector * localPosition;
            transformVector = new Vector2(transformVector.x * (_drone.transform.rotation.y==0 ? 1 : -1), transformVector.y);
            Vector2 guidanceVector = droneStopDistance - transformVector;
            if (guidanceVector.x < 0) {
                guidanceVector.x = 0;
            } else if (guidanceVector.x > 1) {
                guidanceVector.x = 1;
            }
            if (guidanceVector.y < 0) {
                guidanceVector.y = 0;
            } else if (guidanceVector.y > 1) {
                guidanceVector.y = 1;
            }
            
            velocity += Vector2.Lerp(velocity, inputVector * guidanceVector, 2.5f);
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
                        localScale = new Vector3(localScale.x,localScale.y / 2f,transform1.localPosition.z);
                        transform1.localScale = localScale;
                        _shrunk = true;
                    }
                } else {
                    if (_shrunk) {
                        var transform1 = transform;
                        var localScale = transform1.localScale;
                        localScale = new Vector3(localScale.x,localScale.y * 2f,transform1.localPosition.z);
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
                _drone.transform.rotation = transform.rotation;
                _playerDirection = 1;
                healthSlider.direction = Slider.Direction.RightToLeft;
            }
            else if (playerRigidbody2D.velocity.x < -0.05f && _playerDirection == 1)
            {
                transform.Rotate(0,-180,0);
                _drone.transform.rotation = transform.rotation;
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

    void SetDrone(bool state) {
        if (state) {
            dronePanel.gameObject.GetComponent<Image>().gameObject.SetActive(true);
            _isDrone = true;
            playerCamera.gameObject.SetActive(false);
            droneCamera.gameObject.SetActive(true);
            if(_zoomCameraCoroutine != null) StopCoroutine(_zoomCameraCoroutine);
            _zoomCameraCoroutine = StartCoroutine(ZoomCamera(cmLensSize, maxLensSize, 0.5f, 20, droneCamera));
            Movable.GetComponent<MovableMaterialController>().SetMovableState(true);
        } else {
            dronePanel.gameObject.GetComponent<Image>().gameObject.SetActive(false);
            _isDrone = false;
            _drone.transform.localPosition = Vector3.zero;
            droneCamera.gameObject.SetActive(false);
            playerCamera.gameObject.SetActive(true);
            if(_zoomCameraCoroutine != null) StopCoroutine(_zoomCameraCoroutine);
            _zoomCameraCoroutine = StartCoroutine(ZoomCamera(cmLensSize, minLensSize, 0.5f, 20, playerCamera));
            Movable.GetComponent<MovableMaterialController>().SetMovableState(false);
        }
    }

    IEnumerator ZoomCamera(float from, float to, float time, float steps, CinemachineVirtualCamera activeCamera)
    {
        float f = 0;

        activeCamera.m_Lens.OrthographicSize = cmLensSize;
 
        while (f <= 1)
        {
            float size = Mathf.Lerp(from, to, f);
            activeCamera.m_Lens.OrthographicSize = size;
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

        if (other.gameObject.CompareTag("Shoot")) {
            other.gameObject.SetActive(false);
            StartCoroutine(shootCoroutine());
        }
        
        if (other.gameObject.CompareTag("Drone")) {
            other.gameObject.SetActive(false);
            StartCoroutine(droneCoroutine());
        }
        
        if (other.gameObject.CompareTag("Crouch")) {
            other.gameObject.SetActive(false);
            StartCoroutine(crouchCoroutine());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            onGround = false;
        }
    }

    IEnumerator moveCoroutine() {
        tmp.text = "Use WASD to move";
        tmp.gameObject.SetActive(true);
        while (!(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))) {
            yield return null;
        }
        tmp.gameObject.SetActive(false);
    }
    
    IEnumerator shootCoroutine() {
        tmp.text = "Aim with your mouse, hold left click to shoot";
        tmp.gameObject.SetActive(true);
        while (!(Input.GetMouseButton(0))) {
            yield return null;
        }
        tmp.gameObject.SetActive(false);
    }
    
    IEnumerator droneCoroutine() {
        tmp.text = "Go to drone mode by pressing V";
        tmp.gameObject.SetActive(true);
        while (!(Input.GetKeyDown(KeyCode.V))) {
            yield return null;
        }
        tmp.text = "You can move the drone with WASD as well";
        yield return new WaitForSeconds(2);
        tmp.gameObject.SetActive(false);
        StartCoroutine(movableCoroutine());
    }
    
    IEnumerator movableCoroutine() {
        tmp.text = "You can move some boxes while in this mode";
        tmp.gameObject.SetActive(true);
        while (!Input.GetMouseButton(0)) {
            yield return null;
        }
        tmp.gameObject.SetActive(false);
    }
    
    IEnumerator crouchCoroutine() {
        tmp.text = "Hold S to crouch";
        tmp.gameObject.SetActive(true);
        while (!Input.GetKeyDown(KeyCode.S)) {
            yield return null;
        }
        tmp.gameObject.SetActive(false);
    }
}
