using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using EZCameraShake;
using System;
using Random = UnityEngine.Random;

public class WeaponSystem : MonoBehaviour
{
    #region Variables

    #region Loadout
    [Header("Guns")]
    /*[SerializeField] private*/ public Weapon[] loadout;
    [HideInInspector] public Weapon currentGunData;
    public GameObject currentWeapon;
    [SerializeField] private GameObject sight;
    public bool hasSight;
    #endregion

    #region Shooting
    private int bulletsToShoot;
    private Camera cam;
    private Camera weaponCam;
    private GameObject sniperCam;
    private RaycastHit hit;
    public bool shooting;
    private bool readyToShoot;
    public bool isReloading;
    public bool hasResetRecoilPattern;
    private float currentCooldown;
    //[SerializeField] private string damageTag1, damageTag2, damageTag3;
    [SerializeField] private int damageLayer1, damageLayer2;
    #endregion

    #region Inputs
    private BasicInputActions basicInputActions;
    [SerializeField] private bool canReciveInput;
    [SerializeField] private bool isSwitching;
    [SerializeField] private float equipDelay;
    #endregion

    #region Aiming
    [HideInInspector]
    public bool aiming;
    private float targetFOV;
    private float weaponTargetFOV;
    [SerializeField] private float normalFOV = 60;
    private CameraLook camLook;
    #endregion

    #region Recoil
    private Recoil recoilScript;
    #endregion

    #region Equiping
    [Header("Equiping")]
    [SerializeField] private Transform weaponParent;
    private int currentIndex;
    #endregion

    #region UI
    //private GameObject cursor;
    private TextMeshProUGUI ammoCounterText;
    private Image hitMarkerImage;
    private float hitMarkerWaitTime;
    private Color transparentWhite = new Color(1, 1, 1, 0);
    #endregion

    #region SFX
    [Header("SFX")]
    [SerializeField] private AudioSource sfx;
    [SerializeField] private AudioClip hitMarkerSound;
    #endregion

    #region VFX
    [Header("VFX")]
    private Transform firePoint;
    [SerializeField] private TrailRenderer bulletTrail;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color headshotColor;
    [SerializeField] private ParticleSystem muzzleFlash;
    #endregion

    public static Action OnReplenishAmmo;
    #endregion

    #region MonoBehaviour Callbacks
    private void OnEnable()
    {
        basicInputActions.Player.Reload.Enable();
        basicInputActions.Player.Fire.Enable();
        basicInputActions.Player.Weapon1.Enable();
        basicInputActions.Player.Weapon2.Enable();
        basicInputActions.Player.Weapon3.Enable();
        OnReplenishAmmo += ReplenishAmmo;
    }

    private void OnDisable()
    {
        basicInputActions.Player.Reload.Disable();
        basicInputActions.Player.Fire.Disable();
        basicInputActions.Player.Weapon1.Disable();
        basicInputActions.Player.Weapon2.Disable();
        basicInputActions.Player.Weapon3.Disable();
        OnReplenishAmmo -= ReplenishAmmo;
    }

    private void Awake()
    {
        //cursor = GameObject.Find("HUD/Crosshair");
        recoilScript = transform.Find("Cameras/CameraRecoil").GetComponent<Recoil>();
        camLook = GetComponent<CameraLook>();
        readyToShoot = false;
        cam = GetComponentInChildren<Camera>();
        cam = transform.Find("Cameras/CameraRecoil/CameraShaker/Player Camera").GetComponent<Camera>();
        weaponCam = transform.Find("Cameras/CameraRecoil/CameraShaker/Weapon Camera").GetComponent<Camera>();
        hitMarkerImage = GameObject.Find("HUD/Hit Marker").GetComponent<Image>();
        hitMarkerImage.color = transparentWhite;
        ammoCounterText = GameObject.Find("HUD/Ammo Display").GetComponent<TextMeshProUGUI>();
        canReciveInput = true;
        hasResetRecoilPattern = true;

        #region InputActions
        basicInputActions = new BasicInputActions();
        basicInputActions.Player.Reload.performed += Reload;
        basicInputActions.Player.Weapon1.performed += Equip1;
        basicInputActions.Player.Weapon2.performed += Equip2;
        basicInputActions.Player.Weapon3.performed += Equip3;

        #endregion

        foreach (Weapon g in loadout) { g.Initialize(); }
        StartCoroutine(Equip(0));
        normalFOV = camLook.fieldOfView;

        if (currentWeapon != null)
        {
            firePoint = currentWeapon.transform.Find("Anchor/Fire Point");
        }
    }

    private void Update()
    {
        if (currentWeapon != null)
        {
            //if (currentGunData.isAutomatic) { shooting = basicInputActions.Player.Movement.ReadValue<bool>(); }
            //else { shooting = Input.GetKeyDown(shootButton); }

            if (currentGunData.name != "Sniper")
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * currentGunData.aimSpeed);
                weaponCam.fieldOfView = Mathf.Lerp(weaponCam.fieldOfView, weaponTargetFOV, Time.deltaTime * currentGunData.aimSpeed);
            }
        }

        if(currentCooldown > 0) { currentCooldown -= Time.deltaTime; }

        if (hitMarkerWaitTime > 0) { hitMarkerWaitTime -= Time.deltaTime; }
        else if (hitMarkerImage.color.a > 0) { hitMarkerImage.color = Color.Lerp(hitMarkerImage.color, transparentWhite, Time.deltaTime * 8); }

        if (canReciveInput) { HandleInput(); }
    }
    #endregion

    #region Private Methods
    private void HandleInput()
    {
        if (currentWeapon != null)
        {
            {
                if(currentGunData.isMelee) { Attack();}
                else
                {
                    if (readyToShoot && shooting && !isReloading && currentCooldown <= 0 && currentGunData.currentBulletsInMagazine > 0)
                    {
                        if (currentGunData.UpdateMagazine()) { bulletsToShoot = currentGunData.bulletsPerTap; Shoot(); }
                    }

                    if (readyToShoot && shooting && !isReloading && currentCooldown <= 0 && currentGunData.currentBulletsInMagazine <= 0 && currentGunData.currentAmmoStash > 0) { StartCoroutine(HandleReload()); }
                }
            }
        }
    }

    private void Equip1(InputAction.CallbackContext context) { StartCoroutine(Equip(0)); }
    private void Equip2(InputAction.CallbackContext context) { StartCoroutine(Equip(1)); }
    private void Equip3(InputAction.CallbackContext context) { StartCoroutine(Equip(2)); }

    public IEnumerator Equip(int p_ind)
    {
        if (!isSwitching && !isReloading)
        {
            isSwitching = true;
            if(sight != null) { sight = null; }
            if (currentWeapon != null)
            {
                if (currentGunData.isMelee) { currentWeapon.GetComponentInChildren<Animator>().SetTrigger("Stow"); }
                if (!currentGunData.isMelee) { Destroy(currentWeapon); }
            }

            yield return new WaitForSeconds(equipDelay);

            currentIndex = p_ind;

            GameObject t_newWeapon = Instantiate(loadout[currentIndex].prefab, weaponParent.position, weaponParent.rotation, weaponParent) as GameObject;
            t_newWeapon.transform.localPosition = Vector3.zero;
            t_newWeapon.transform.localEulerAngles = Vector3.zero;

            currentWeapon = t_newWeapon;
            currentGunData = loadout[currentIndex];
            recoilScript.gun = currentGunData;

            targetFOV = 60;
            weaponTargetFOV = 60;

            firePoint = loadout[currentIndex].prefab.transform.Find("Anchor/Fire Point");

            if (currentGunData.isAutomatic || currentGunData.isMelee)
            {
                basicInputActions.Player.Fire.started += _ => shooting = true;
                basicInputActions.Player.Fire.canceled += _ => shooting = false;
            }
            else { basicInputActions.Player.Fire.performed += _ => shooting = true; }

            if (currentGunData.useMuzzleFlash) { muzzleFlash = FindObjectOfType<ParticleSystem>(); }
            UpdateAmmoUI(currentGunData.isMelee);

            readyToShoot = true;

            HandleSightSpawning();

            isSwitching = false;
        }

        StopCoroutine(Equip(p_ind));
    }

    private void ChangeLayersRecursively(GameObject p_target, int p_layer)
    {
        p_target.layer = p_layer;
        foreach(Transform a in p_target.transform) { ChangeLayersRecursively(a.gameObject, p_layer); }
    }

    private void Reload(InputAction.CallbackContext context)
    {
        if (!isReloading && currentGunData.GetMag() <= currentGunData.magazineSize - 1 && currentGunData.GetStash() > 0) { StartCoroutine(HandleReload()); }
    }

    private IEnumerator HandleReload()
    {
        if(currentWeapon != null)
        {
            isReloading = true;
            //currentWeapon.SetActive(false);

            yield return new WaitForSeconds(currentGunData.reloadTime);

            currentGunData.Reload();
            UpdateAmmoUI(false);
            //currentWeapon.SetActive(true);
            isReloading = false;
        }
    }

    public void Aim(bool isAiming)
    {
        if(!currentWeapon) { return; }

        aiming = isAiming;

        //find the Anchor
        Transform tempAnchor = currentWeapon.transform.Find("Anchor");
        Transform tempStateADS = currentWeapon.transform.Find("States/ADS");
        Transform tempStateHip = currentWeapon.transform.Find("States/Hip");

        //determine gun position
        if (!currentGunData.isMelee)
        {
            if (isAiming)
            {
                tempAnchor.position = Vector3.Lerp(tempAnchor.position,
                    new Vector3(tempStateADS.position.x, hasSight && currentGunData.isPistol ? tempStateADS.position.y + currentGunData.sightOffset : tempStateADS.position.y, tempStateADS.position.z),
                    Time.deltaTime * (currentGunData.aimSpeed * currentGunData.aimPosKickReturnSpeed));
                targetFOV = normalFOV / currentGunData.playerCamZoomMultiplier;
                weaponTargetFOV = normalFOV / currentGunData.weaponCamZoomMultiplier;
                //cursor.SetActive(false);
                //if (currentGunData.name != "Sniper")
                //{
                //}
                //else { sniperCam.gameObject.SetActive(true); }
            }
            else
            {
                tempAnchor.position = Vector3.Lerp(tempAnchor.position, tempStateHip.position, Time.deltaTime * (currentGunData.aimSpeed * currentGunData.posKickReturnSpeed));
                //cursor.SetActive(true);
                /*if (currentGunData.name != "Sniper")*/
                targetFOV = normalFOV; weaponTargetFOV = normalFOV;
                //else { sniperCam.gameObject.SetActive(false); }
            }
        }
        else { targetFOV = normalFOV; weaponTargetFOV = normalFOV; }
    }

    private void Attack()
    {
        if (shooting && aiming) { shooting = false; aiming = false; }
        currentWeapon.GetComponentInChildren<Animator>().SetBool("Light Attack", shooting);
        currentWeapon.GetComponentInChildren<Animator>().SetBool("Heavy Attack", aiming);
    }

    private void Shoot()
    {
        if (currentGunData.isMelee) { return; }
        cam = transform.Find("Cameras/CameraRecoil/CameraShaker/Player Camera").GetComponent<Camera>();

        readyToShoot = false;

        //cooldown
        currentCooldown = currentGunData.fireRate;

        //bullet spread
        Vector3 tempSpread = cam.transform.position + cam.transform.forward * 1000f;
        tempSpread += Random.Range(-currentGunData.bulletSpread, currentGunData.bulletSpread) * cam.transform.up;
        tempSpread += Random.Range(-currentGunData.bulletSpread, currentGunData.bulletSpread) * cam.transform.right;
        tempSpread -= cam.transform.position;
        tempSpread.Normalize();

        //raycast
        if (Physics.Raycast(cam.transform.position, tempSpread, out hit, 1000f, currentGunData.canBeShot))
        {
            //Debug.Log(hit.collider.name);

            //TrailRenderer trail = Instantiate(bulletTrail, firePoint.position, firePoint.rotation);
            //StartCoroutine(SpawnTrail(trail, hit));

            if (hit.collider.gameObject.layer == damageLayer1 || hit.collider.gameObject.layer == damageLayer2)
            {
                hit.collider.GetComponent<EnemyBodyPartHealthManager>().DamageEnemyPart(currentGunData.damage);
                if (hit.collider.CompareTag("EnemyHead")) { HitmarkerEffect(true); }
                else { HitmarkerEffect(false); }
            }
            if (hit.rigidbody != null) { hit.rigidbody.AddForceAtPosition(cam.transform.forward * (currentGunData.bulletForce * 1000), hit.point); }
            ApplyBulletHole();
        }

        bulletsToShoot--;

        if (bulletsToShoot > 0) { Invoke("Shoot", currentGunData.timeBetweenBullets/* / 60*/); }

        if (!IsInvoking("ResetShot") && !readyToShoot)
        {
            Invoke("ResetShot", currentGunData.timeBetweenFiring);
        }
        if (bulletsToShoot == 0)
        {
            //gunshot sound
            PlayGunshotSound();
            if(muzzleFlash != null) { muzzleFlash.Play(); }
            UpdateAmmoUI(false);
            //recoil
            recoilScript.RecoilFire();
            CameraShaker.Instance.ShakeOnce(currentGunData.magnitude, currentGunData.roughness, currentGunData.fadeInTime, currentGunData.fadeOutTime);
        }

        if (currentGunData.useRecovery) { /*currentWeapon.GetComponent<Animator>().Play("Recovery", 0, 0);*/ }
    }

    private void ResetShot() { readyToShoot = true; /*hasResetRecoilPattern = true;*/ if (!currentGunData.isAutomatic) { shooting = false; } }

    private void HandleSightSpawning()
    {
        if (currentGunData.isPistol)
        {
            GameObject sight = currentWeapon.transform.Find("Anchor/T77/Reflex Sight").gameObject;
            if (hasSight) { sight.SetActive(true); }
            else { sight.SetActive(false); }
        }
    }

    public void HitmarkerEffect(bool heashot)
    {
        if (heashot)
        {
            hitMarkerImage.color = headshotColor;
            sfx.PlayOneShot(hitMarkerSound, 1);
            hitMarkerWaitTime = 0.25f;
        }
        else
        {
            hitMarkerImage.color = normalColor;
            sfx.PlayOneShot(hitMarkerSound, 1);
            hitMarkerWaitTime = 0.25f;
        }
    }

    private void PlayGunshotSound()
    {
        sfx.clip = currentGunData.gunshotSounds[Random.Range(0, currentGunData.gunshotSounds.Length/* - 1*/)];
        sfx.pitch = 1 - currentGunData.pitchRandomization + Random.Range(-currentGunData.pitchRandomization, currentGunData.pitchRandomization);
        sfx.PlayOneShot(sfx.clip, currentGunData.volume);
    }

    private void ApplyBulletHole()
    {
        GameObject bulletHole = Instantiate(currentGunData.bulletHolePrefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
        bulletHole.transform.parent = hit.collider.gameObject.transform;
        Destroy(bulletHole, 4f);
    }

    private void UpdateAmmoUI(bool isMelee)
    {
        if (currentWeapon != null)
        {
            if (!isMelee) { ammoCounterText.SetText(currentGunData.GetMag() / currentGunData.bulletsPerTap + " / " + currentGunData.GetStash() / currentGunData.bulletsPerTap); }
            else { ammoCounterText.SetText(" "); }
        }
    }

    private void DamageEnemyPlayer(int damage)
    {
        GetComponent<PlayerPolishManager>().ApplyDamage(damage);
        //Debug.Log(GetComponent<PlayerController>().currentHealth);
    }

    private void ReplenishAmmo()
    {
        currentGunData.currentBulletsInMagazine = currentGunData.magazineSize;
        currentGunData.currentAmmoStash = currentGunData.maxAmmoStash;
    }
    #endregion
}
