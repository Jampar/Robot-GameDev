using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class Ammo
{
    public string ammoName;
    public GameObject projectile;
    public int count;
    public Sprite ammoIcon;
}

public class PlayerCombat : DamageableObject
{
    public bool aiming;
    
    Animator animator;
    Weapon currentWeapon;
    PlayerController playerController;

    public Transform weaponParent;

    public ArmouryObject armouryObject;
    List<GameObject> weapons = new List<GameObject>();

    int currentWeaponIndex = 0;

    public bool ammoSelection;

    public Image crossHair;
    public GameObject weaponUI;
    Animator weaponUIAnim;

    bool fadeUI = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        weaponUIAnim = weaponUI.GetComponent<Animator>();

        if(weapons.Count > 0)
            EquipWeapon(0);

         WipeAmmoStocks();
    }

    void WipeAmmoStocks(){
        foreach(Ammo ammo in armouryObject.ammoBag){
            ammo.count = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(weapons.Count > 0)
        {
            if(Input.GetButtonDown("Reload")) ammoSelection = !ammoSelection;
            weaponUIAnim.SetBool("Select Ammo", ammoSelection);

            if(ammoSelection)
            {
                if(Input.GetButtonDown("ReloadCycleRight")){
                    weaponUIAnim.SetTrigger("Clicked E");
                    currentWeapon.currentAmmoIndex += 1;
                    if(currentWeapon.currentAmmoIndex >= currentWeapon.localAmmoSource.Count) currentWeapon.currentAmmoIndex = 0;
                }
                if(Input.GetButtonDown("ReloadCycleLeft")){
                    weaponUIAnim.SetTrigger("Clicked Q");
                    currentWeapon.currentAmmoIndex -= 1;
                    if(currentWeapon.currentAmmoIndex < 0) currentWeapon.currentAmmoIndex = currentWeapon.localAmmoSource.Count - 1;
                }
            }

           
            aiming = Input.GetButton("Fire2"); 
            if(!ammoSelection)  
                animator.SetBool("Aiming",aiming);

            if(aiming)
            {
                fadeUI = true;
                if(Input.GetButton("Fire1") && !playerController.isSprinting() && !ammoSelection) {
                    currentWeapon.Fire();
                }  

                if(isCrossHairHostile())
                {
                    crossHair.color = Color.red;
                }
                else
                {
                    crossHair.color = Color.gray;
                }

            }
            else
            {
                crossHair.color = Color.gray;
            }

            if(!aiming){
                Vector2 scrollDelta = Input.mouseScrollDelta;
                if(scrollDelta.sqrMagnitude !=0){
                    EquipNeighbourWeapon(Mathf.RoundToInt(scrollDelta.y));
                }
            }
            
            currentWeapon.projectileFirePoint.LookAt(AimDirection());
        }

        UpdateWeaponUI();
    }

    void UpdateWeaponUI()
    {
        if(isWeaponEquipped())
        {
            weaponUI.SetActive(true);
            weaponUI.transform.Find("Count").GetComponent<Text>().text = currentWeapon.localAmmoSource.ToArray()[currentWeapon.currentAmmoIndex].count.ToString();
            weaponUI.transform.Find("Ammo Icon").GetComponent<Image>().sprite = currentWeapon.localAmmoSource.ToArray()[currentWeapon.currentAmmoIndex].ammoIcon;
            weaponUI.transform.Find("Weapon Icon").GetComponent<Image>().sprite = currentWeapon.weaponIcon;
            

            if(!ammoSelection){
                weaponUIAnim.SetBool("Fade In",fadeUI);
                weaponUIAnim.SetBool("Fade Out",!fadeUI);
            }
            else
            {
                weaponUI.transform.Find("Ammo Icon").Find("Type").GetComponent<Text>().text = currentWeapon.localAmmoSource.ToArray()[currentWeapon.currentAmmoIndex].ammoName;
            }

        }
        else
        {
            weaponUI.SetActive(false);
        }
        fadeUI = false;
    }

    void EquipWeapon(int weaponIndex)
    {
        if(weapons[weaponIndex] != currentWeapon)
        {
            currentWeaponIndex = weaponIndex;

            if(isWeaponEquipped())
            {
                currentWeapon.UnselectWeapon();
                Destroy(currentWeapon.gameObject);
            }

            GameObject weaponGameObject = Instantiate(weapons[weaponIndex]);
            Weapon newWeapon = weaponGameObject.GetComponent<Weapon>();
            
            weaponGameObject.name = weaponGameObject.name.Substring(0,weaponGameObject.name.Length - "(Clone)".Length);
            weaponGameObject.transform.SetParent(weaponParent);
            weaponGameObject.transform.localPosition = Vector3.zero;
            weaponGameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);

            newWeapon.SelectWeapon();
            currentWeapon = newWeapon;

            Debug.Log(currentWeapon.name + " Equipped");
        }
    }

    public void AddToAvaliableWeapons(GameObject added){

        print(added.name);
        GameObject prefab = armouryObject.armoury.Where(obj => obj.name == added.name).SingleOrDefault().gameObject;
        prefab.GetComponent<Weapon>().playerCombat = this;
        weapons.Add(prefab);
        if(weapons.Count == 1) EquipWeapon(0);
        fadeUI = true;
    }

    public void AddTypeToAmmoCap(Ammo ammoType)
    {
        armouryObject.ammoBag.Add(ammoType);
    }
    
    public void IncreaseAmmoCount(int ammoIndex, int increaseCount){
        armouryObject.ammoBag.ToArray()[ammoIndex].count += increaseCount;
        fadeUI = true;
        
    }

    void EquipNeighbourWeapon(int direction)
    {
        int newIndex = currentWeaponIndex + direction;

        if(newIndex < 0)
        {
            newIndex = weapons.Count -1;
        }
        else if(newIndex > weapons.Count - 1)
        {
            newIndex = 0;
        }

        EquipWeapon(newIndex);
        fadeUI = true;
    }

    Vector3 AimDirection()
    {
        Vector3 targetPos = Vector3.zero;
        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if(Physics.Raycast(ray, out hit))
        {
            targetPos = hit.point;
            Debug.DrawLine(weaponParent.position, targetPos, Color.black);
        }
        else  
        {
            targetPos = Camera.main.transform.forward * 1000;
            Debug.DrawLine(weaponParent.position, targetPos, Color.white);
        }

        return targetPos;
    }

    public Weapon GetCurrentWeapon()
    {
        if(isWeaponEquipped())
            return currentWeapon;
        else return null;
    }

    public bool isWeaponEquipped(){
        if(currentWeapon != null)  return true;
        else return false;
    }


    bool isCrossHairHostile()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if(Physics.Raycast(ray, out hit))
        {
            if(hit.transform.GetComponent<DamageableObject>())  return true;
        }
        return false;

    }
}




