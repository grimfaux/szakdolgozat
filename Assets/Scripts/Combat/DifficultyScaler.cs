using UnityEngine;

/// Global multipliers every combat actor runs its numbers through.
/// The adaptive difficulty layer only has to write these fields, nothing else.
public static class DifficultyScaler {
    public static float PlayerDamageDealt = 1f;
    public static float PlayerDamageTaken = 1f;
    public static float EnemyAggression = 1f;
    public static float EnemyAttackSpeed = 1f;
    public static float EnemyPoise = 1f;

    public static void ResetToDefault() {
        PlayerDamageDealt = 1f;
        PlayerDamageTaken = 1f;
        EnemyAggression = 1f;
        EnemyAttackSpeed = 1f;
        EnemyPoise = 1f;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetOnPlay() {
        ResetToDefault();
    }
}
