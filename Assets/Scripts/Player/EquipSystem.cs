using UnityEngine;

public class EquipmentSystem : MonoBehaviour {
    [SerializeField] private GameObject weapon;
    [SerializeField] private GameObject weaponHolder;
    [SerializeField] private GameObject weaponSheath;
    public Animator playerAnimator;

    private GameObject currentWeaponInHand;
    private GameObject currentWeaponInSheath;
    private bool isEquipped = false;

    public bool IsEquipped => isEquipped;

    void Start() {
        if (playerAnimator == null) playerAnimator = GetComponent<Animator>();
        if (weapon != null && weaponSheath != null) {
            currentWeaponInSheath = Instantiate(weapon, weaponSheath.transform);
        }
    }

    // Animation event on the Sword_Enter / Sword_Exit clips.
    public void EquipWeapon() {
        if (weapon == null) return;

        if (!isEquipped) {
            if (weaponHolder == null) return;
            currentWeaponInHand = Instantiate(weapon, weaponHolder.transform);
            if (currentWeaponInSheath != null) Destroy(currentWeaponInSheath);
        } else {
            if (weaponSheath == null) return;
            currentWeaponInSheath = Instantiate(weapon, weaponSheath.transform);
            if (currentWeaponInHand != null) Destroy(currentWeaponInHand);
        }
        isEquipped = !isEquipped;
    }

    // Animation events on the attack clip.
    public void StartDealDamage() {
        DamageDealer dealer = GetHeldDamageDealer();
        if (dealer != null) dealer.StartDealDamage();
    }

    public void EndDealDamage() {
        DamageDealer dealer = GetHeldDamageDealer();
        if (dealer != null) dealer.EndDealDamage();

        if (playerAnimator != null) {
            playerAnimator.SetBool("isAttacking", false);
            playerAnimator.SetTrigger("move");
        }
    }

    private DamageDealer GetHeldDamageDealer() {
        if (currentWeaponInHand == null) return null;
        return currentWeaponInHand.GetComponentInChildren<DamageDealer>();
    }
}
