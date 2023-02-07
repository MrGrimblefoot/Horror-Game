using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilSystem : MonoBehaviour
{
    private bool isAiming;

    private Vector3 currentRotation;
    private Vector3 targetRotation;

    private WeaponSystem weaponScript;
    public Weapon gun;
    public GameObject weapon;

    public int currentStep;
    public bool hasResetRecoilPattern;
    public float recoilResetTimer;

    void Start()
    {
        
    }

    void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, gun.returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Lerp(targetRotation, targetRotation, gun.snappiness * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void RecoilFire()
    {

    }
}
