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

    public Transform backpackPoint;
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

    public Image healthUI;

    #region Zoom Variables

    [Header("Zoom Variables")]

    float cameraOriginalPOV;
    public float zoomPOV = 40;
    float targetFOV;
    public float zoomTime = 4;

    #endregion

    bool fadeUI = false;

    static GameObject _playerInstance;
    static GameObject backpack;
    static bool equipBackpack;

    float recoilSpread;
    public float maxRecoil;
    bool recoilFull;

    float holdThreshold = 1.0f;
    float holdTimer;
    bool performedHeavyAttack;

    float healthLerpSpeed = 2.0f;
    
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
        freeLookCamera.m_CommonLens = true;
        cameraOriginalPOV = freeLookCamera.m_Lens.FieldOfView;
        ZoomOutPOV();

        //***************************************************      

        holdTimer = holdThreshold;
    }
     
    // Update is called once per frame
    void Update()
    {
        if (backpack)
        {
            if (equipBackpack) SetBackpackPosition();

            //Melee Handling ************************************
            if (!isSlotEmpty(WeaponSlot.Melee) && !Input.GetButton("Fire2"))
                MeleeHandling();

            if (!isSlotEmpty(WeaponSlot.Primary) && !isSlotEmpty(WeaponSlot.Secondary))
            {
                // Weapon Swap **************************************
                if (Input.GetKeyDown(KeyCode.X)) ToggleSelectedWeapon();
            }

            if (!isSlotEmpty(selectedSlot) && !isMeleeAnimationPlaying())
                //Shooting Handling *********************************
                ShootingHandling();
        }

        UiHandling();
    }

    void MeleeHandling()
    {
        //Get when the melee button is down
        bool meleeInput = Input.GetButton("Fire1");

        if (PlayerController.movementType != PlayerController.MovementTypeLookup.Sprint)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                //Set the animator state to do melee attack
                animator.SetTrigger("Melee");
            }

        }

        if (meleeInput && !performedHeavyAttack)
        {
            holdTimer -= Time.deltaTime;

            if (holdTimer <= 0.0f)
            {
                animator.SetTrigger("Heavy Hit");
                holdTimer = holdThreshold;
                performedHeavyAttack = true;
            }
        }
        else
        {
            holdTimer = holdThreshold;
            animator.SetBool("Charging", false);

            if (Input.GetButtonUp("Fire1"))
                performedHeavyAttack = false;
        }

        GetWeaponGameobjectFromSlot(WeaponSlot.Melee).GetComponent<MeleeWeapon>().isSwinging = isMeleeAnimationPlaying();


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

        freeLookCamera.m_Lens.FieldOfView = Mathf.Lerp(freeLookCamera.m_Lens.FieldOfView, targetFOV,zoomTime * Time.deltaTime);

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
                case (WeaponSlot.Primary):
                    animator.SetBool("Primary Aim", true);
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
            animator.SetBool("Primary Aim", false);
        }

    }
    void UiHandling()
    {
        healthUI.fillAmount = Mathf.Lerp(healthUI.fillAmount, currentHealth / maxHealth + maxHealth / 1000, healthLerpSpeed * Time.deltaTime); ;
    }

    void ZoomInPOV()
    {
        if(targetFOV != zoomPOV)
            targetFOV = zoomPOV;
    }
    void ZoomOutPOV()
    {
        if (targetFOV != cameraOriginalPOV)
            targetFOV = cameraOriginalPOV;
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

    public static void EquipBackpack(GameObject _backpack)
    {
        backpack = _backpack;
        equipBackpack = true;
    }

    void SetBackpackPosition()
    {
        backpack.transform.SetParent(backpackPoint);
        backpack.transform.localPosition = Vector3.zero;
        backpack.transform.localRotation = Quaternion.Euler(new Vector3(-90,0,-90));

        equipBackpack = false;
    }

    bool isMeleeAnimationPlaying()
    {
        return animator.GetCurrentAnimatorStateInfo(1).IsName("Melee") || 
               animator.GetCurrentAnimatorStateInfo(1).IsName("Melee 0") ||
               animator.GetCurrentAnimatorStateInfo(1).IsName("Skeleton|(new) Heavy Hit") ||
               animator.GetCurrentAnimatorStateInfo(1).IsName("Skeleton|(new) Melee Idle");
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




