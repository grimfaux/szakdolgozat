using System;
using UnityEngine;

public class EquipmentSystem : MonoBehaviour {
    [SerializeField] private GameObject weapon;
    [SerializeField] private GameObject weaponHolder;
    [SerializeField] private GameObject weaponSheath;
    public Animator playerAnimator;

    
    private GameObject currentWeaponInHand;
    private GameObject currentWeaponInSheath;
    private bool isEquipped = false;

    void Start() {
        playerAnimator = GetComponent<Animator>();
        currentWeaponInSheath = Instantiate(weapon, weaponSheath.transform);
    }

    public void EquipWeapon() {
        if (!isEquipped) {
            currentWeaponInHand = Instantiate(weapon, weaponHolder.transform);
            Destroy(currentWeaponInSheath);
        } else {
            currentWeaponInSheath = Instantiate(weapon, weaponSheath.transform);
            Destroy(currentWeaponInHand);
        }
        isEquipped = !isEquipped;
    }

    public void StartDealDamage() {
        currentWeaponInHand.GetComponentInChildren<DamageDealer>().StartDealDamage();
    }

    public void EndDealDamage() {
        currentWeaponInHand.GetComponentInChildren<DamageDealer>().EndDealDamage();
        playerAnimator.SetBool("isAttacking", false);
        playerAnimator.SetTrigger("move");
    }
}
