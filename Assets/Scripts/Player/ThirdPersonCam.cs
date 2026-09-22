using UnityEngine;

/// Keeps the orientation transform pointing from the camera towards the player.
/// Character rotation belongs to RootMotionPlayerMovement, not here.
public class ThirdPersonCam : MonoBehaviour {
    [Header("References")]
    public Transform orientation;
    public Transform player;

    [Header("Cursor")]
    [SerializeField] private bool lockCursor = true;

    private void Start() {
        if (lockCursor) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // LateUpdate so the orientation reflects where the camera actually ended up.
    private void LateUpdate() {
        if (player == null || orientation == null) return;

        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        if (viewDir.sqrMagnitude > 0.0001f) {
            orientation.forward = viewDir.normalized;
        }
    }
}
