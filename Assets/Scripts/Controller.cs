//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Advertisements;
//using UnityEngine.Events;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;
//using CatharGames.AudioSystem;
//using CatharGames.Pilgrim;
//using Multiplayer.Events;


//public class Controller : MonoBehaviour, IController, IPlayerNetworkIdRequired
//{

//	[SerializeField] private GameSettings _Settings;
//	[SerializeField] private CharacterEvent _CharacterEvent;
//	[SerializeField] private CharacterEvent _ReplayAfterCompletion;
//	[SerializeField] AdsSystem _AdsSystem;
//	[SerializeField] int _NumberRetryBeforeAds;
//	[SerializeField] PickupSettings _PickupSettings;

//	[SerializeField] GameplayElementsEvents _GameplayElementEvents;

//	[SerializeField] private float _ResetVelocity;
//	private bool _UseJoystick;
//#if UNITY_EDITOR
//	[SerializeField] private Text testText;
//#endif

//	#region Attributes
//	[Header("Configuration")]
//	[Tooltip("How fast the character speeds up in the start area.")]
//	public float startAccelerationAmount = .66f;

//	[Tooltip("How fast the character speeds up when moving downhill.")]
//	public float accelerationAmount = 1f;

//	[Tooltip("Controls whether the character needs to be touching a ramp to gain downhill speed.")]
//	public bool accelerateOnRampsOnly;

//	[Tooltip("Controls the curve of how sensitive the joystick is")]
//	[Range(0f, 10f)] public float joystickSensitivity = 1f;

//	[Tooltip("Controls the curve of how much rotation is applied based on joystick position. (See 'Sean Task' google doc for details")]
//	[Range(1f, 10f)] public float joystickExponent = 2f;

//	[Tooltip("Fastest speed the character can obtain.")]
//	public float maxVelocity = 100f;

//	[Tooltip("Multiplier for speed boost given when the character lands on a ramp.")]
//	public float landingBoostPercent = 1f;

//	[Tooltip("Number of times the player can perform a swipe boost per jump.")]
//	public int maxBoostsPerJump = 3;

//	[Tooltip("The percent of the width screen that the swipe gesture should cover to give the optimal swipe boost.")]
//	public float idealSwipeBoostWidth = .5f;

//	[Tooltip("The percent of the width screen that the swipe gesture should cover to give the optimal swipe boost.")]
//	public float swipeBoostWidthTolerance = .5f;

//	[Tooltip("The amount of boost applied when the optimal swipe boost is performed")]
//	public float maxSwipeBoost = 10f;

//	[Tooltip("Controls whether the character needs to be touching a ramp to automatically accelerate when below threshold.")]
//	public bool autoAccelerateOnRampsOnly = false;

//	[Tooltip("If velocity is less than this ammount, the character will automatically accelerate.")]
//	public float autoAccelerateThreshold = 1f;

//	[Tooltip("How quickly the character speeds up until back at the threshold.")]
//	public float autoAccelerateAmount = 1f;

//	[Tooltip("Number of seconds after dropping below the threshold until the character begins to auto-accelerate.")]
//	public float autoAccelerateDelay = 0f;

//	[Tooltip("Will prevent speed from dropping too much when the player is on a ramp.")]
//	public bool limitDecelerationOnRamps;
//	[Tooltip("When above is enabled: The max allowable loss of speed per second as a percentage.  Set to 0 to not allow any loss of speed.")]
//	[Range(0f, 1f)] public float decelerationLimit = .1f;


//	private NetworkPlayerInfo _LocalPlayerInfo;

//	private Coroutine _MaxVelocityResetTimer;
//	[Tooltip("When this is unchecked, character will hop force will be applied in the direction of the bhop platform if it is tilted")]
//	public bool alwaysHopStraightUp;

//	[Tooltip("How high do bhops make you jump")]
//	public float bhopForce = 10f;
//#if UNITY_EDITOR

//	[Tooltip("Sensitivity of the arrow keys for rotation")]
//	public float arrowRotationSpeed = 1f;
//#endif

//	[Header("Sending Events")]
//	[SerializeField] UnityEvent m_OnBoostTrigger;
//	#endregion

//	#region Private Fields
//	private const float ANGLE_THRESHOLD = 45f;//The max allowable angle difference between player and bhop platform to perform a hop

//	private Vector2 lastPressPos;
//	private Rigidbody rigidBody;
//	private bool touching;
//	private bool prevTouching;
//	public bool inStartArea = false;
//	private float? previousYPos = null;
//	private enum AutoAccState { NONE, WAITING, ACCELERATING };
//	private AutoAccState autoAccState = AutoAccState.NONE;
//	private ExtendedTouching extendedTouching;

//	private Vector3 startPos;
//	private Quaternion startRot;

//	private const float SWIPE_BEGIN_THRESHOLD = .15f;
//	private const float SWIPE_MAX_TIME = .5f;
//	private const float SWIPE_COOLDOWN_TIME = .1f;
//	private int swipeBoostCounter;
//	private float? initialSwipeBoostX;
//	private bool swipeReturn, swipeReturnToRight, onSwipeCooldown;
//	private float swipeBoostToApply;
//	private float swipeOffset, swipeBoostPercentage;//temporarily made these global for debugging
//	private float touchRotationSensitivity = .01f;
//	private float joystickSensitivity = .01f;
//	private float prevMagnitude;
//	private bool applyPickupBoost;

//	private VariableJoystick joystick;
//	private int _CurrentRetryTime = 0;

//	private int _InboundsTouchPoints = 0;
//	private float initialMaxVelocity;

//	values used when the player grabs a powerUp

//	 public int BoostSpeed { set; get; }
//	public float Timer { set; get; }
//	private float currentCDtime;
//	public bool isOnCD;


//	[Header("UnityEvent")]
//	[SerializeField] private UnityEvent _OnStartAreaExit;
//	[SerializeField] public UnityEvent _OnResetCharacter;

//	[Header("Audio")]
//	[SerializeField] private SfxManager _SfxManager;
//	[SerializeField] private AudioClipInfo _OnDeathSfx;
//	[SerializeField] private AudioClipInfo _OnDeathFromOtherSfx;

//	public bool IsInStartArea { get { return inStartArea; } }
//	public float MaxVelocity { get { return maxVelocity; } }
//	#endregion

//	#region Monobehaviour Methods
//	void Awake()
//	{
//		if (rigidBody == null)
//			rigidBody = GetComponent<Rigidbody>();

//		if (joystick == null)
//			joystick = FindObjectOfType<VariableJoystick>();

//		if (extendedTouching == null)
//			extendedTouching = GetComponentInChildren<ExtendedTouching>();
//	}

//	void Start()
//	{
//		_AdsSystem.Initialize();
//		_ReplayAfterCompletion.RegisterListener(OnReplayAfterCompletion);
//		UpdateSensibility();
//		Debug.Log("joystickSensitivity: " + joystickSensitivity);

//		RefreshController();
//		initialMaxVelocity = maxVelocity;

//		_GameplayElementEvents.RegisterListener<GameplayElementsEvents.OnLocalProjectileTouchOtherPlayer>(
//			OnLocalProjectileTouchOtherPlayer);
//	}

//	private void OnLocalProjectileTouchOtherPlayer(GameplayElementsEvents.OnLocalProjectileTouchOtherPlayer e)
//	{
//		TODO Add validation if there is abuse here
//		if (e.PlayerIdTouched == _LocalPlayerInfo.UniqueId)
//			{
//				RespawnCharacter(CharacterDeathType.Projectile);
//			}
//	}

//	public void UpdateSensibility()
//	{
//		touchRotationSensitivity = _Settings.SensibilityScreen;
//		joystickSensitivity = _Settings.SensibilityJoystick;
//	}

//	public void RefreshController()
//	{
//		if (joystick == null)
//		{
//			joystick = FindObjectOfType<VariableJoystick>();
//		}
//		_UseJoystick = joystick != null && (_Settings.ControllerType == ControllerType.Joystick);

//		if (joystick != null)
//		{
//			joystick.gameObject.SetActive(_UseJoystick);
//		}
//	}

//	private void OnReplayAfterCompletion()
//	{
//		_ReplayAfterCompletion.UnregisterListener(OnReplayAfterCompletion);
//		string sceneName = SceneManager.GetActiveScene().name;
//		SceneManager.LoadScene(sceneName);
//	}

//	public void ActivateScript()
//	{
//		enabled = true;
//	}

//	private void OnEnable()
//	{
//		_CharacterEvent.RegisterListener(ResetPosition);
//	}
//	private void OnDisable()
//	{
//		_CharacterEvent.UnregisterListener(ResetPosition);
//	}

//	void FixedUpdate()
//	{
//		if (IsInStartArea)
//		{
//			return;
//		}

//		bool down = false;
//		float xPos;

//		if (_UseJoystick)
//		{
//			down = joystick.Down;
//			xPos = joystick.Horizontal;
//		}
//		else
//		{
//			down = Input.GetMouseButton(0);
//			if (down)
//			{
//				xPos = Input.mousePosition.x;
//			}
//			else
//			{
//				xPos = (Screen.width / 2f);
//			}
//		}

//		ProcessPress(new Vector2(xPos, 0));
//		if (!down)
//		 else
//				{
//					initialSwipeBoostX = null;
//					swipeReturn = false;
//				}


//#if UNITY_EDITOR
//		Debug.Log(touching ? "touching" : "not touching");
//		if (testText != null)
//			testText.text = string.Format("touching: {0}, et: {1}", touching, extendedTouching.IsExtendedTouching);
//		if (Input.GetKey("left"))
//		{
//			Rotate(-arrowRotationSpeed);
//		}
//		if (Input.GetKey("right"))
//		{
//			Rotate(arrowRotationSpeed);
//		}
//		if (Input.GetKeyDown(KeyCode.R))
//			ResetPosition();
//#endif

//		ModifyVelocity();
//		ResetBoostedVelocity();
//	}

//	public void SetStart(Transform position)
//	{
//		startPos = position.position;
//		startRot = position.rotation;
//	}

//	private void OnCollisionEnter(Collision collision)
//	{
//		touching = true;
//		swipeBoostCounter = 0;
//		ApplyLandingBoost();

//		RespawnPoint spawnPoint = collision.gameObject.GetComponentInChildren<RespawnPoint>();
//		if (spawnPoint != null)
//		{
//			startPos = spawnPoint.Position;
//			startRot = spawnPoint.Rotation;

//			PlayerRankingSystem.Instance.Local_TouchRespawnPoint(_LocalPlayerInfo.UniqueId, spawnPoint);
//		}

//		Vector3 validDirection = Vector3.up;
//		if (collision.gameObject.tag == "bhop")
//		{
//			for (int k = 0; k < collision.contacts.Length; k++)
//			{
//				Debug.Log("collision.contacts[k].normal: " + collision.contacts[k].normal);
//				float angleBetween = Vector3.Angle(collision.contacts[k].normal, validDirection);
//				Debug.Log("angleBetween: " + angleBetween);

//				if (angleBetween <= ANGLE_THRESHOLD)
//				{
//					Debug.Log("hopping collision " + collision.gameObject.name);
//					Hop(collision.transform.forward);
//					break;
//				}
//			}
//		}
//	}

//	private void OnCollisionExit(Collision collision)
//	{
//		touching = false;
//		previousYPos = null;
//		initialSwipeBoostX = null;
//		swipeReturn = false;
//	}
//	#endregion

//	#region Private Methods
//	private void ResetBoostedVelocity()
//	{
//		if (isOnCD)
//		{
//			currentCDtime += 1 / Timer * Time.deltaTime;

//			if (currentCDtime >= 1)
//			{
//				BoostSpeed = 0;
//				currentCDtime = 0;
//				isOnCD = false;
//			}
//		}
//	}

//	private void ApplyLandingBoost()
//	{
//		float yVelocity = rigidBody.velocity.y;
//		BoostForwardVelocity(-yVelocity * landingBoostPercent);
//		Debug.Log("applying landing boost: " + yVelocity);

//		m_OnBoostTrigger.Invoke();

//	}

//	private void BoostForwardVelocity(float ammount)
//	{
//		Vector3 velocity = rigidBody.velocity;
//		velocity.z += ammount;
//		rigidBody.velocity = velocity;
//	}

//	set velocity to go in the direction that the character is facing
//	also accelerate if under velocity threshold
//	private void ModifyVelocity()
//	{
//		Vector2 twoDVel = new Vector2(rigidBody.velocity.x, rigidBody.velocity.z);
//		float magnitude = twoDVel.magnitude;

//		if (!inStartArea)
//		{
//			if ((!autoAccelerateOnRampsOnly || touching) && magnitude < autoAccelerateThreshold)
//			{
//				if (autoAccState == AutoAccState.NONE)
//				{
//					StopCoroutine("AutoAccelerateWait");
//					StartCoroutine("AutoAccelerateWait");
//				}
//				else if (autoAccState == AutoAccState.ACCELERATING)
//				{
//					Debug.Log("below threshold. velocity: " + magnitude);
//					magnitude += autoAccelerateAmount;
//				}
//			}
//			else
//			{
//				autoAccState = AutoAccState.NONE;
//			}
//			float deltaMagnitude = magnitude - prevMagnitude;   //change in speed this frame
//			float deltaMagnitudePercent = (magnitude - prevMagnitude) / prevMagnitude;  //percent change in speed this frame
//			float deltaMagnitudePercentPerSecond = deltaMagnitudePercent / Time.deltaTime; //percent change in speed per second
//			Debug.Log("deltaMagnitude: " + deltaMagnitude);
//			Debug.Log("deltaMagnitudePercent: " + deltaMagnitudePercent);
//			Debug.Log("deltaMagnitudePercentPerSecond: " + deltaMagnitudePercentPerSecond);
//			float deccelerationRate = deltaMagnitude / Time.deltaTime;
//			Debug.Log("deccelerationRate: " + deccelerationRate);
//			if (limitDecelerationOnRamps && touching && prevTouching && -deltaMagnitudePercentPerSecond > decelerationLimit)
//			{
//				float newMagnitude = prevMagnitude - prevMagnitude * decelerationLimit * Time.deltaTime;
//				Debug.Log("deltaMagnitudePercentPerSecond " + deltaMagnitudePercentPerSecond);
//				Debug.Log(string.Format("preventing deceleration on ramp. prevMagnitude: {0}. magnitude: {1}, setting: {2}", prevMagnitude, magnitude, newMagnitude));
//				magnitude = newMagnitude;
//			}
//			prevMagnitude = magnitude;
//			prevTouching = touching;
//		}
//		Debug.Log("autoAccState: " + autoAccState);

//		Debug.DrawRay(transform.position, transform.forward, Color.red, 0.1f);
//		Debug.DrawRay(transform.position, rb.velocity, Color.yellow, 0.1f);
//		if (applyPickupBoost)
//		{
//			Debug.Log(string.Format("applying pickup boost of {0} previous magnitude: {1}", _PickupSettings.boostAmount, magnitude));
//			magnitude += _PickupSettings.boostAmount;
//			applyPickupBoost = false;
//		}
//		magnitude = Mathf.Min(magnitude, maxVelocity);
//		Debug.Log("magnitude: " + magnitude);
//		Vector3 newVelocity = new Vector3(transform.forward.x, 0, transform.forward.z).normalized * magnitude;
//		newVelocity += new Vector3(0, rigidBody.velocity.y, 0);
//		rigidBody.velocity = newVelocity;
//	}

//	private IEnumerator AutoAccelerateWait()
//	{
//		autoAccState = AutoAccState.WAITING;
//		yield return new WaitForSeconds(autoAccelerateDelay);
//		autoAccState = AutoAccState.ACCELERATING;
//	}

//	void Rotate(float amount)
//	{
//		transform.Rotate(Vector3.up * amount);
//	}

//	void ProcessPress(Vector3 pressPos)
//	{
//		if (!accelerateOnRampsOnly || touching)
//		{
//			if (inStartArea)
//			{
//				Debug.Log("start area force");
//				rigidBody.AddRelativeForce(new Vector3(0, 0, startAccelerationAmount), ForceMode.Impulse);
//			}
//			else
//			{
//				if (previousYPos != null)
//				{
//					float deltaY = (float)previousYPos - transform.position.y;
//					if (deltaY > 0)
//					{
//						rigidBody.AddRelativeForce(new Vector3(0, 0, accelerationAmount * deltaY), ForceMode.Impulse);
//						Debug.Log("adding acceleration. deltaY: " + deltaY);
//					}
//				}
//			}
//			previousYPos = transform.position.y;
//		}

//		ProcessSwipeBoost(pressPos);

//		float distanceFromCenter = Screen.width / 2 - pressPos.x;
//		float turnAmount = 0;
//		if (_UseJoystick)
//		{
//			turnAmount = GetExponentialTurnAmount(pressPos.x);
//		}
//		else
//		{
//			turnAmount = -distanceFromCenter * touchRotationSensitivity;
//		}
//		Rotate(turnAmount);

//		lastPressPos = pressPos;
//	}

//	in air boosting by swiping side to side
//	private void ProcessSwipeBoost(Vector3 pressPos)
//	{
//		Debug.Log("ProcessSwipeBoost " + touching + ", " + initialSwipeBoostX);
//		float xDelta = lastPressPos.x - pressPos.x;
//		if (!touching)//must be in air to do swipe boost
//			if (!onSwipeCooldown)
//			{
//				if (initialSwipeBoostX == null)
//				{
//					if (Mathf.Abs(xDelta) > SWIPE_BEGIN_THRESHOLD)
//					{
//						initialSwipeBoostX = lastPressPos.x;
//						StopCoroutine("TimeSwipeBoost");
//						StartCoroutine("TimeSwipeBoost");
//						Debug.Log("begin swipe boost motion " + xDelta);
//					}
//				}
//				else
//				{
//					if (swipeReturn)
//					{
//						bool applyBoost = false;
//						if (swipeReturnToRight)
//						{
//							if (pressPos.x >= initialSwipeBoostX)
//								applyBoost = true;
//						}
//						else
//						{
//							if (pressPos.x <= initialSwipeBoostX)
//								applyBoost = true;
//						}
//						if (applyBoost)
//						{
//							swipeReturn = false;
//							applyBoost = false;
//							initialSwipeBoostX = null;
//							if (swipeBoostCounter < maxBoostsPerJump)
//							{
//								Debug.Log(string.Format("Swipe was off by {0}. Applying {1}% boost. Boost amount is {2}", swipeOffset, Mathf.RoundToInt(swipeBoostPercentage * 100), swipeBoostToApply));
//								BoostForwardVelocity(swipeBoostToApply);
//								StartCoroutine("SwipeCooldown");
//								swipeBoostCounter++;
//							}
//						}
//					}
//					else
//					{
//						float xBoostDelta = (float)initialSwipeBoostX - pressPos.x;
//						float swipeDistance = 0;
//						if (xBoostDelta > 0 && xDelta < 0)//changed directions after heading left
//						{
//							Debug.Log("swipe return to right");
//							swipeReturn = true;
//							swipeReturnToRight = true;
//							swipeDistance = Mathf.Abs(xBoostDelta);
//						}
//						if (xBoostDelta < 0 && xDelta > 0)//changed directions after heading right
//						{
//							Debug.Log("swipe return to left");
//							swipeReturn = true;
//							swipeReturnToRight = false;
//							swipeDistance = Mathf.Abs(xBoostDelta);
//						}
//						if (swipeReturn)
//						{
//							calculate amount of boost to apply upon completing side to side swipe
//	                        /*float*/
//								  swipeOffset = Mathf.Abs(swipeDistance - idealSwipeBoostWidth);//TODO: stop using global variable
//							/*float*/
//							swipeBoostPercentage = 1 - Mathf.Clamp01(swipeOffset / swipeBoostWidthTolerance);//TODO: stop using global variable

//							swipeBoostToApply = maxSwipeBoost * swipeBoostPercentage;
//							Debug.Log("boostToApply: " + swipeBoostToApply);
//						}
//					}
//				}
//			}
//	}

//	private IEnumerator TimeSwipeBoost()
//	{
//		yield return new WaitForSeconds(SWIPE_MAX_TIME);
//		swipeReturn = false;
//		initialSwipeBoostX = null;
//	}

//	private IEnumerator SwipeCooldown()
//	{
//		onSwipeCooldown = true;
//		yield return new WaitForSeconds(SWIPE_COOLDOWN_TIME);
//		onSwipeCooldown = false;
//	}

//	void OnTriggerStay(Collider other)
//	{
//		if (other.CompareTag("StartArea"))
//			inStartArea = true;
//	}

//	private void OnTriggerExit(Collider other)
//	{
//		if (other.CompareTag("StartArea"))
//		{
//			inStartArea = false;
//			_OnStartAreaExit.Invoke();
//		}

//		if (other.CompareTag("Extended"))
//		{
//			Debug.Log("extended exit");
//		}

//		if (other.CompareTag("Respawn"))
//		{
//			if (--_InboundsTouchPoints <= 0)
//			{
//				RespawnCharacter(CharacterDeathType.Respawn);
//			}
//		}
//	}

//	private void RespawnCharacter(CharacterDeathType type)
//	{
//		switch (type)
//		{
//			case CharacterDeathType.Unknown:
//			case CharacterDeathType.Respawn:
//				_SfxManager.PlaySfx(_OnDeathSfx);
//				break;
//			case CharacterDeathType.Projectile:
//				_SfxManager.PlaySfx(_OnDeathFromOtherSfx);
//				break;
//			default:
//				_SfxManager.PlaySfx(_OnDeathSfx);
//				Debug.LogError($"RespawnCharacter :: Not supported, default used :: {type}");
//				break;
//		}
//		_CharacterEvent?.Raise();
//	}

//	private void OnTriggerEnter(Collider other)
//	{
//		RespawnPoint spawnPoint = other.gameObject.GetComponentInChildren<RespawnPoint>();
//		if (spawnPoint != null)
//		{
//			startPos = spawnPoint.Position;
//			startRot = spawnPoint.Rotation;
//			PlayerRankingSystem.Instance.Local_TouchRespawnPoint(_LocalPlayerInfo.UniqueId, spawnPoint);
//		}

//		if (other.CompareTag("Extended"))
//		{
//			Debug.Log("extended enter");
//		}

//		if (other.CompareTag("Respawn"))
//		{
//			_InboundsTouchPoints++;
//			if (_InboundsTouchPoints <= 0)
//				_InboundsTouchPoints = 1; // Failsafe to avoid loop respawning the player...
//		}

//		if (other.CompareTag("RespawnOnTouch"))
//		{
//			RespawnCharacter(CharacterDeathType.Respawn);
//		}

//		Projectile projectile = other.gameObject.GetComponentInParent<Projectile>();
//		if (projectile != null)
//		{
//			if (projectile.CheckIfRealCollision(_LocalPlayerInfo))
//			{
//				projectile.ConfirmCollision();
//				RespawnCharacter(CharacterDeathType.Projectile);
//			}
//		}
//	}

//	public void ResetPosition()
//	{
//		_CurrentRetryTime++;
//		if ((_NumberRetryBeforeAds >= 0) && (_CurrentRetryTime >= _NumberRetryBeforeAds))
//		{
//			_CurrentRetryTime = 0;
//			Advertisement.AddListener(this);
//			_AdsSystem.ShowRewardAds();
//		}
//		else
//		{
//			InternalResetPosition();
//		}
//	}

//	private void InternalResetPosition()
//	{
//		transform.position = startPos;
//		transform.rotation = startRot;
//		rigidBody.velocity = rigidBody.velocity.normalized * _ResetVelocity;
//		prevTouching = false;
//		StopCoroutineResetMaxVelocity();
//		ResetMaxVelocity();
//		_OnResetCharacter.Invoke();
//	}

//	private void ResetMaxVelocity()
//	{
//		maxVelocity = initialMaxVelocity;
//	}

//	private float GetExponentialTurnAmount(float joystickX)
//	{
//		float x = joystickSensitivity * Mathf.Pow(Mathf.Abs(joystickX), joystickExponent);
//		if (joystickX < 0)
//			x *= -1;
//		return x;
//	}

//	private void Hop(Vector3 hopDirection)
//	{
//		if (alwaysHopStraightUp)
//			rigidBody.AddForce(new Vector3(0, bhopForce, 0), ForceMode.VelocityChange);
//		else
//		{
//			rigidBody.AddForce(hopDirection.normalized * bhopForce, ForceMode.VelocityChange);
//			Debug.Log(hopDirection);
//		}
//	}


//	public void OnUnityAdsReady(string placementId)
//	{
//	}

//	public void OnUnityAdsDidError(string message)
//	{
//		_CurrentRetryTime = 0;
//		InternalResetPosition();
//	}

//	public void OnUnityAdsDidStart(string placementId)
//	{
//	}

//	public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
//	{
//		_CurrentRetryTime = 0;
//		InternalResetPosition();
//	}

//	Pickup Behaviour
//	private void ApplyPickupBoost()
//	{
//		applyPickupBoost = true;
//	}

//	private void RaiseMaxVelocity()
//	{
//		maxVelocity += _PickupSettings.maxSpeedRaise;

//		StopCoroutineResetMaxVelocity();
//		_MaxVelocityResetTimer = StartCoroutine(ResetMaxVelocityTimer(_PickupSettings.boostMaxVelocityTime));
//	}

//	void StopCoroutineResetMaxVelocity()
//	{
//		if (_MaxVelocityResetTimer != null)
//		{
//			StopCoroutine(_MaxVelocityResetTimer);
//			_MaxVelocityResetTimer = null;
//		}
//	}
//	IEnumerator ResetMaxVelocityTimer(float timeBeforeReset)
//	{
//		yield return new WaitForSeconds(timeBeforeReset);

//		ResetMaxVelocity();
//		_MaxVelocityResetTimer = null;
//	}

//	end Pickup Behaviour
//	#endregion

//	public void ApplyBoostPickup()
//	{
//		RaiseMaxVelocity();
//		ApplyPickupBoost();
//	}

//	public void SetNetworkPlayerInfo(NetworkPlayerInfo playerInfo)
//	{
//		_LocalPlayerInfo = playerInfo;
//	}

//	public enum CharacterDeathType
//	{
//		Unknown,
//		Respawn,
//		Projectile
//	}
//}
