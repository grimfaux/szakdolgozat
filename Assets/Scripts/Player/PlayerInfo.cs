using System;
using UnityEngine;

public class PlayerInfo : MonoBehaviour, IDamageable {
    [Header("Player Info")]
    public float maxHealth = 100f;
    public float maxStamina = 50f;

    [Header("Invulnerability")]
    [Tooltip("Set by the dodge roll. Damage is ignored while this is above zero.")]
    [SerializeField] private float invulnerabilityRemaining;

    public event Action<DamageInfo> Damaged;
    public event Action Died;

    private float health;
    private float stamina;

    public float Stamina {
        get { return stamina; }
        set { stamina = Mathf.Clamp(value, 0, maxStamina); }
    }

    public float Health {
        get { return health; }
        set { health = Mathf.Clamp(value, 0, maxHealth); }
    }

    public bool IsDead => health <= 0f;
    public bool IsInvulnerable => invulnerabilityRemaining > 0f;

    private void Awake() {
        health = maxHealth;
        stamina = maxStamina;
    }

    private void Update() {
        if (invulnerabilityRemaining > 0f) {
            invulnerabilityRemaining -= Time.deltaTime;
        }
    }

    public void TakeDamage(DamageInfo info) {
        if (IsDead || IsInvulnerable) return;

        float amount = info.Amount * DifficultyScaler.PlayerDamageTaken;
        Health = Health - amount;

        CombatStats.HitsTaken++;
        CombatStats.DamageTaken += amount;
        Damaged?.Invoke(info);

        if (IsDead) {
            CombatStats.PlayerDeaths++;
            Died?.Invoke();
        }
    }

    public void GrantInvulnerability(float duration) {
        invulnerabilityRemaining = Mathf.Max(invulnerabilityRemaining, duration);
    }

    /// Flat cost. Callers that drain over time multiply by Time.deltaTime themselves.
    public void DecreaseStamina(float amount) {
        this.Stamina = this.Stamina - amount;
    }

    public void RegenStamina(float amount) {
        this.Stamina = this.Stamina + amount;
    }

    public bool TrySpendStamina(float amount) {
        if (Stamina < amount) return false;
        DecreaseStamina(amount);
        return true;
    }
}
