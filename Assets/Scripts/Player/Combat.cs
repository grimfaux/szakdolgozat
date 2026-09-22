using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(EquipmentSystem))]
public class Combat : MonoBehaviour {
    [SerializeField] private Animator playerAnimator;

    private EquipmentSystem _equipment;

    private void Awake() {
        if (playerAnimator == null) playerAnimator = GetComponentInChildren<Animator>();
        _equipment = GetComponent<EquipmentSystem>();
    }

    private void OnEnable() {
        PlayerInputs.Acquire();
        PlayerInputs.Controls.Player.Equip.performed += OnEquip;
        PlayerInputs.Controls.Player.LightAttack.performed += OnLightAttack;
    }

    private void OnDisable() {
        PlayerInputs.Controls.Player.Equip.performed -= OnEquip;
        PlayerInputs.Controls.Player.LightAttack.performed -= OnLightAttack;
        PlayerInputs.Release();
    }

    private void OnLightAttack(InputAction.CallbackContext context) {
        if (!_equipment.IsEquipped || _equipment.IsSwapping) return;
        if (playerAnimator.GetBool(PlayerAnimatorParams.IsAttacking)) return;

        playerAnimator.SetBool(PlayerAnimatorParams.IsAttacking, true);
        playerAnimator.SetTrigger(PlayerAnimatorParams.Attack);
    }

    private void OnEquip(InputAction.CallbackContext context) {
        if (_equipment.IsSwapping) return;
        if (playerAnimator.GetBool(PlayerAnimatorParams.IsAttacking)) return;

        bool moving = playerAnimator.GetFloat(PlayerAnimatorParams.Speed) > 0.001f;
        bool equipped = _equipment.IsEquipped;

        if (moving) {
            playerAnimator.SetTrigger(equipped ? PlayerAnimatorParams.UnequipUpper : PlayerAnimatorParams.EquipUpper);
        } else {
            playerAnimator.SetTrigger(equipped ? PlayerAnimatorParams.UnequipFull : PlayerAnimatorParams.EquipFull);
        }

        // enterCombat is a trigger, so it fires once per draw rather than every frame.
        if (!equipped) playerAnimator.SetTrigger(PlayerAnimatorParams.EnterCombat);

        _equipment.BeginSwap();
    }
}
