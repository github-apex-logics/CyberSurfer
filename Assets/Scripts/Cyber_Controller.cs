
using LightDI;
using MaykerStudio.Demo;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.Windows;



public class Cyber_Controller : MonoBehaviour, IInjectable
{
    [Header("References")]
    [Inject] private LevelManager LevelManager;
    public WeaponDatabaseSO WeaponDatabaseSO;
    public Rigidbody rb;
    public GameObject camera;
    public GameObject characterVisuals;
    public Transform groundCheck;
    public Transform edgeCheck;
    public Transform resetTarget;
    public UIControllerBtns UIControllerBtns;
    public AudioSource audioSource;
    public Material knifeMaterial;


    [Header("Movement Settings")]
    [Tooltip("Fixed movement speed")]
    public float moveSpeed = 50f;

    [Tooltip("Turning speed of the character")]
    public float turnSpeed = 200f;

    [Tooltip("Sensitivity multiplier for turning")]
    public float turnSensitivity = 1.5f;

    [Tooltip("Multiplier for momentum preservation")]
    public float momentumMultiplier = 15f;

    [Tooltip("Force applied while steering in air")]
    public float airSteeringFactor = 2f;

    [Tooltip("Boost multiplier when on ramp")]
    public float rampBoost = 1.2f;

    [Tooltip("Strength of movement alignment to ramp surface")]
    public float rampAlignmentStrength = 0.6f;

    [Header("Jump & Gravity Settings")]
    [Tooltip("Jump force when on ground")]
    public float jumpForce = 10f;

    [Tooltip("Additional boost when jumping from ramp edge")]
    public float extraJumpForce = 15f;

    [Tooltip("Forward force when jumping from ramp edge")]
    public float edgeBoostForwardForce = 5f;

    [Tooltip("Gravity multiplier for falling")]
    public float gravityMultiplier = 2f;

    [Header("Ground Check Settings")]
    [Tooltip("Radius used to check for ground")]
    public float groundCheckRadius = 0.3f;

    [Tooltip("Layer for detecting ground")]
    public LayerMask groundLayer;

    [Tooltip("Layer for detecting ramps")]
    public LayerMask rampLayer;

    [Tooltip("Layer for detecting ramp edges")]
    public LayerMask rampEdgeLayer;

    [Header("Combat / Slash Settings")]
    [Tooltip("Slash projectile prefab")]
    public GameObject slashEffectPrefab;

    [Tooltip("Homing slash prefab")]
    public GameObject homingSlash;

    [Tooltip("Slash particle effect")]
    public ParticleSystem slashEffect;

    [Tooltip("Where the slash spawns from")]
    public Transform spawnPoint;

    [Tooltip("Speed of the slash projectile")]
    public float slashSpeed = 20f;

    [Tooltip("Lifetime of the slash projectile")]
    public float slashLifetime = 1.5f;





    [Header("Health Settings")]
    [Tooltip("Maximum health value")]
    public float health;

    [Tooltip("Current health")]
    public float currentHealth;

    [Tooltip("UI health bar reference")]
    public Image healthBar;


    [Header("Rotation Settings")]
    public float dragSensitivity = 0.3f; // How much rotation per pixel dragged
    public float rotationSmoothness = 10f;


    private Vector2 lastTouchPos;
    private bool isDragging = false;

    [Header("Optional UI Touch Zone")]
    public RectTransform touchZoneUI;


    // --- Internal States ---
    private bool hasBoosted, hasShield;
    private float turnInput, turnInputY;
    private Vector3 moveDirection;
    private RaycastHit rampHit;
    public bool isGrounded, onRamp, onRampEdge;
    public float testtime;
    private float currentTilt = 0f;
    [SerializeField] private float tiltDamping = 10f;
    private float targetYaw, targetPitch;

    public Texture knifeTexture, knifeEmission;

    private void OnEnable()
    {
        UIControllerBtns = FindAnyObjectByType<UIControllerBtns>();
       // LevelManager = FindAnyObjectByType<LevelManager>();

        

        touchZoneUI= UIControllerBtns.touchArea;

        int selectedIndex = PlayerPrefs.GetInt("SelectedCharacterIndex", 0);
      


        StartCoroutine(InjectionDelay());
        LoadCharacter(Database.SelectedCharacter);
        LoadKnifeTextures();
    }



    IEnumerator InjectionDelay()
    {
        yield return new WaitForEndOfFrame();
        InjectionManager.RegisterObject(this);

             
    }
   

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevents unwanted tilting
       // audioSource = GetComponent<AudioSource>();
        UIControllerBtns.powerUpBtn.onClick.AddListener(ActivatePowerUp);
        currentHealth = health;
        targetYaw = transform.eulerAngles.y;
    }

    void Update()
    {
       
    }

    void FixedUpdate()
    {
        if (LevelManager == null)
            return;
        else if (LevelManager.startGame)
        {
            HandleMovement();
            HandleMobileInput();
            //sHandleRotation();
            // HandleTilt();

            HandleTouchInput();
        }
    }


    void LoadCharacter(int index)
    {
        
        for (int i = 0; i < 4; i++)
        {
            if (i == index)
            {
                characterVisuals.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                characterVisuals.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        
    }
    
    void LoadKnifeTextures()
    {
        knifeTexture = WeaponDatabaseSO.weapons[Database.SelectedKnife - 1].base_texture;
        knifeEmission = WeaponDatabaseSO.weapons[Database.SelectedKnife - 1].emission_texture;

        knifeMaterial.SetTexture("_BaseMap", knifeTexture);
        knifeMaterial.SetTexture("_EmissionMap", knifeEmission);
    }
    private bool IsTouchWithinUIZone(Vector2 screenPos)
    {
        if (touchZoneUI == null) return true;

        return RectTransformUtility.RectangleContainsScreenPoint(touchZoneUI, screenPos, null);
    }


    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);


            if (!IsTouchWithinUIZone(touch.position))
                return;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    lastTouchPos = touch.position;
                    isDragging = true;
                    break;

                case TouchPhase.Moved:
                    if (isDragging)
                    {
                        float deltaX = touch.position.x - lastTouchPos.x;
                        targetYaw += deltaX * TurnSensitivity();

                        float deltaY =   touch.position.y - lastTouchPos.y;
                        targetPitch -= deltaY * TurnSensitivity();

                        lastTouchPos = touch.position;
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isDragging = false;
                    break;
            }
        }


        Quaternion targetRot = Quaternion.Euler(Mathf.Clamp( targetPitch,-30,30), targetYaw, 0f);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, Time.fixedDeltaTime * rotationSmoothness));

    }
   

    void HandleMobileInput()
    {
        turnInput = 0;
        turnInputY = 0;
        
        turnInput = Input.GetAxis("Horizontal");
        turnInputY = Input.GetAxis("Vertical");

    }

    void HandleMovement()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        onRamp = Physics.CheckSphere(groundCheck.position, groundCheckRadius, rampLayer);
        onRampEdge = Physics.CheckSphere(groundCheck.position, groundCheckRadius, rampEdgeLayer);

        //onRamp = Physics.Raycast(groundCheck.position, Vector3.down, out rampHit, groundCheckRadius * 2f, rampLayer);
        //onRampEdge = Physics.Raycast(edgeCheck.position, Vector3.down, out RaycastHit edgeHit, groundCheckRadius * 2f, rampEdgeLayer);

        Vector3 forward = transform.forward;

        // Align with ramp if available
        if (onRamp)
        {
            Vector3 rampAligned = Vector3.ProjectOnPlane(forward, rampHit.normal).normalized;
            moveDirection = Vector3.Lerp(forward, rampAligned, rampAlignmentStrength) * moveSpeed * rampBoost;
        }
        else
        {
            moveDirection = forward * moveSpeed;
        }

        // Momentum preserving horizontal velocity
        Vector3 horizontalVelocity = Vector3.ProjectOnPlane(rb.linearVelocity, Vector3.up);
        Vector3 velocityChange = (moveDirection - horizontalVelocity);
        velocityChange.y = 0f;
        rb.AddForce(velocityChange * momentumMultiplier, ForceMode.Acceleration);

        // Air control
        if (!isGrounded)
        {
            Vector3 airControl = transform.right * currentRotationInput * airSteeringFactor;
            rb.AddForce(airControl, ForceMode.Acceleration);
        }

        // Bunny hop
        if (isGrounded && rb.linearVelocity.y <= 0.1f)
        {
            StartCoroutine(Jump());

            if (SoundManager.instance)
                SoundManager.instance.PlayClip(ClipName.Jump);
        }

        // Ramp edge boost
        if (onRampEdge && !hasBoosted)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, extraJumpForce, rb.linearVelocity.z);
            Vector3 edgeForwardBoost = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            rb.linearVelocity += edgeForwardBoost * edgeBoostForwardForce;
            hasBoosted = true;
        }

        // on Ramp
        if (onRamp)
        {
           audioSource.gameObject.SetActive(true);

        }
        else
        {
            audioSource.gameObject.SetActive(false);
        }


        if (!onRampEdge && isGrounded)
        {
            hasBoosted = false; // Reset boost flag when back on ground
        }

        // Apply gravity
        rb.AddForce(Vector3.down * gravityMultiplier * rb.mass, ForceMode.Acceleration);
    }

    IEnumerator Jump()
    {
        yield return new WaitForSeconds(testtime);
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
    }
   
  




    float currentPitch = 0;


    float currentRotationInput = 0f;
   public float smoothVelocity = 0f; // For SmoothDamp


    // ==> old  controller

//    void HandleRotation()
//    {
//        float rawInput = 0f;

////#if UNITY_ANDROID && !UNITY_EDITOR
//    rawInput = joystick.Horizontal;
////#elif UNITY_EDITOR
////        rawInput = turnInput;
////#endif

//        // Smooth the input (feel free to tune the `smoothTime`)
//        currentRotationInput = Mathf.SmoothDamp(currentRotationInput, rawInput, ref smoothVelocity, 0.05f);

//        if (Mathf.Abs(currentRotationInput) > 0.01f)
//        {
//            float rotationThisFrame = currentRotationInput * (turnSpeed * TurnSensitivity()) * Time.fixedDeltaTime;
//            Quaternion deltaRotation = Quaternion.Euler(0f, rotationThisFrame, 0f);
//            Quaternion targetRotation = rb.rotation * deltaRotation;
//            rb.MoveRotation(targetRotation);
//        }
//    }



   
   
    float TurnSensitivity()
    {
        dragSensitivity = PlayerPrefs.GetFloat("Sensitivity", 0.25f);
        return dragSensitivity;
    }


    void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

   public CharacterAnimation ca;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            //isGrounded = true;
            // if (SoundManager.instance)
            //     SoundManager.instance.PlayClip(ClipName.Jump);

           // ca.Jump();
        }
        if (collision.gameObject.CompareTag("Finish"))
        {
            SoundManager.instance.PlayClip(ClipName.LevelComplete);
            StartCoroutine(delay());
        }
    }
    public bool slashAnim;

    public void ActivatePowerUp()
    {
        if (UIControllerBtns.powerUp == Powers.Slash)
        {
            SpawnSlash(slashEffectPrefab);
        }
        if (UIControllerBtns.powerUp == Powers.Boost)
        {
            Boost();
        }
        if (UIControllerBtns.powerUp == Powers.HomingSlash)
        {
            HomingSpawnSlash(homingSlash);
        }
        if (UIControllerBtns.powerUp == Powers.Shield)
        {
            ActivateShield();
        }


        UIControllerBtns.SwitchSlots();
        UIControllerBtns.PowerButtonInteractbale(false);
        //UIControllerBtns.powerUpIcon.sprite = null;

    }

    //to call this func directly from the button
    public void SlashPower()
    {
        SpawnSlash(slashEffectPrefab);
    }

    void Boost()
    {
        StartCoroutine(Booster());   
    }

    IEnumerator Booster()
    {
        UIControllerBtns.boostEffect.SetActive(true);
        moveSpeed += 30f;
        yield return new WaitForSeconds(10f);
        moveSpeed -= 30f;
        UIControllerBtns. boostEffect.SetActive(false);
    }



    public void SpawnSlash(GameObject obj)
    {

        GameObject slash = Instantiate(obj, spawnPoint.position, spawnPoint.rotation);
        slashAnim = true;   
        slashEffect.Play();
        StartCoroutine(SlashDelay(slash));
      
    }

    public void ActivateShield()
    {
        StartCoroutine(ShieldEffect());
    }

    IEnumerator ShieldEffect()
    {
        UIControllerBtns.shieldObj.SetActive(true);
        hasShield = true;
        yield return new WaitForSeconds(10f);
        hasShield = false;
        UIControllerBtns.shieldObj.SetActive(false);
    }

    public void HomingSpawnSlash(GameObject obj)
    {

        GameObject slash = Instantiate(obj, spawnPoint.position, spawnPoint.rotation);
        slashAnim = true;
        slashEffect.Play();
        StartCoroutine(homingSlashDelay(slash));

    }
    IEnumerator homingSlashDelay(GameObject slash)
    {
        yield return new WaitForSeconds(0.025f);
        slashAnim = false;
       
       // Destroy(slash, slashLifetime);
    }

    IEnumerator SlashDelay(GameObject slash)
    {
        yield return new WaitForSeconds(0.025f);
        slashAnim=false;
        Rigidbody rb = slash.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = spawnPoint.forward * slashSpeed;
        }

        Destroy(slash, slashLifetime);
    }

    IEnumerator delay()
    {
        yield return new WaitForSeconds(0.5f);

       
        LevelManager.GameComplete();
 
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
          //  isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {

        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<RespawnCollider>(out RespawnCollider rc))
        {
            resetTarget = rc.spawnPoint;
            Debug.Log("I m called :  ");
        }
        if (other.gameObject.CompareTag("DeadZone"))
        {
            ResetPos();
        }
        if (other.gameObject.CompareTag("Coin"))
        {
            SoundManager.instance.PlayClip(ClipName.Coin);
           // slashAnim = true;
            AddCoin();
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("Enemy_Shot") && !hasShield)
        {
            TakeDamage(5);
        }


       


        if (other.gameObject.TryGetComponent<PowerUp>(out PowerUp pu))
        {
     
            SoundManager.instance.PlayClip(ClipName.PowerUp);
           
            //UIControllerBtns.UpdateIcon(pu.powerUp);
            UIControllerBtns.PowerSlot(pu.powerUp);
            UIControllerBtns.PowerButtonInteractbale(true);
            pu.ChoosePowerUps();

            other.gameObject.SetActive(false);
        }
       
    }   


    void UpdateHealthUI()
    {
        float fillAmount = currentHealth / health;
        //Debug.Log(fillAmount);

        healthBar.fillAmount = fillAmount;
    }
    public void TakeDamage(int damage)
    {

        currentHealth -= damage;
        UIControllerBtns.hitEffect.SetActive(true);
        UpdateHealthUI();

        camera.GetComponent<CameraShake>().Shake();

        if (currentHealth <= 0)
        {
            Die();
        }

    }

    private void Die()
    {
        SoundManager.instance.PlayClip(ClipName.EnemyDie);
        moveSpeed = 0;
        LevelManager.startGame = false;
       // Destroy(gameObject, 4f);
        StartCoroutine(delayDie());
    }

    IEnumerator delayDie()
    {
        yield return new WaitForSeconds(0.5f);
        LevelManager.GameOver();
       // LevelManager.EndGame();
    }


    void AddCoin()
    {
        int c = Database.Coins;
        c++;
        LevelManager.coinCount++;
        Database.Coins = c;
    }


    public void ResetPos()
    {
        transform.position = resetTarget.position;
        transform.rotation = resetTarget.rotation;
        targetYaw = resetTarget.eulerAngles.y;
        SoundManager.instance.PlayClip(ClipName.Spawn);
    }

    public void PostInject()
    {
        
    }
}


