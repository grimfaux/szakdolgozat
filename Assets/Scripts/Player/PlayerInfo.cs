using System;
using UnityEngine;

public class PlayerInfo : MonoBehaviour {
    [Header("Player Info")]
    private float health = 100f;
    public float maxHealth = 100f;
    private float stamina = 50f;
    public float maxStamina = 50f;

    public float Stamina {
        get { return stamina; }
        set { stamina = Mathf.Clamp(value, 0, maxStamina); }
    }
    
    public float Health {
        get { return health; }
        set { health = Mathf.Clamp(value, 0, maxHealth); }
    }
    
    

    public void DecreaseStamina(float amount) {
        this.Stamina = this.Stamina - amount * Time.deltaTime;
    }
    
    public void RegenStamina(float amount) {
        this.Stamina = this.Stamina + amount * Time.deltaTime;
    }
}
