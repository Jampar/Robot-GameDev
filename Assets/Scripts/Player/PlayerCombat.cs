using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public bool aiming;
    
    Animator animator;
    Weapon currentWeapon;

    public Transform weaponParent;

    public List<GameObject> avaliableWeapons = new List<GameObject>();

    int currentWeaponIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        EquipWeapon(0);
    }

    // Update is called once per frame
    void Update()
    {
        aiming = Input.GetMouseButton(1);    
        animator.SetBool("Aiming",aiming);

        if(aiming && Input.GetMouseButton(0)){
            currentWeapon.Fire();
        }  

        if(!aiming){
            Vector2 scrollDelta = Input.mouseScrollDelta;
            if(scrollDelta.sqrMagnitude !=0){
                EquipNeighbourWeapon(Mathf.RoundToInt(scrollDelta.y));
            }
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
        avaliableWeapons.Add(added);
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
}
