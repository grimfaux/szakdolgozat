using UnityEngine;

public struct DamageInfo {
    public float Amount;
    public float PoiseDamage;
    public Vector3 HitPoint;
    public Vector3 HitNormal;
    public GameObject Source;

    public DamageInfo(float amount, float poiseDamage, Vector3 hitPoint, Vector3 hitNormal, GameObject source) {
        Amount = amount;
        PoiseDamage = poiseDamage;
        HitPoint = hitPoint;
        HitNormal = hitNormal;
        Source = source;
    }
}

public interface IDamageable {
    bool IsDead { get; }
    void TakeDamage(DamageInfo info);
}
