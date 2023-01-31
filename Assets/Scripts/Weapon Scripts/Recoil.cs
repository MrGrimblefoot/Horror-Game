using UnityEngine;

public class Recoil : MonoBehaviour
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
        weaponScript = GetComponentInParent<WeaponSystem>();
    }

    void Update()
    {
        isAiming = weaponScript.aiming;

        if(gun != null)
        {
            if (weaponScript.aiming)
            {
                targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, gun.aimReturnSpeed * Time.deltaTime);
                currentRotation = Vector3.Slerp(currentRotation, targetRotation, gun.aimSnappiness * Time.fixedDeltaTime);
            }
            else
            {
                targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, gun.returnSpeed * Time.deltaTime);
                currentRotation = Vector3.Slerp(currentRotation, targetRotation, gun.snappiness * Time.fixedDeltaTime);
            }

            transform.localRotation = Quaternion.Euler(currentRotation);
        }

        recoilResetTimer += Time.deltaTime;
        if(recoilResetTimer >= 0.7f)
        {
            if (!weaponScript.shooting && !hasResetRecoilPattern) { hasResetRecoilPattern = true;/* print("recoil pattern reset!");*/ }
            recoilResetTimer = 0;
        }
        //else { Debug.Log("No gun, so can't move gun!"); }
    }

    //public void RecoilFire()
    //{
    //    if(gun != null)
    //    {
    //        if (weaponScript.aiming)
    //        {
    //            targetRotation += new Vector3(gun.aimRecoilX, Random.Range(-gun.aimRecoilY, gun.aimRecoilY), Random.Range(-gun.aimRecoilZ, gun.aimRecoilZ));
    //            weaponScript.currentWeapon.transform.Rotate(-gun.rotationalKickback / 10f, 0, 0);
    //            weaponScript.currentWeapon.transform.position -= weaponScript.currentWeapon.transform.forward * gun.positionalKickback / 10f;
    //        }
    //        else
    //        {
    //            targetRotation += new Vector3(gun.recoilX, Random.Range(-gun.recoilY, gun.recoilY), Random.Range(-gun.recoilZ, gun.recoilZ));
    //            weaponScript.currentWeapon.transform.Rotate(-gun.rotationalKickback / 5.5f, 0, 0);
    //            weaponScript.currentWeapon.transform.position -= weaponScript.currentWeapon.transform.forward * gun.positionalKickback / 5.5f;
    //        }
    //    }
    //    //else { Debug.Log("No gun, so can't recoil!"); }
    //}

    public void RecoilFire()
    {
        if(gun != null)
        {
            Transform currentGun = weaponScript.currentWeapon.transform.GetComponentInChildren<Sway>().transform;

            if (gun.randomizeRecoil)
            {
                float xRecoil = Random.Range(-gun.randomRecoilConstraints.x, gun.randomRecoilConstraints.x);
                float yRecoil = Random.Range(-gun.randomRecoilConstraints.y, gun.randomRecoilConstraints.y);
                targetRotation += new Vector3(xRecoil, yRecoil, 0);
            }
            else
            {
                //if (!hasResetRecoilPattern) { currentStep++; currentStep = gun.magazineSize + 1 - gun.currentBulletsInMagazine; }
                //else { currentStep = gun.magazineSize + 1 - gun.currentBulletsInMagazine; hasResetRecoilPattern = false; }
                if (!hasResetRecoilPattern)
                {
                    if (currentStep >= gun.magazineSize) { currentStep = gun.magazineSize; }
                    else { currentStep++; }
                }
                else { currentStep = 0; hasResetRecoilPattern = false; }

                currentStep = Mathf.Clamp(currentStep, 0, gun.recoilPattern.Length - 1);

                targetRotation += gun.recoilPattern[currentStep];
            }

            currentGun.Rotate(gun.rotationalKickback / 10f, 0, 0);
            currentGun.position -= weaponScript.currentWeapon.transform.forward * gun.positionalKickback / 10f;
        }
        else { return; }
    }
}
