using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable {
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Poise")]
    [Tooltip("Stagger triggers once accumulated poise damage exceeds this.")]
    [SerializeField] private float maxPoise = 40f;
    [SerializeField] private float poiseRegenPerSecond = 15f;
    [SerializeField] private float poiseRegenDelay = 1.5f;

    [Header("Death")]
    [SerializeField] private float destroyDelay = 3f;
    [SerializeField] private bool destroyOnDeath = true;

    [Header("References")]
    [SerializeField] private Animator animator;

    public event Action<DamageInfo> Damaged;
    public event Action<DamageInfo> Staggered;
    public event Action Died;

    private static readonly int HitHash = Animator.StringToHash("hit");
    private static readonly int StaggerHash = Animator.StringToHash("stagger");
    private static readonly int DieHash = Animator.StringToHash("die");

    private float _health;
    private float _poise;
    private float _lastHitTime;

    public float CurrentHealth => _health;
    public float MaxHealth => maxHealth;
    public float Normalized => maxHealth > 0f ? _health / maxHealth : 0f;
    public bool IsDead => _health <= 0f;

    private void Awake() {
        _health = maxHealth;
        _poise = maxPoise * DifficultyScaler.EnemyPoise;
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    private void Update() {
        if (IsDead) return;

        float poiseCap = maxPoise * DifficultyScaler.EnemyPoise;
        if (_poise < poiseCap && Time.time - _lastHitTime >= poiseRegenDelay) {
            _poise = Mathf.Min(poiseCap, _poise + poiseRegenPerSecond * Time.deltaTime);
        }
    }

    public void TakeDamage(DamageInfo info) {
        if (IsDead) return;

        _health = Mathf.Max(0f, _health - info.Amount);
        _lastHitTime = Time.time;
        Damaged?.Invoke(info);

        if (IsDead) {
            Die();
            return;
        }

        _poise -= info.PoiseDamage;
        if (_poise <= 0f) {
            _poise = maxPoise * DifficultyScaler.EnemyPoise;
            Staggered?.Invoke(info);
            SetTriggerIfPresent(StaggerHash);
        } else {
            SetTriggerIfPresent(HitHash);
        }
    }

    public void Heal(float amount) {
        if (IsDead) return;
        _health = Mathf.Min(maxHealth, _health + amount);
    }

    public void ResetHealth() {
        _health = maxHealth;
        _poise = maxPoise * DifficultyScaler.EnemyPoise;
    }

    private void Die() {
        CombatStats.EnemiesKilled++;
        Died?.Invoke();
        SetTriggerIfPresent(DieHash);

        foreach (Collider col in GetComponentsInChildren<Collider>()) col.enabled = false;
        if (destroyOnDeath) Destroy(gameObject, destroyDelay);
    }

    // The enemy animator controller does not exist yet, so setting an unknown
    // parameter would spam the console. Check before touching it.
    private void SetTriggerIfPresent(int hash) {
        if (animator == null) return;
        foreach (AnimatorControllerParameter p in animator.parameters) {
            if (p.nameHash == hash && p.type == AnimatorControllerParameterType.Trigger) {
                animator.SetTrigger(hash);
                return;
            }
        }
    }
}
