using UnityEngine;

/// Raw combat telemetry. The adaptive difficulty layer reads these to decide
/// whether the player is struggling or coasting.
public static class CombatStats {
    public static int SwingsThrown;
    public static int HitsLanded;
    public static int HitsTaken;
    public static float DamageDealt;
    public static float DamageTaken;
    public static int EnemiesKilled;
    public static int PlayerDeaths;

    public static float Accuracy => SwingsThrown > 0 ? (float)HitsLanded / SwingsThrown : 0f;

    public static void ResetAll() {
        SwingsThrown = 0;
        HitsLanded = 0;
        HitsTaken = 0;
        DamageDealt = 0f;
        DamageTaken = 0f;
        EnemiesKilled = 0;
        PlayerDeaths = 0;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetOnPlay() {
        ResetAll();
    }
}
