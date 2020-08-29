using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using System;
using System.Text.RegularExpressions;

public class PlayerCombat : DamageableObject
{
    bool aiming;

    #region Accessed Components

    [HideInInspector]
    public Animator animator;
    Weapon currentWeapon;
    PlayerController playerController;
    CinemachineFreeLook freeLookCamera;

    #endregion


    #region Weapons

    [Header("Weapons Equipped")]

    public static GameObject meleeWeapon;
    public static GameObject primaryWeapon;
    public static GameObject secondaryWeapon;

    public enum WeaponSlot { Primary, Secondary, Melee, None };
    static WeaponSlot selectedSlot;
    public static int[] ammoCount;

    #endregion


    #region Weapon Hold Transforms

    [Header("Transforms for Weapons")]

    public Transform weaponHoldTransform;
    public Transform meleeWeaponStoreTransform;
    public Transform primaryWeaponStoreTransform;
    public Transform secondaryWeaponStoreTransform;

    #endregion


    #region Weapon UI

    [Header("Combat UI")]

    public GameObject crossHair;
    public GameObject weaponUI;
    public GameObject recoilUI;
    Animator weaponUIAnim;

    #endregion

    #region Zoom Variables

    [Header("Zoom Variables")]

    float cameraOriginalPOV;
    public float zoomPOV = 40;
    float targetPOV;
    public float zoomTime = 2;

    #endregion

    bool fadeUI = false;

    static GameObject _playerInstance;

    float recoilSpread;
    public float maxRecoil;
    bool recoilFull;
    
    void Start()
    {
        //Find length of ammo type enum
        int ammoEnumLength = Enum.GetNames(typeof(RangedWeapon.AmmoType)).Length;
        //Create int array of that length
        ammoCount = new int[ammoEnumLength];
    }

    void Awake()
    {
        // Preserve Player Data between scenes: *************

        //if we don't have an [_instance] set yet
        if(!_playerInstance)
            _playerInstance = this.gameObject;
         //otherwise, if we do, kill this thing
        else
            Destroy(this.gameObject);
 
        DontDestroyOnLoad(this.gameObject);

        //***************************************************


        //Store Component data to access: *******************

        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();

        freeLookCamera = Camera.main.transform.GetChild(0).GetComponent<CinemachineFreeLook>();
        cameraOriginalPOV = freeLookCamera.m_Lens.FieldOfView;

        //***************************************************        
    }
     
    // Update is called once per frame
    void Update()
    {
        //Melee Handling ************************************
        if (!isSlotEmpty(WeaponSlot.Melee) && !Input.GetButton("Fire2"))
            MeleeHandling();

        if(!isSlotEmpty(WeaponSlot.Primary) && !isSlotEmpty(WeaponSlot.Secondary))
        {
            // Weapon Swap **************************************
            if (Input.GetKeyDown(KeyCode.X)) ToggleSelectedWeapon();
        }
         
        if(!isSlotEmpty(selectedSlot) && !isMeleeAnimationPlaying())
            //Shooting Handling *********************************
            ShootingHandling();
    }

    void MeleeHandling()
    {
        //Get when the melee button is down
        bool meleeInput = Input.GetButton("Fire1");

        if (PlayerController.movementType != PlayerController.MovementTypeLookup.Sprint)
        {
            //Set the animator state to do melee attack
            animator.SetBool("Melee", meleeInput);
        }
        else
        {
            //Set the animator state to do melee attack
            animator.SetBool("Melee", false);
        }

        //If animator playing melee animation hold the sword in hand.
        if (isMeleeAnimationPlaying())
        {
            SetWeaponTransform(WeaponSlot.Melee, weaponHoldTransform);
        }
        //Else store the melee weapon
        else
        {
            SetWeaponTransform(WeaponSlot.Melee, meleeWeaponStoreTransform);
        }
    }
    void ShootingHandling()
    {
        bool aimInput = Input.GetButton("Fire2");

        //Set the animator state to aim
        animator.SetBool("Aim", aimInput);
        crossHair.GetComponent<Animator>().SetBool("Open", aimInput);

        aiming = aimInput;

        if (Input.GetButton("Fire2"))
        {
            ZoomInPOV();
            SetWeaponTransform(selectedSlot, weaponHoldTransform);

            switch (selectedSlot) 
            {
                case (WeaponSlot.Secondary):
                    animator.SetBool("Secondary Aim", true);
                    break;
            }

            if (Input.GetButton("Fire1"))
            {
                GetWeaponGameobjectFromSlot(selectedSlot).GetComponent<RangedWeapon>().Fire();
            }
        }
        else
        {
            ZoomOutPOV();
            SetWeaponTransform(selectedSlot, GetTransformFromSlot(selectedSlot));
            animator.SetBool("Secondary Aim", false);

        }

    }

    void ZoomInPOV()
    {
        if(targetPOV != zoomPOV)
            targetPOV = zoomPOV;
    }
    void ZoomOutPOV()
    {
        if (targetPOV != cameraOriginalPOV)
            targetPOV = cameraOriginalPOV;
    }

    void ToggleSelectedWeapon()
    {
        switch(selectedSlot)
        {
            case WeaponSlot.Primary:
                selectedSlot = WeaponSlot.Secondary;
                break;

            case WeaponSlot.Secondary:
                selectedSlot = WeaponSlot.Primary;
                break;
        }
    }
   
    void SetWeaponTransform(WeaponSlot slot, Transform transformPoint)
    {
        //Get weapon gameobject
        GameObject weapon = GetWeaponGameobjectFromSlot(slot);

        //Remove any parent
        weapon.transform.SetParent(null);
        //Match position
        weapon.transform.position = transformPoint.position;

        //Match rotation
        weapon.transform.rotation = transformPoint.rotation;
        
        //Parent weapon to transformPoint
        weapon.transform.SetParent(transformPoint);
    }

    public static void EquipWeapon(Weapon weapon)
    {
        switch (weapon.slot)
        {
            case WeaponSlot.Melee:
                meleeWeapon = weapon.gameObject;
                break;

            case WeaponSlot.Primary:
                primaryWeapon = weapon.gameObject;
                selectedSlot = weapon.slot;
                break;

            case WeaponSlot.Secondary:
                secondaryWeapon = weapon.gameObject;
                selectedSlot = weapon.slot;
                break;
        }
    }

    bool isMeleeAnimationPlaying()
    {
        return animator.GetCurrentAnimatorStateInfo(1).IsName("Melee 1") || animator.GetCurrentAnimatorStateInfo(1).IsName("Melee 2");
    }
    public bool isAiming()
    {
        return aiming;
    }
    bool isSlotEmpty(WeaponSlot slot)
    {
        GameObject slotObject = GetWeaponGameobjectFromSlot(slot);
        if(slotObject == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    GameObject GetWeaponGameobjectFromSlot(WeaponSlot slot)
    {
        //Variable to store retrived weapon
        GameObject weaponGameObject = null;

        //Switch-Case statement finding correct weapon
        switch (slot)
        {
            case WeaponSlot.Melee:
                weaponGameObject = meleeWeapon;
                break;

            case WeaponSlot.Primary:
                weaponGameObject = primaryWeapon;
                break;

            case WeaponSlot.Secondary:
                weaponGameObject = secondaryWeapon;
                break;

            case WeaponSlot.None:
                weaponGameObject = null;
                break;
        }

        //return weapon
        return weaponGameObject;
    }
    WeaponSlot GetSlotFromWeaponGameObject(GameObject weapon)
    {
        //Variable to store retrived weapon
        WeaponSlot slot = WeaponSlot.None;

        if (weapon == primaryWeapon) slot = WeaponSlot.Primary;
        if (weapon == secondaryWeapon) slot = WeaponSlot.Secondary;
        if (weapon == meleeWeapon) slot = WeaponSlot.Melee;

        //return weapon
        return slot;
    }
    Transform GetTransformFromSlot(WeaponSlot slot)
    {
        //Variable to store retrived weapon
        Transform transformPoint = transform;

        switch (slot)
        {
            case WeaponSlot.Primary:
                transformPoint = primaryWeaponStoreTransform;
                break;
            case WeaponSlot.Secondary:
                transformPoint = secondaryWeaponStoreTransform;
                break;
            case WeaponSlot.Melee:
                transformPoint = meleeWeaponStoreTransform;
                break;
        }

        //return weapon
        return transformPoint;
    }

    public void IncreaseAmmoCount(RangedWeapon.AmmoType type, int count)
    {
        ammoCount[(int)type] += count;
    }

    public void FireAnimation()
    {
        animator.SetTrigger("Fire");
    }

}




