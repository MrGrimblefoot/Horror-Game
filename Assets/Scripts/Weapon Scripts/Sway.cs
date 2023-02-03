using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
//using Photon.Pun;

public class Sway : MonoBehaviour/*PunCallbacks*/
{
    #region Variables
    private Quaternion originRotation;
    public WeaponSystem gun;

    public float targetXMouse = 0f;
    public float targetYMouse = 0f;

    private PlayerInput playerInput;
    private BasicInputActions basicInputActions;
    #endregion

    #region MonoBehaviour Callbacks
    private void Start()
    {
        originRotation = transform.localRotation;
        gun = GetComponentInParent<WeaponSystem>();

        #region InputActions
        basicInputActions = new BasicInputActions();
        basicInputActions.Player.MouseX.performed += ctx => targetXMouse = ctx.ReadValue<float>();
        basicInputActions.Player.MouseX.Enable();
        basicInputActions.Player.MouseY.performed += ctx => targetYMouse = ctx.ReadValue<float>();
        basicInputActions.Player.MouseY.Enable();
        #endregion

    }
    void Update()
    {
        UpdateSway();
    }
    #endregion

    #region Private Methods
    private void UpdateSway()
    {
        //calculate target rotation
        Quaternion tempXAdj = Quaternion.AngleAxis((!gun.aiming ? (gun.currentGunData.swayIntensity / 50) * gun.currentGunData.rotKickReturnSpeed : (gun.currentGunData.aimSwayIntensity / 50) * gun.currentGunData.aimRotKickReturnSpeed) * targetXMouse, Vector3.up);
        Quaternion tempYAdj = Quaternion.AngleAxis((!gun.aiming ? (gun.currentGunData.swayIntensity / 50) * gun.currentGunData.rotKickReturnSpeed : (gun.currentGunData.aimSwayIntensity / 50) * gun.currentGunData.aimRotKickReturnSpeed) * targetYMouse, Vector3.right);
        Quaternion tempZAdj = Quaternion.AngleAxis((!gun.aiming ? (gun.currentGunData.swayIntensity / 50) * gun.currentGunData.rotKickReturnSpeed : (gun.currentGunData.aimSwayIntensity / 50) * gun.currentGunData.aimRotKickReturnSpeed) * targetYMouse, Vector3.right);
        Quaternion targetRotation = originRotation * tempXAdj * tempYAdj * tempZAdj;

        //rotate towards target rotation
        if (!gun.aiming) { transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * gun.currentGunData.rotKickReturnSpeed); }
        else { transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * gun.currentGunData.aimRotKickReturnSpeed); }
    }
    #endregion
}
