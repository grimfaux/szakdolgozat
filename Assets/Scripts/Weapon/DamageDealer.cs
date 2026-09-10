using System;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    private bool canDealDamage;
    private List<GameObject> hasDealtDamage;

    [SerializeField] private float weaponLength;
    [SerializeField] private float weaponDamage;
    
    void Start() {
        canDealDamage = false;
        hasDealtDamage = new List<GameObject>();
    }

    void Update() {
        if (canDealDamage) {
            RaycastHit hit;
            
            int layerMask = 1 << LayerMask.NameToLayer("Enemy");
            if (Physics.Raycast(transform.position, -transform.up, out hit, weaponLength, layerMask)) {
                if (!hasDealtDamage.Contains(hit.collider.gameObject)) {
                    Debug.Log("damage");
                    hasDealtDamage.Add(hit.collider.gameObject);
                    Debug.Log(hasDealtDamage.Count);
                }
            }
        }
    }

    public void StartDealDamage() {
        Debug.Log("StartDealDamage");
        canDealDamage = true;
        hasDealtDamage.Clear();
        Debug.Log(hasDealtDamage.Count);
    }

    public void EndDealDamage() {
        Debug.Log("EndDealDamage");
        canDealDamage = false;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position - transform.up * weaponLength);
    }
}
