using System;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerMoveStatus { NotMoving, Walking, Running, NotGrounded, Landing, Crouching }
public enum CurveControlledBobCallbackType { Horizontal, Vertical }

// Delegates
public delegate void CurveControlledBobCallback();

[Serializable]
public class CurveControlledBobEvent
{
	public float Time = 0.0f;
	public CurveControlledBobCallback Function = null;
	public CurveControlledBobCallbackType Type = CurveControlledBobCallbackType.Vertical;
}

[Serializable]
public class CurveControlledBob
{
	[SerializeField]
	AnimationCurve _bobcurve = new AnimationCurve(
		new Keyframe(0f, 0f), 
		new Keyframe(0.5f, 1f), 
		new Keyframe(1f, 0f), 
		new Keyframe(1.5f, -1f),
		new Keyframe(2f, 0f));

	[SerializeField] float _horizontalMultiplier = 0.01f;
	[SerializeField] float _verticalMultiplier = 0.02f;
	[SerializeField] float _verticaltoHorizontalSpeedRatio = 2.0f;
	[SerializeField] float _baseInterval = 1.0f;

	private float _prevXPlayHead;
	private float _prevYPlayHead;
	private float _xPlayHead;
	private float _yPlayHead;
	private float _curveEndTime;
	private List<CurveControlledBobEvent> _events = new List<CurveControlledBobEvent>();

	public void Initialize()
	{
		// Record time length of bob curve
		_curveEndTime = _bobcurve[_bobcurve.length - 1].time;
		_xPlayHead = 0.0f; 
		_yPlayHead = 0.0f;
	}

	public void RegisterEventCallback(float time, CurveControlledBobCallback function, CurveControlledBobCallbackType type)
	{
		CurveControlledBobEvent ccbeEvent = new CurveControlledBobEvent();
		ccbeEvent.Time = time;
		ccbeEvent.Function = function;
		ccbeEvent.Type = type;
		_events.Add(ccbeEvent);
		_events.Sort((CurveControlledBobEvent t1, CurveControlledBobEvent t2) => t1.Time.CompareTo(t2.Time));
	}

	public Vector3 GetVectorOffset(float speed)
	{
		_xPlayHead += (speed * Time.deltaTime) / _baseInterval;
		_yPlayHead += ((speed * Time.deltaTime) / _baseInterval) * _verticaltoHorizontalSpeedRatio;

		if (_xPlayHead > _curveEndTime)
			_xPlayHead -= _curveEndTime;

		if (_yPlayHead > _curveEndTime) 
			_yPlayHead -= _curveEndTime;

		// Process Events
		for (int i = 0; i < _events.Count; i++)
		{
			CurveControlledBobEvent ev = _events[i];
			if (ev != null)
			{
				if (ev.Type == CurveControlledBobCallbackType.Vertical)
				{
					if ((_prevYPlayHead < ev.Time && ev.Time <= _yPlayHead) ||
						(_prevYPlayHead > _yPlayHead && (_prevYPlayHead < ev.Time || ev.Time <= _yPlayHead)))
					{
						ev.Function();
					}
				}
				else
				{
					if ((_prevXPlayHead < ev.Time && ev.Time <= _xPlayHead) ||
						(_prevXPlayHead > _xPlayHead && (_prevXPlayHead < ev.Time || ev.Time <= _xPlayHead)))
					{
						ev.Function();
					}
				}
			}
		}

		float xPos = _bobcurve.Evaluate(_xPlayHead) * _horizontalMultiplier;
		float yPos = _bobcurve.Evaluate(_yPlayHead) * _verticalMultiplier;

		_prevXPlayHead = _xPlayHead;
		_prevYPlayHead = _yPlayHead;

		return new Vector3(xPos, yPos, 0f);
	}
}

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour {

	[SerializeField] private float _crouchAttenuation = 0.2f;

	[SerializeField] private float _walkSpeed = 2.0f;
	[SerializeField] private float _runSpeed = 4.5f;
	[SerializeField] private float _jumpSpeed = 7.5f;
	[SerializeField] private float _crouchSpeed = 1.0f;
	[SerializeField] private float _stickToGroundForce = 5.0f;
	[SerializeField] private float _gravityMultiplier = 2.5f;
	[SerializeField] private UnityStandardAssets.Characters.FirstPerson.MouseLook _mouseLook = new UnityStandardAssets.Characters.FirstPerson.MouseLook();
	[SerializeField] private CurveControlledBob _headBob = new CurveControlledBob();
	[SerializeField] private float _runStepLengthen = 0.75f;
	[SerializeField] private GameObject _flashLight = null;

    [Header("Shared Variables - Broadcasters")]
    [SerializeField] protected SharedVector3 _broadcastPosition = null;
    [SerializeField] protected SharedVector3 _broadcastDirection = null;
	private Camera _camera = null;
	private bool _jumpButtonPressed = false;
	private Vector2 _inputVector = Vector2.zero;
	private Vector3 _moveDirection = Vector3.zero;
	private bool _previouslyGrounded = true;
	private bool _isWalking = true;
	private bool _isJumping = false;
	private bool _isCrouching = false;
	private Vector3 _localSpaceCameraPos = Vector3.zero;
	private float _controllerHeight = 0;

	//Timer
	private float _fallingTimer = 0f;
    private PlayerMoveStatus _movementStatus = PlayerMoveStatus.NotMoving;
    public CharacterController characterController { get; private set; } = null;

    protected void Start()
	{
		characterController = GetComponent<CharacterController>();
		_controllerHeight = characterController.height;

		_camera = Camera.main;
		_localSpaceCameraPos = _camera.transform.localPosition;

		_movementStatus = PlayerMoveStatus.NotMoving;
		_fallingTimer = 0;

		_mouseLook.Init(transform, _camera.transform);
		_headBob.Initialize();

		if (_flashLight)
			_flashLight.SetActive(false);
	}

	protected void Update()
	{
		_fallingTimer += Time.deltaTime;

		// Allow Mouse Look a chance to process mouse and rotate camera
		if (Time.timeScale > Mathf.Epsilon)
			_mouseLook.LookRotation(transform, _camera.transform);

		if (Input.GetButtonDown("Flashlight"))
		{
			if (_flashLight)
				_flashLight.SetActive(!_flashLight.activeSelf);
		}

		if (!_jumpButtonPressed && !_isCrouching)
			_jumpButtonPressed = Input.GetButtonDown("Jump");

		if (Input.GetButtonDown("Crouch"))
		{
			_isCrouching = !_isCrouching;
			characterController.height = _isCrouching == true ? _controllerHeight / 2 : _controllerHeight;
		}

		if (!_previouslyGrounded && characterController.isGrounded)
		{
			if (_fallingTimer > 0.5f)
			{
                _fallingTimer = 0.0f;
			}

			_moveDirection.y = 0f;
			_isJumping = false;
			_movementStatus = PlayerMoveStatus.Landing;
		}
		else if (!characterController.isGrounded)
		{
			_movementStatus = PlayerMoveStatus.NotGrounded;
		}			
		else if (characterController.velocity.sqrMagnitude < 0.01f)
		{
			_movementStatus = PlayerMoveStatus.NotMoving;
		}			
		else if (_isCrouching)
		{
			_movementStatus = PlayerMoveStatus.Crouching;
		}
		else if (_isWalking)
		{
			_movementStatus = PlayerMoveStatus.Walking;
		}
		else
			_movementStatus = PlayerMoveStatus.Running;

		_previouslyGrounded = characterController.isGrounded;
	}

	protected void FixedUpdate()
	{
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");
		_isWalking = !Input.GetKey(KeyCode.LeftShift);

		float speed = _isCrouching ? _crouchSpeed : _isWalking ? _walkSpeed : _runSpeed;
		_inputVector = new Vector2(horizontal, vertical);

		if (_inputVector.sqrMagnitude > 1) _inputVector.Normalize();

		Vector3 desiredMove = transform.forward * _inputVector.y + transform.right * _inputVector.x;

		// Get a normal for the surface that is being touched to move along it
        if (Physics.SphereCast(transform.position, characterController.radius, Vector3.down, out var hitInfo, characterController.height / 2f, 1))
			desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

		// Scale movement by our current speed (walking value or running value)
		_moveDirection.x = desiredMove.x * speed;
		_moveDirection.z = desiredMove.z * speed;

		// If grounded
		if (characterController.isGrounded)
		{
			// Apply severe down force to keep control sticking to floor
			_moveDirection.y = -_stickToGroundForce;

			// If the jump button was pressed then apply speed in up direction
			// and set isJumping to true. Also, reset jump button status
			if (_jumpButtonPressed)
			{
				_moveDirection.y = _jumpSpeed;
				_jumpButtonPressed = false;
				_isJumping = true;
            }
		}
		else
		{
			// Otherwise we are not on the ground so apply standard system gravity multiplied
			// by our gravity modifier
			_moveDirection += Physics.gravity * _gravityMultiplier * Time.fixedDeltaTime;
		}

		characterController.Move(_moveDirection * Time.fixedDeltaTime);

		Vector3 speedXz = new Vector3(characterController.velocity.x, 0, characterController.velocity.z);
		if (speedXz.magnitude > 0.01f && characterController.isGrounded)
		{
			_camera.transform.localPosition = _localSpaceCameraPos + _headBob.GetVectorOffset(characterController.velocity.magnitude * (_isCrouching || _isWalking ? 1.0f : _runStepLengthen));
		}
		else
			_camera.transform.localPosition = _localSpaceCameraPos;

        // Update broadcasters
        _broadcastPosition.value  = transform.position;
        _broadcastDirection.value = transform.forward;
	}
}