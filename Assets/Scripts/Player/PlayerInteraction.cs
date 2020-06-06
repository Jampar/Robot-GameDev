using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class PlayerInteraction : MonoBehaviour
{

    float interactionRange = 5.0f;
    GameObject lastViewed;
    RaycastHit hit;

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


            if(Vector3.Distance(transform.position,hit.point) < interactionRange)
            {         
                if(viewed.GetComponent<Interactable>())
                {
                    Interactable interactable = viewed.GetComponent<Interactable>();

                    if(interactable != currentWeapon)
                    {
                        interactable.CreateTooltip();
                        if(interactable.outline)
                            OutlineGameObject(viewed);

                        if(Input.GetButtonDown("Interact"))
                        {
                            interactable.PerformInteraction();
                            interactable.DestroyToolTip();
                        }
                    }
                }
            }
            else
            {
                StopInteractingWithGameObject(viewed);
            }
        }
        if(lastViewed != null && viewed != lastViewed)
        {
            StopInteractingWithGameObject(lastViewed);
        } 

        lastViewed = viewed;
    }

    void OutlineGameObject(GameObject outlineGameObject){
        if(!outlineGameObject.GetComponent<Outline>())
            outlineGameObject.AddComponent<Outline>();
    }

    void StopInteractingWithGameObject(GameObject stopInteraction){
        if(stopInteraction.GetComponent<Outline>()) Destroy(stopInteraction.GetComponent<Outline>());
        if(stopInteraction.GetComponent<Interactable>()) stopInteraction.GetComponent<Interactable>().DestroyToolTip();
    }

    GameObject CurrentViewedObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if(Physics.Raycast(ray,out hit))
        {
            return hit.transform.gameObject;
        }
        
        return gameObject;
        
    }
}
