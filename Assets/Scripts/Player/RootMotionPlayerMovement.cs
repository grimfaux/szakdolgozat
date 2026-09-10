using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class RootMotionPlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float initMoveSpeed = 5f;
    public float initRunSpeed = 10f;
    public float rotationSpeed = 10f;
    public float gravity = 9.81f;

    [Header("GroundCheck")]
    public float playerHeight = 2f;
    public LayerMask whatIsGround;

    [Header("References")]
    public Transform orientation;
    public Animator playerAnimator;

    private CharacterController _controller;
    private PlayerControls _controls;
    private PlayerInfo _playerInfo;

    private Vector2 _moveInput;
    private Vector3 _moveDirection;
    private Vector3 _velocity;

    private bool _isRunning;
    private bool _isDashing;
    private float moveSpeed;

    private bool _grounded;

    private void Awake() {
        _controller = GetComponent<CharacterController>();
        playerAnimator = GetComponent<Animator>();
        _playerInfo = GetComponent<PlayerInfo>();
        _controls = new PlayerControls();

        moveSpeed = initMoveSpeed;
    }

    private void OnEnable() {
        _controls.Enable();
        _controls.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _controls.Player.Move.canceled += ctx => _moveInput = Vector2.zero;
        _controls.Player.Run.performed += OnRunPressed;
        _controls.Player.Run.canceled += OnRunReleased;
        _controls.Player.Dash.performed += OnDashPressed;
    }

    private void OnDisable() {
        _controls.Disable();
    }

    private void Update() {
        CheckGrounded();
        ApplyGravity();
        HandlePlayerInfo();
        HandleMovement();

        playerAnimator.SetFloat("speed", _moveDirection.magnitude * moveSpeed);
    }

    #region Input Callbacks
    private void OnRunPressed(InputAction.CallbackContext context) {
        if (_playerInfo.Stamina > 0)
            _isRunning = true;
    }

    private void OnRunReleased(InputAction.CallbackContext context) {
        _isRunning = false;
    }

    private void OnDashPressed(InputAction.CallbackContext context) {
        if (_playerInfo.Stamina > 0)
            StartCoroutine(DashRoutine());
    }
    #endregion

    #region Movement
    private void HandleMovement() {
        if (playerAnimator.GetBool("isAttacking")) {
            _moveDirection = Vector3.zero;
            return;
        }

        _moveDirection = orientation.forward * _moveInput.y + orientation.right * _moveInput.x;

        moveSpeed = _isRunning ? initRunSpeed : initMoveSpeed;

        _controller.Move(_moveDirection.normalized * (moveSpeed * Time.deltaTime));

        if (_moveDirection != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(_moveDirection.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnAnimatorMove() {
        if (playerAnimator.GetBool("isAttacking")) {
            Vector3 rootMotion = playerAnimator.deltaPosition;
            rootMotion.y = 0;
            _controller.Move(rootMotion);

            transform.rotation *= playerAnimator.deltaRotation;
        }
    }
    #endregion

    #region Gravity & Ground
    private void CheckGrounded() {
        _grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
    }

    private void ApplyGravity() {
        if (_grounded && _velocity.y < 0)
            _velocity.y = -2f;

        _velocity.y -= gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }
    #endregion

    #region Player Info (Stamina)
    private void HandlePlayerInfo() {
        if (_isRunning) {
            _playerInfo.DecreaseStamina(5f * Time.deltaTime);
            if (_playerInfo.Stamina <= 0)
                _isRunning = false;
        }
        else {
            _playerInfo.RegenStamina(5f * Time.deltaTime);
        }
    }

    private IEnumerator DashRoutine() {
        _isDashing = true;
        _playerInfo.DecreaseStamina(40f);
        yield return new WaitForSeconds(0.2f);
        _isDashing = false;
    }
    #endregion
}

