using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class PlayerInteraction : MonoBehaviour
{

    float interactionRange = 4.0f;
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
            if(Vector3.Distance(transform.position,viewed.transform.position) < interactionRange){         
                if(viewed.tag == "Interactable")
                {
                    if(!viewed.GetComponent<Outline>())
                        viewed.AddComponent<Outline>();

                    if(Input.GetButtonDown("Interact")){
                        InteractCorrectly(viewed);
                    }
                }
            }
        }

        if(lastViewed != null && viewed != lastViewed)
        {
            if(lastViewed.GetComponent<Outline>()) Destroy(lastViewed.GetComponent<Outline>());
        } 

        lastViewed = viewed;
    }

    void InteractCorrectly(GameObject interactable)
    {
        PlayerCombat playerCombat = GetComponent<PlayerCombat>();
        if(interactable.GetComponent<Weapon>())
        {
            playerCombat.AddToAvaliableWeapons(interactable);
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
