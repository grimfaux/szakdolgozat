using UnityEngine;

/// Cached parameter hashes for the player animator controller.
public static class PlayerAnimatorParams {
    public static readonly int Speed = Animator.StringToHash("speed");
    public static readonly int IsAttacking = Animator.StringToHash("isAttacking");
    public static readonly int Attack = Animator.StringToHash("attack");
    public static readonly int Move = Animator.StringToHash("move");
    public static readonly int EnterCombat = Animator.StringToHash("enterCombat");
    public static readonly int EquipFull = Animator.StringToHash("equipFull");
    public static readonly int UnequipFull = Animator.StringToHash("unequipFull");
    public static readonly int EquipUpper = Animator.StringToHash("equipUpper");
    public static readonly int UnequipUpper = Animator.StringToHash("unequipUpper");
    public static readonly int Dodge = Animator.StringToHash("dodge");
}
