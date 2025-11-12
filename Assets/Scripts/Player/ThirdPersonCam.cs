using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCam : MonoBehaviour {

    [Header("References")] public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public CharacterController playerController;

    public float rotationSpeed;
    
    private PlayerControls _controls;
    private Vector2 _moveInput;
    private InputAction _moveAction;

    private void Awake() {
        _controls = new PlayerControls();
    }
    
    private void OnEnable() {
        _controls.Enable();
        _moveAction = _controls.Player.Move;
        _moveAction.performed += OnMove;
        _moveAction.canceled += OnMoveCancelled;
    }

    private void OnDisable() {
        _moveAction.performed -= OnMove;
        _moveAction.canceled -= OnMoveCancelled;
        _controls.Disable();
    }

    private void OnMoveCancelled(InputAction.CallbackContext ctx) {
        _moveInput = Vector2.zero;
    }

    private void OnMove(InputAction.CallbackContext ctx) {
        _moveInput = ctx.ReadValue<Vector2>();
    }
    
    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void Update() {
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;
        
        float horizontalInput = _moveInput.x;
        float verticalInput = _moveInput.y;
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (inputDir != Vector3.zero) {
            playerObj.forward = Vector3.Lerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }
    }
}
