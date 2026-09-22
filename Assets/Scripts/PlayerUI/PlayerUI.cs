using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {
    [Header("UI")]
    public Slider healthBar;
    public Slider staminaBar;

    [SerializeField] private PlayerInfo playerInfo;

    private void Awake() {
        if (playerInfo == null) {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null) playerInfo = playerObject.GetComponent<PlayerInfo>();
        }

        if (playerInfo == null) {
            Debug.LogError("PlayerUI found no PlayerInfo, disabling the bars.", this);
            enabled = false;
            return;
        }

        // Maxima only change if the player levels up, so they do not belong in Update.
        if (healthBar != null) healthBar.maxValue = playerInfo.maxHealth;
        if (staminaBar != null) staminaBar.maxValue = playerInfo.maxStamina;
    }

    private void Update() {
        if (healthBar != null) healthBar.value = playerInfo.Health;
        if (staminaBar != null) staminaBar.value = playerInfo.Stamina;
    }
}
