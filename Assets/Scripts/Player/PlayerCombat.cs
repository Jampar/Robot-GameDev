using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCombat : DamageableObject
{
    public bool aiming;
    
    Animator animator;
    Weapon currentWeapon;
    PlayerController playerController;

    public Transform weaponParent;

    public List<GameObject> possibleWeapons = new List<GameObject>();
    List<GameObject> avaliableWeapons = new List<GameObject>();

    int currentWeaponIndex = 0;

    public int defaultFOV = 60;
    public int zoomFOV = 50;

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
            aiming = Input.GetMouseButton(1);    
            animator.SetBool("Aiming",aiming);
            if(aiming){
                if(Input.GetMouseButton(0) && !playerController.isSprinting()) {
                    currentWeapon.Fire();
                }  
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
        avaliableWeapons.Add(prefab);
        if(avaliableWeapons.Count == 1) EquipWeapon(0);
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
        RaycastHit hit;
        Vector3 targetPos = Vector3.zero;

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
}




