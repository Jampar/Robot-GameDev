using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{

    float interactionRange = 3.0f;
    GameObject lastViewed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GameObject viewed = CurrentViewedObject();
        
        if(viewed != lastViewed){
            //viewed.AddComponent<Outline>();
            //Destroy(lastViewed.GetComponent<Outline>());
        }

        if(Input.GetButtonDown("Interact") && viewed != null && viewed.tag == "Interactable")
        {
            if(viewed.GetComponent<Weapon>()){
                GetComponent<PlayerCombat>().AddToAvaliableWeapons(viewed);
                Destroy(viewed.gameObject);
            }
        }
        lastViewed = viewed;
    }


    GameObject CurrentViewedObject()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if(Physics.Raycast(ray,out hit))
        {
            if((hit.point - transform.position).magnitude < interactionRange)
                return hit.transform.gameObject;
        }
        
        return null;
        
    }
}
