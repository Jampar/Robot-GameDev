using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class Ammo
{
    public GameObject projectile;
    public int count;
}

public class PlayerCombat : DamageableObject
{
    public bool aiming;
    
    Animator animator;
    Weapon currentWeapon;
    PlayerController playerController;

    public Transform weaponParent;

    public List<Ammo> ammoCap = new List<Ammo>();

    public List<GameObject> possibleWeapons = new List<GameObject>();
    List<GameObject> avaliableWeapons = new List<GameObject>();

    int currentWeaponIndex = 0;

    public bool ammoSelection;

    public Image crossHair;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();

        if(avaliableWeapons.Count > 0)
            EquipWeapon(0);
    }

    // Update is called once per frame
    void Update()
    {
        if(avaliableWeapons.Count > 0)
        {
            if(Input.GetButtonDown("Reload")) ammoSelection = !ammoSelection;

            if(ammoSelection)
            {
                if(Input.GetButtonDown("ReloadCycleRight")){
                    currentWeapon.currentAmmoIndex += 1;
                    if(currentWeapon.currentAmmoIndex >= ammoCap.Count) currentWeapon.currentAmmoIndex = 0;
                }
                if(Input.GetButtonDown("ReloadCycleLeft")){
                    currentWeapon.currentAmmoIndex -= 1;
                    if(currentWeapon.currentAmmoIndex < 0) currentWeapon.currentAmmoIndex = ammoCap.Count - 1;
                }
            }

           
            aiming = Input.GetMouseButton(1);    
            animator.SetBool("Aiming",aiming);
            if(aiming)
            {
                if(Input.GetMouseButton(0) && !playerController.isSprinting() && !ammoSelection) {
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
    }

    void EquipWeapon(int weaponIndex)
    {
        if(avaliableWeapons[weaponIndex] != currentWeapon)
        {
            currentWeaponIndex = weaponIndex;

            if(currentWeapon != null)
            {
                currentWeapon.UnselectWeapon();
                Destroy(currentWeapon.gameObject);
            }

            GameObject weaponGameObject = Instantiate(avaliableWeapons[weaponIndex]);
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
        GameObject prefab = possibleWeapons.Where(obj => obj.name == added.name).SingleOrDefault();
        prefab.GetComponent<Weapon>().playerCombat = this;
        avaliableWeapons.Add(prefab);
        if(avaliableWeapons.Count == 1) EquipWeapon(0);
    }

    public void AddTypeToAmmoCap(Ammo ammoType)
    {
        ammoCap.Add(ammoType);
    }
    
    public void IncreaseAmmoCount(int ammoIndex, int increaseCount){
        ammoCap.ToArray()[ammoIndex].count += increaseCount;
    }

    void EquipNeighbourWeapon(int direction)
    {
        int newIndex = currentWeaponIndex + direction;

        if(newIndex < 0)
        {
            newIndex = avaliableWeapons.Count -1;
        }
        else if(newIndex > avaliableWeapons.Count - 1)
        {
            newIndex = 0;
        }

        EquipWeapon(newIndex);
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




