using UnityEditor;

public static class DebugCheatTools
{
    [MenuItem("DebugToolsAndCheats/Debug Tools/Invulnerablity")]
    public static void Invulnerablity()
    {
        PlayerPolishManager.OnInvulnerablity();
    }

    [MenuItem("DebugToolsAndCheats/Debug Tools/Heal To Max")]
    public static void HealToMax()
    {
        PlayerPolishManager.OnHealToMax();
    }

    [MenuItem("DebugToolsAndCheats/Debug Tools/Kill Player")]
    public static void KillPlayer()
    {
        PlayerPolishManager.OnPlayerDie();
    }

    [MenuItem("DebugToolsAndCheats/Debug Tools/Replenish Ammo")]
    public static void ReplenishAmmo()
    {
        WeaponSystem.OnReplenishAmmo();
    }
}
