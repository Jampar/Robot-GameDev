using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Interactable : MonoBehaviour
{
    [Header("Interactable")]
    public InteractableTooltip tooltip;
    public GameObject tooltipGameobject;
    public Transform tooltipPoint;
    GameObject tooltipInstance;
    bool created = false;

    public void CreateTooltip(){
        if(!created){
            tooltipInstance = Instantiate(tooltipGameobject);
            tooltipInstance.transform.GetChild(0).GetComponent<Text>().text = tooltip.Name;
            tooltipInstance.transform.GetChild(1).GetComponent<Text>().text = tooltip.Description;
            if(tooltip.Icon != null)
                tooltipInstance.transform.GetChild(2).GetComponent<Image>().sprite = tooltip.Icon;
            else{
                tooltipInstance.transform.GetChild(2).gameObject.SetActive(false);
            }
            tooltipInstance.transform.position = tooltipPoint.position;
            tooltipInstance.transform.SetParent(transform);
        }
        created = true;
    }
    public void DestroyToolTip()
    {
        if(created){
            Destroy(tooltipInstance.gameObject);
            created = false;
        }
    }
    
}
