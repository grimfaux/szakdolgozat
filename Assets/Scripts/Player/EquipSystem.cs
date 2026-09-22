using UnityEngine;

/// Owns whether the weapon is drawn. The animation events on the draw/sheathe
/// clips call EquipWeapon, so this flag only flips once the swap actually happened.
public class EquipmentSystem : MonoBehaviour {
    [SerializeField] private GameObject weapon;
    [SerializeField] private GameObject weaponHolder;
    [SerializeField] private GameObject weaponSheath;

    [Tooltip("If the equip animation event never fires, unlock input again after this long.")]
    [SerializeField] private float swapTimeout = 2f;

    public Animator playerAnimator;

    private GameObject currentWeaponInHand;
    private GameObject currentWeaponInSheath;
    private bool isEquipped;
    private float swapStartedAt = -1f;

    public bool IsEquipped => isEquipped;
    public bool IsSwapping => swapStartedAt >= 0f;

    private void Awake() {
        if (playerAnimator == null) playerAnimator = GetComponentInChildren<Animator>();
    }

    private void Start() {
        if (weapon != null && weaponSheath != null) {
            currentWeaponInSheath = Instantiate(weapon, weaponSheath.transform);
        }
    }

    private void Update() {
        if (IsSwapping && Time.time - swapStartedAt > swapTimeout) {
            swapStartedAt = -1f;
        }
    }

    public void BeginSwap() {
        swapStartedAt = Time.time;
    }

    // Animation event on the draw / sheathe clips.
    public void EquipWeapon() {
        swapStartedAt = -1f;
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
            playerAnimator.SetBool(PlayerAnimatorParams.IsAttacking, false);
            playerAnimator.SetTrigger(PlayerAnimatorParams.Move);
        }
    }

    private DamageDealer GetHeldDamageDealer() {
        if (currentWeaponInHand == null) return null;
        return currentWeaponInHand.GetComponentInChildren<DamageDealer>();
    }
}
