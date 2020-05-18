﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class PlayerInteraction : MonoBehaviour
{

    float interactionRange = 5.0f;
    GameObject lastViewed;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GameObject viewed = CurrentViewedObject();

        if(viewed != null)
        {   
            Weapon currentWeapon = null;
            if(GetComponent<PlayerCombat>().isWeaponEquipped())
                currentWeapon = GetComponent<PlayerCombat>().GetCurrentWeapon();

            if(Vector3.Distance(transform.position,viewed.transform.position) < interactionRange){         
                if(viewed.GetComponent<Interactable>())
                {
                    Interactable interactable = viewed.GetComponent<Interactable>();
                    if(interactable != currentWeapon){
                        interactable.CreateTooltip();

                        if(!viewed.GetComponent<Outline>())
                            viewed.AddComponent<Outline>();

                        if(Input.GetButtonDown("Interact")){
                            InteractCorrectly(interactable);
                            interactable.DestroyToolTip();
                        }
                    }
                }
            }
            else
            {
                if(viewed.GetComponent<Outline>()) Destroy(viewed.GetComponent<Outline>());
                if(viewed.GetComponent<Interactable>()) viewed.GetComponent<Interactable>().DestroyToolTip();
            }
        }

        if(lastViewed != null && viewed != lastViewed)
        {
            if(lastViewed.GetComponent<Outline>()) Destroy(lastViewed.GetComponent<Outline>());
            if(lastViewed.GetComponent<Interactable>()) lastViewed.GetComponent<Interactable>().DestroyToolTip();
        } 

        lastViewed = viewed;
    }

    void InteractCorrectly(Interactable interactable)
    {
        PlayerCombat playerCombat = GetComponent<PlayerCombat>();
      
        if(interactable.GetComponent<Weapon>())
        {
            playerCombat.AddToAvaliableWeapons(interactable.gameObject);
            Destroy(interactable.gameObject);
        }

        if(interactable.GetComponent<LootSource>())
        {
            LootSource interactableLootSource = interactable.GetComponent<LootSource>();

            switch (interactableLootSource.type)
            {
                case LootSource.LootType.Ammo:

                    playerCombat.IncreaseAmmoCount(interactableLootSource.ammoIndex,interactableLootSource.lootCount);
                    interactable.tag = "Untagged";
                    Destroy(interactable.GetComponent<Outline>());
                    interactable.GetComponent<Renderer>().materials[1].DisableKeyword("_EMISSION");
                    Destroy(interactable);
                    break;
            }
        }
    }


    GameObject CurrentViewedObject()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if(Physics.Raycast(ray,out hit))
        {
            return hit.transform.gameObject;
        }
        
        return gameObject;
        
    }
}
