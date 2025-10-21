using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMovement : MonoBehaviour {
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float gravity = 9.81f;
    

    [Header("GroundCheck")]
    public float playerHeight;
    public LayerMask whatIsGround;
    private bool _grounded;
    
    public Transform orientation;
    
    private Vector3 _moveDirection;
    private Vector3 _velocity;
    private CharacterController _controller;
    private PlayerControls _controls;
    private Vector2 _moveInput;

    private void Awake() {
        _controller = GetComponent<CharacterController>();
        _controls = new PlayerControls();
    }

    private void OnEnable() {
        _controls.Enable();
        _controls.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _controls.Player.Move.canceled += ctx => _moveInput = Vector2.zero;
    }

    private void OnDisable() {
        _controls.Disable();
    }

    private void Update() {
        _grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        HandleMovement();
        ApplyGravity();
    }

    private void ApplyGravity() {
        if (_grounded && _velocity.y < 0) 
            _velocity.y = -2f;
        
        _velocity.y -= gravity * Time.deltaTime;
        
        _controller.Move(_velocity * Time.deltaTime);
    }

    private void HandleMovement() {
        Vector3 moveDir = orientation.forward * _moveInput.y + orientation.right * _moveInput.x;

        _controller.Move(moveDir.normalized * (moveSpeed * Time.deltaTime));

        if (moveDir != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}


