using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;
using System.Collections;
using System;

public class CyberNetworkController : NetworkBehaviour
{
    [Header("References")]
    public Rigidbody rb;
    public GameObject cameraObj, characterObj;
   
    public Image healthBar;
    public GameObject completePanel;
    public NetworkedManager networkManager;
    public SceneCalls sceneCallbacks;

    [Header("Gameplay")]
    public float moveSpeed = 50f;
    public float turnSpeed = 200f;
    public float jumpForce = 10f;
    public float gravityMultiplier = 2f;
    public float extraJumpForce = 15f;

    public Transform groundCheck;
    public Transform edgeCheck;
    public LayerMask groundLayer, rampLayer, rampEdgeLayer;
    public float groundCheckRadius = 0.3f;

    public GameObject slashEffectPrefab, homingSlash;
    public ParticleSystem slashEffect;
    public Transform spawnPoint;
    public float slashSpeed = 20f;
    public float slashLifetime = 1.5f;
    public GameObject boostEffect;

    [Networked] public float currentHealth { get; set; }
    [Networked] public bool isJumping { get; set; }
    [Networked] public bool startGame { get; set; }
    [Networked] public NetworkBool slashAnim { get; set; }
    [Networked] public bool isGrounded { get; set; }
    [Networked] public bool onRamp { get; set; }
    [Networked] public bool onRampEdge { get; set; }

    [Networked] public float ProgressToFinish { get; set; }

    public Transform finishLine;


    private UIControllerBtns ui;
    public float horizontalInput;
    private float verticalInput;
    public Transform resetTarget;
   // public bool isGrounded, onRamp, onRampEdge;
    private RaycastHit rampHit;



    public RectTransform touchZoneUI;

    // Touch rotation input
    public float rotationInput = 0f;
    public float accumulatedRotationDelta, targetPitch;
    public Quaternion initialRotation;
    private Vector2 lastTouchPos;
    private bool isDragging = false;
    private float targetYaw;
    private float dragSensitivity = 0.2f;
    public float rotationSmoothness = 10f;
    public bool initializedRotation;


 
    public override void Spawned()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        ui = FindAnyObjectByType<UIControllerBtns>();
        networkManager = FindAnyObjectByType<NetworkedManager>();
        sceneCallbacks = FindAnyObjectByType<SceneCalls>();

        if (HasInputAuthority)
        {
            cameraObj.SetActive(true);
     
            touchZoneUI = ui.touchArea;
            ui.powerUpBtn.onClick.AddListener(ActivatePowerUp);
            accumulatedRotationDelta = transform.eulerAngles.y;

            sceneCallbacks.localPlayerController = this;
            finishLine = networkManager.finish;
        }
        else
        {
            cameraObj.SetActive(false);
        }

        if (Object.HasStateAuthority)
        {
            currentHealth = 100;
            NetworkedManager.Instance.MarkPlayerReady(Object.InputAuthority);
        }
        if (Object.HasStateAuthority)
        {
            initialRotation = transform.rotation; //  Store it once
        }
    }


    void Update()
    {
        if (HasInputAuthority)
        {
            HandleTouchInput();
        }
    }



    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority || !networkManager.startGame) return;

        if (!initializedRotation)
        {
            rb.MoveRotation(initialRotation);
            initializedRotation = true;
        }

        if (GetInput(out PlayerInputData input))
        {
            horizontalInput = input.horizontal;
            verticalInput = input.vertical;



            Quaternion targetRot = Quaternion.Euler(Mathf.Clamp(input.rotationPitchDelta, -30, 30), input.rotationDelta, 0f);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, Time.fixedDeltaTime * rotationSmoothness));

            HandleMovement();
            HandleRotation();
            ApplyGravity();
        }
        TrackProgress();

    }


    private void HandleTouchInput()
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
                        accumulatedRotationDelta += deltaX * dragSensitivity; //  accumulate delta

                        float deltaY = touch.position.y - lastTouchPos.y;
                        targetPitch -= deltaY * dragSensitivity;


                        lastTouchPos = touch.position;

                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isDragging = false;
                    break;
            }
        }
    }

    private void ApplyTouchRotation(float inputYaw)
    {
        if (Mathf.Abs(inputYaw) > 0.01f)
        {
            targetYaw += inputYaw;
            Quaternion targetRot = Quaternion.Euler(0f, targetYaw, 0f);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, Time.fixedDeltaTime * 10f));
        }
    }

    private bool IsTouchWithinUIZone(Vector2 screenPos)
    {
        if (touchZoneUI == null) return true;

        return RectTransformUtility.RectangleContainsScreenPoint(touchZoneUI, screenPos, null);
    }




    private void HandleMovement()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        onRamp = Physics.Raycast(groundCheck.position, Vector3.down, out rampHit, groundCheckRadius * 2f, rampLayer);
        onRampEdge = Physics.Raycast(edgeCheck.position, Vector3.down, out _, groundCheckRadius * 2f, rampEdgeLayer);

        if (!isGrounded)
        {
            Vector3 moveDirection = transform.forward * moveSpeed;
            rb.linearVelocity = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.z);
        }

        if (isGrounded && !isJumping)
        {
            Runner.StartCoroutine(Jump());
        }

        if (onRampEdge)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, extraJumpForce, rb.linearVelocity.z);
        }
    }

    private IEnumerator Jump()
    {
        isJumping = true;
        yield return new WaitForSeconds(0.05f);
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        isJumping = false;
    }

    private void HandleRotation()
    {
        //float turnInput = HasInputAuthority ? horizontalInput : 0f;
        //float rotationAmount = turnInput * turnSpeed * Runner.DeltaTime;//.fixedDeltaTime;

        //if (Mathf.Abs(rotationAmount) > 0.01f)
        //{
        //    Quaternion delta = Quaternion.Euler(0, rotationAmount, 0);
        //    rb.MoveRotation(rb.rotation * delta);
        //}


        float rotationAmount = horizontalInput * turnSpeed * Runner.DeltaTime;
        float rotationAmountYaw = verticalInput * turnSpeed * Runner.DeltaTime;

        if (Mathf.Abs(rotationAmount) > 0.01f)
        {
            Quaternion deltaRotation = Quaternion.Euler(Mathf.Clamp(rotationAmountYaw, -30, 30), rotationAmount, 0f);
            rb.MoveRotation(rb.rotation * deltaRotation);
        }



    }

    private void ApplyGravity()
    {
        rb.AddForce(Vector3.down * gravityMultiplier * rb.mass, ForceMode.Acceleration);
    }

    public void ActivatePowerUp()
    {
        if (!HasInputAuthority) return;
        //if (ui.powerUp == Powers.Slash) TrySpawnSlash();// SpawnSlash(slashEffectPrefab);
        //if (ui.powerUp == Powers.Boost) Runner.StartCoroutine(Boost());
        //if (ui.powerUp == Powers.HomingSlash) SpawnSlash(homingSlash);
        TrySpawnSlash();
    }








    public void TrySpawnSlash()
    {
        if (HasInputAuthority)
        {
            SpawnSlash_RPC(spawnPoint.forward);
            slashAnim = true;
            Runner.StartCoroutine(SlashDelay());
            Debug.Log(" i m called for slash " );
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    void SpawnSlash_RPC(Vector3 direction)
    {
        if (slashEffect != null)
            slashEffect.Play();

        var slashObj = Runner.Spawn(slashEffectPrefab, spawnPoint.position, Quaternion.LookRotation(direction), Object.InputAuthority);
        if (slashObj.TryGetComponent<Slash>(out var slash))
        {
            slash.Launch(direction);
        }
    }






    private void SpawnSlash(GameObject prefab)
    {
        GameObject slash = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        slashAnim = true;
        slashEffect.Play();
     //   Runner.StartCoroutine(SlashDelay(slash));
    }

    private IEnumerator SlashDelay()
    {
        yield return new WaitForSeconds(0.025f);
        slashAnim = false;
       
    }

    private IEnumerator Boost()
    {
        boostEffect.SetActive(true);
        moveSpeed += 30f;
        yield return new WaitForSeconds(10f);
        moveSpeed -= 30f;
        boostEffect.SetActive(false);
    }

    public void TakeDamage(int amount)
    {
        if (!HasStateAuthority) return;
        currentHealth -= amount;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthBar)
        {
            float fill = currentHealth / 100f;
            healthBar.fillAmount = fill;
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
             if (SoundManager.instance)
                 SoundManager.instance.PlayClip(ClipName.Jump);
            Runner.StartCoroutine(Jump());
        }
        if (collision.gameObject.CompareTag("Finish"))
        {
           // StartCoroutine(delay());
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
            //ResetPos();
            if (HasInputAuthority)
                RPC_RequestReset();
        }
        if (other.gameObject.CompareTag("Coin"))
        {
            SoundManager.instance.PlayClip(ClipName.Coin);
            // slashAnim = true;
            //AddCoin();
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("Enemy_Shot"))
        {
            TakeDamage(5);
        }

        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            if (SoundManager.instance)
                SoundManager.instance.PlayClip(ClipName.Jump);

            Runner.StartCoroutine(Jump());
        }



        //if (other.gameObject.TryGetComponent<PowerUp>(out PowerUp pu))
        //{

        //    SoundManager.instance.PlayClip(ClipName.PowerUp);

        //    UIControllerBtns.UpdateIcon(pu.powerUp);
        //    UIControllerBtns.PowerButtonInteractbale(true);
        //    pu.ChoosePowerUps();
        //}

    }

    public void TrackProgress()
    {
        if (finishLine == null)
        {
            finishLine = networkManager.finish;
        }
        else
        {
            float distance = Vector3.Distance(transform.position, finishLine.position);
            ProgressToFinish = distance;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    public void ResetPos()
    {
        accumulatedRotationDelta = resetTarget.eulerAngles.y;
        transform.position = resetTarget.localPosition;
        transform.rotation = resetTarget.localRotation;


        
     

        SoundManager.instance.PlayClip(ClipName.Spawn);
    }



    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestReset()
    {
        DoResetPos();
    }



    private void DoResetPos()
    {
        accumulatedRotationDelta = resetTarget.eulerAngles.y;
        transform.position = resetTarget.position;
        transform.rotation = resetTarget.rotation;

        SoundManager.instance.PlayClip(ClipName.Spawn);
    }



}
