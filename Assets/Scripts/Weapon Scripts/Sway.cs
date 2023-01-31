using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
//using Photon.Pun;

public class Sway : MonoBehaviour/*PunCallbacks*/
{
    #region Variables
    [SerializeField] private float swayIntensity;
    [Tooltip("This modifies the weapon sway when at the hip. Lower numbers make it less sensitive, and higher numbers make it more sensitive.")]
    [SerializeField] private float hipSwayModifier = 10;
    [Tooltip("This modifies the weapon sway when ADSing. Lower numbers make it less sensitive, and higher numbers make it more sensitive.")]
    [SerializeField] private float ADSSwayModifier = 50;
    [SerializeField] private float smooth;

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
        Quaternion tempXAdj = Quaternion.AngleAxis((!gun.aiming ? swayIntensity * hipSwayModifier : swayIntensity * ADSSwayModifier) * targetXMouse, Vector3.up);
        Quaternion tempYAdj = Quaternion.AngleAxis((!gun.aiming ? swayIntensity * hipSwayModifier : swayIntensity * ADSSwayModifier) * targetYMouse, Vector3.right);
        Quaternion tempZAdj = Quaternion.AngleAxis((!gun.aiming ? swayIntensity * hipSwayModifier : swayIntensity * ADSSwayModifier) * targetYMouse, Vector3.right);
        Quaternion targetRotation = originRotation * tempXAdj * tempYAdj * tempZAdj;

        //rotate towards target rotation
        if (!gun.aiming) { transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * smooth); }
        else { transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * gun.currentGunData.rotationalKickbackReturnSpeed * gun.currentGunData.ADSSwayMultiplier); }
    }
    #endregion
}
