using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponStowDraw : MonoBehaviour
{
    [SerializeField] private GameObject objToDestroy;
    [SerializeField] private Animator anim;
    [SerializeField] private MeleeDamageHandler damageHandler;

    public void Stow() { Destroy(objToDestroy); }
    public void ResetAttack() { damageHandler.hasDealtDamage = false; damageHandler.hasMadeSound = false; }
}
