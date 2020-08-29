using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cakeslice;

public abstract class Interactable : MonoBehaviour
{
    [Header("Interactable")]
    public bool interactable = true;

    [Space]

    [Header("Tool Tip")]
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
            created = true;
        }
    }

    void OutlineGameObject()
    {
        if (!GetComponent<cakeslice.Outline>())
            gameObject.AddComponent<cakeslice.Outline>();
    }

    public void Highlight()
    {
        CreateTooltip();
        OutlineGameObject();
    }

    public void RemoveInteractableHighlight()
    {
        DestroyToolTip();
        DestroyOutline();
    }

    public void DestroyToolTip()
    {
        if(created){
            Destroy(tooltipInstance.gameObject);
            created = false;
        }
    }

    void DestroyOutline()
    {
        if (GetComponent<cakeslice.Outline>()) Destroy(GetComponent<cakeslice.Outline>());
    }

    public virtual void PerformInteraction(){
        print("Interacted with " + name);
    }
    
}
