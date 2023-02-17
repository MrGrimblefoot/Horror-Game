using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeDamageHandler : MonoBehaviour
{
    private float damage;
    [SerializeField] private WeaponSystem weaponSystem;
    [SerializeField] private Weapon weapon;
    [SerializeField] private int damageLayer1, damageLayer2;
    public bool hasMadeSound;
    public bool hasDealtDamage;

    private void OnEnable()
    {
        //weaponSystem = FindObjectOfType<WeaponSystem>().GetComponent<WeaponSystem>();        
        weaponSystem = GetComponentInParent<WeaponSystem>();
        damage = weapon.damage;
        hasDealtDamage = false;
    }

    private void OnCollisionEnter (Collision other)
    {
        CheckForEnemy(other);
        //print("Checking for enemy!");
    }

    private void CheckForEnemy(Collision other)
    {
        if (!hasDealtDamage)
        {
            if (other.collider.gameObject.layer == damageLayer1/* || other.collider.gameObject.layer == damageLayer2*/)
            {
                print(other.gameObject.name);
                other.gameObject.GetComponent<EnemyBodyPartHealthManager>().DamageEnemyPart(damage);
                if (!hasMadeSound) { weaponSystem.HitmarkerEffect(false); hasMadeSound = true; }
                hasDealtDamage = true;
            }
        }
        //else { print("Already dealt damage!"); }
    }

    public void ResetHitmarker() { hasMadeSound = false; }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(impactPoint.position, length);
    //    //Gizmos.DrawRay(impactPoint.position, impactPoint.up);
    //}
}
