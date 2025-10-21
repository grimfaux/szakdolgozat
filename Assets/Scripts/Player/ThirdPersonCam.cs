using UnityEngine;

public class ThirdPersonCam : MonoBehaviour {

    [Header("References")] public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public CharacterController playerController;

    public float rotationSpeed;
    
    private PlayerControls controls;
    private Vector2 moveInput;

    private void Awake() {
        controls = new PlayerControls();
    }
    
    private void OnEnable() {
        controls.Enable();
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    private void OnDisable() {
        controls.Player.Move.performed -= ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled -= ctx => moveInput = Vector2.zero;
        controls.Disable();
    }
    
    
    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void Update() {
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;
        
        float horizontalInput = moveInput.x;
        float verticalInput = moveInput.y;
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (inputDir != Vector3.zero) {
            playerObj.forward = Vector3.Lerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }
    }
}
