using System.Collections.Generic;
using UnityEngine;

/// Sweeps the blade between frames instead of firing a single ray, so a fast
/// swing cannot tunnel past a target. Enabled by animation events on the attack
/// clip via EquipmentSystem.StartDealDamage / EndDealDamage.
public class DamageDealer : MonoBehaviour {
    [Header("Blade")]
    [SerializeField] private float weaponLength = 1f;
    [Tooltip("Points sampled along the blade. More points catch thinner targets.")]
    [SerializeField] private int sampleCount = 5;

    [Header("Damage")]
    [SerializeField] private float weaponDamage = 20f;
    [SerializeField] private float poiseDamage = 20f;
    [SerializeField] private LayerMask hittableLayers = ~0;

    private readonly List<IDamageable> _alreadyHit = new List<IDamageable>();
    private Vector3[] _previousSamples;
    private bool _canDealDamage;
    private Transform _owner;

    private void Awake() {
        _owner = transform.root;
        sampleCount = Mathf.Max(2, sampleCount);
        _previousSamples = new Vector3[sampleCount];
    }

    private void LateUpdate() {
        if (!_canDealDamage) return;

        for (int i = 0; i < sampleCount; i++) {
            Vector3 current = SamplePosition(i);
            Vector3 previous = _previousSamples[i];

            // Swept check against where this point was last frame.
            if (previous != current && Physics.Linecast(previous, current, out RaycastHit sweepHit, hittableLayers, QueryTriggerInteraction.Ignore)) {
                TryDamage(sweepHit);
            }

            _previousSamples[i] = current;
        }

        // Plus a ray down the blade itself, for targets the sweep passes alongside.
        Vector3 origin = transform.position;
        if (Physics.Raycast(origin, -transform.up, out RaycastHit bladeHit, weaponLength, hittableLayers, QueryTriggerInteraction.Ignore)) {
            TryDamage(bladeHit);
        }
    }

    private void TryDamage(RaycastHit hit) {
        if (_owner != null && hit.collider.transform.IsChildOf(_owner)) return;

        IDamageable target = hit.collider.GetComponentInParent<IDamageable>();
        if (target == null || target.IsDead) return;
        if (_alreadyHit.Contains(target)) return;

        _alreadyHit.Add(target);

        float amount = weaponDamage * DifficultyScaler.PlayerDamageDealt;
        target.TakeDamage(new DamageInfo(amount, poiseDamage, hit.point, hit.normal, _owner != null ? _owner.gameObject : gameObject));

        CombatStats.HitsLanded++;
        CombatStats.DamageDealt += amount;
    }

    private Vector3 SamplePosition(int index) {
        float t = index / (float)(sampleCount - 1);
        return transform.position - transform.up * (weaponLength * t);
    }

    public void StartDealDamage() {
        sampleCount = Mathf.Max(2, sampleCount);
        if (_previousSamples == null || _previousSamples.Length != sampleCount) {
            _previousSamples = new Vector3[sampleCount];
        }

        _canDealDamage = true;
        _alreadyHit.Clear();
        CombatStats.SwingsThrown++;

        for (int i = 0; i < sampleCount; i++) {
            _previousSamples[i] = SamplePosition(i);
        }
    }

    public void EndDealDamage() {
        _canDealDamage = false;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Application.isPlaying && _canDealDamage ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, transform.position - transform.up * weaponLength);
    }
}
