using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInfo))]
public class RootMotionPlayerMovement : MonoBehaviour {
    [Header("Movement Settings")]
    public float initMoveSpeed = 5f;
    public float initRunSpeed = 10f;
    public float rotationSpeed = 10f;
    public float gravity = 9.81f;

    [Header("Stamina")]
    public float runStaminaPerSecond = 5f;
    public float staminaRegenPerSecond = 5f;

    [Header("Dodge")]
    public float dodgeDistance = 4f;
    public float dodgeDuration = 0.25f;
    public float dodgeStaminaCost = 25f;
    [Tooltip("How long the player ignores damage after the dodge starts.")]
    public float dodgeInvulnerability = 0.35f;
    [Tooltip("Let a root motion dodge clip drive the movement instead of a manual slide.")]
    public bool useRootMotionForDodge;

    [Header("References")]
    public Transform orientation;
    public Animator playerAnimator;

    private CharacterController _controller;
    private PlayerInfo _playerInfo;

    private Vector2 _moveInput;
    private Vector3 _moveDirection;
    private float _inputMagnitude;
    private float _verticalVelocity;
    private float _currentSpeed;

    private readonly HashSet<int> _triggers = new HashSet<int>();
    private bool _isRunning;
    private float _dodgeTimeLeft;
    private Vector3 _dodgeDirection;

    public bool IsDodging => _dodgeTimeLeft > 0f;
    private bool IsAttacking => playerAnimator.GetBool(PlayerAnimatorParams.IsAttacking);

    private void Awake() {
        _controller = GetComponent<CharacterController>();
        _playerInfo = GetComponent<PlayerInfo>();
        if (playerAnimator == null) playerAnimator = GetComponentInChildren<Animator>();

        if (playerAnimator != null && playerAnimator.runtimeAnimatorController != null) {
            foreach (AnimatorControllerParameter p in playerAnimator.parameters) {
                if (p.type == AnimatorControllerParameterType.Trigger) _triggers.Add(p.nameHash);
            }
        }
    }

    private void OnEnable() {
        PlayerInputs.Acquire();
        PlayerControls.PlayerActions player = PlayerInputs.Controls.Player;
        player.Move.performed += OnMove;
        player.Move.canceled += OnMove;
        player.Run.performed += OnRunPressed;
        player.Run.canceled += OnRunReleased;
        player.Dash.performed += OnDashPressed;
    }

    private void OnDisable() {
        PlayerControls.PlayerActions player = PlayerInputs.Controls.Player;
        player.Move.performed -= OnMove;
        player.Move.canceled -= OnMove;
        player.Run.performed -= OnRunPressed;
        player.Run.canceled -= OnRunReleased;
        player.Dash.performed -= OnDashPressed;
        PlayerInputs.Release();
    }

    private void Update() {
        float dt = Time.deltaTime;

        ReadMoveDirection();
        UpdateStamina(dt);

        Vector3 motion = Vector3.zero;

        if (IsDodging) {
            _dodgeTimeLeft -= dt;
            _currentSpeed = 0f;
            if (!useRootMotionForDodge && dodgeDuration > 0f) {
                motion = _dodgeDirection * (dodgeDistance / dodgeDuration);
            }
        } else if (IsAttacking) {
            _currentSpeed = 0f;
        } else {
            _currentSpeed = _isRunning ? initRunSpeed : initMoveSpeed;
            motion = _moveDirection * (_currentSpeed * _inputMagnitude);
            RotateTowardsMovement(dt);
        }

        ApplyGravity(dt);
        motion.y = _verticalVelocity;

        // One Move per frame keeps collision resolution predictable.
        _controller.Move(motion * dt);

        playerAnimator.SetFloat(PlayerAnimatorParams.Speed, _currentSpeed * _inputMagnitude);
    }

    #region Input Callbacks
    private void OnMove(InputAction.CallbackContext context) {
        _moveInput = context.ReadValue<Vector2>();
    }

    private void OnRunPressed(InputAction.CallbackContext context) {
        if (_playerInfo.Stamina > 0f) _isRunning = true;
    }

    private void OnRunReleased(InputAction.CallbackContext context) {
        _isRunning = false;
    }

    private void OnDashPressed(InputAction.CallbackContext context) {
        if (IsDodging || IsAttacking) return;
        if (!_playerInfo.TrySpendStamina(dodgeStaminaCost)) return;

        _dodgeDirection = _moveDirection != Vector3.zero ? _moveDirection : transform.forward;
        _dodgeDirection.y = 0f;
        _dodgeDirection.Normalize();
        transform.rotation = Quaternion.LookRotation(_dodgeDirection);

        _dodgeTimeLeft = dodgeDuration;
        _playerInfo.GrantInvulnerability(dodgeInvulnerability);
        SetTriggerIfPresent(PlayerAnimatorParams.Dodge);
    }
    #endregion

    #region Movement
    private void ReadMoveDirection() {
        Vector3 raw = orientation.forward * _moveInput.y + orientation.right * _moveInput.x;
        raw.y = 0f;

        _inputMagnitude = Mathf.Clamp01(raw.magnitude);
        _moveDirection = raw.sqrMagnitude > 0.0001f ? raw.normalized : Vector3.zero;
    }

    private void RotateTowardsMovement(float dt) {
        if (_moveDirection == Vector3.zero) return;

        Quaternion target = Quaternion.LookRotation(_moveDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed * dt);
    }

    private void OnAnimatorMove() {
        if (!IsAttacking && !(IsDodging && useRootMotionForDodge)) return;

        Vector3 rootMotion = playerAnimator.deltaPosition;
        rootMotion.y = 0f;
        _controller.Move(rootMotion);

        transform.rotation *= playerAnimator.deltaRotation;
    }
    #endregion

    #region Gravity
    private void ApplyGravity(float dt) {
        if (_controller.isGrounded && _verticalVelocity < 0f) {
            _verticalVelocity = -2f;
        } else {
            _verticalVelocity -= gravity * dt;
        }
    }
    #endregion

    #region Stamina
    private void UpdateStamina(float dt) {
        bool sprinting = _isRunning && _inputMagnitude > 0.1f && !IsDodging;

        if (sprinting) {
            _playerInfo.DecreaseStamina(runStaminaPerSecond * dt);
            if (_playerInfo.Stamina <= 0f) _isRunning = false;
        } else {
            _playerInfo.RegenStamina(staminaRegenPerSecond * dt);
        }
    }
    #endregion

    // The animator has no dodge parameter yet; setting an unknown one spams the console.
    private void SetTriggerIfPresent(int hash) {
        if (_triggers.Contains(hash)) playerAnimator.SetTrigger(hash);
    }
}
