using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Combat : MonoBehaviour {
    
    public Animator playerAnimator;
    private PlayerControls _controls;
    
    private bool isEquipped = false;
    private bool moving = false;     

    private void Awake() {
        playerAnimator = GetComponent<Animator>();
        _controls = new PlayerControls();
    }

    private void OnEnable() {
        _controls.Enable();
        _controls.Player.Equip.performed += OnEquip;
        _controls.Player.LightAttack.performed += OnLightAttack;
    }

    private void OnLightAttack(InputAction.CallbackContext obj) {
        if (isEquipped && !playerAnimator.GetBool("isAttacking")) {
            Debug.Log(playerAnimator.GetCurrentAnimatorStateInfo(1).fullPathHash);
            playerAnimator.SetBool("isAttacking", true);
            playerAnimator.SetTrigger("attack");
        }
    }

    private void OnEquip(InputAction.CallbackContext obj) {
        if (moving) {
            playerAnimator.SetTrigger(isEquipped ? "unequipUpper" : "equipUpper");
        } else {
            playerAnimator.SetTrigger(isEquipped ? "unequipFull" : "equipFull");
        }
        isEquipped = !isEquipped;
    }

    private void OnDisable() {
        _controls.Disable();
    }

    void Update() {
        if (isEquipped) playerAnimator.SetTrigger("enterCombat");
        
        moving = playerAnimator.GetFloat("speed") > 0.001f;
    }
}
