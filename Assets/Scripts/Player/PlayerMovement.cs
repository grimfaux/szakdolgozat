using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMovement : MonoBehaviour {
    [Header("Movement Settings")]
    public float initMoveSpeed = 5f;
    public float initRunSpeed = 10f;
    public float rotationSpeed = 10f;
    public float gravity = 9.81f;
    

    [Header("GroundCheck")]
    public float playerHeight;
    public LayerMask whatIsGround;
    private bool _grounded;
    
    public Transform orientation;
    public float moveSpeed;

    private bool _isDashing;
    private bool _isRunning;
    private Vector3 _moveDirection;
    private Vector3 _velocity;
    [SerializeField] private PlayerInfo _playerInfo;
    private CharacterController _controller;
    private PlayerControls _controls;
    private Vector2 _moveInput;

    private void Awake() {
        _controller = GetComponent<CharacterController>();
        _playerInfo = GetComponent<PlayerInfo>();
        _controls = new PlayerControls();
        moveSpeed = initMoveSpeed;
    }

    private void OnEnable() {
        _controls.Enable();
        _controls.Player.Dash.performed += OnDashPressed;
        _controls.Player.Run.performed += OnRunPressed;
        _controls.Player.Run.canceled += OnRunReleased;
        _controls.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _controls.Player.Move.canceled += ctx => _moveInput = Vector2.zero;
    }

    private void OnDisable() {
        _controls.Disable();
    }

    private void Update() {
        _grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        ApplyGravity();
        HandleMovement();
        HandlePlayerInfo();
    }

    private void OnRunReleased(InputAction.CallbackContext obj) {
        _isRunning = false;
    }

    private void OnRunPressed(InputAction.CallbackContext obj) {
        if (_playerInfo.Stamina <= 0) return;
        _isRunning = true;
    }

    private void OnDashPressed(InputAction.CallbackContext obj) {
        _isDashing = true;
    }

    private void ApplyGravity() {
        if (_grounded && _velocity.y < 0) {
            _velocity.y = -2f;
        }
        
        _velocity.y -= gravity * Time.deltaTime;
        
        _controller.Move(_velocity * Time.deltaTime);
    }
    
    private void HandlePlayerInfo() {
        if (_isRunning) {
            _playerInfo.DecreaseStamina(5f);
            Debug.Log(_playerInfo.Stamina +", "+ Time.deltaTime);
            if (_playerInfo.Stamina <= 0)
                _isRunning = false;
        }

        if (!_isRunning) {
            _playerInfo.RegenStamina(5f);
            Debug.Log(_playerInfo.Stamina +", "+ Time.deltaTime);
        }

        if (_isDashing) {
            _playerInfo.DecreaseStamina(40f);
            StartCoroutine(Wait(2f * Time.deltaTime));
            _isDashing = false;
        }
    }

    private void HandleMovement() {
        Vector3 moveDir = orientation.forward * _moveInput.y + orientation.right * _moveInput.x;

        if (_isRunning)
            moveSpeed = initRunSpeed;
        else if (_isDashing)
            moveSpeed = 30f;
        else
            moveSpeed = initMoveSpeed;

        _controller.Move(moveDir.normalized * (moveSpeed * Time.deltaTime));

        if (moveDir != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    IEnumerator Wait(float waitTime) {
        yield return new WaitForSeconds(waitTime);
    }
}


