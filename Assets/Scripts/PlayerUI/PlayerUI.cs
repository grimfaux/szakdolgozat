using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {
    private PlayerInfo _playerInfo;
    
    [Header("UI")]
    public Slider healthBar;
    public Slider staminaBar;

    void Awake() {
        _playerInfo = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInfo>();
    }

    void Update() {
        healthBar.maxValue = _playerInfo.maxHealth;
        staminaBar.maxValue = _playerInfo.maxStamina;
        healthBar.value = _playerInfo.Health;
        staminaBar.value = _playerInfo.Stamina;
    }
}
