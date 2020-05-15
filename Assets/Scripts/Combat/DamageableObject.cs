using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DamageableObject : MonoBehaviour
{
    [Space]
    [Header("DamageableObject")]
    public DamageMatrix.DamObTypes damObType;
    public float currentHealth;
    public float maxHealth;

    public GameObject heathBarPrefab;
    public Transform healthBarPoint;
    GameObject healthBarInstance;
    bool healthBarCreated;
    
    public void CreateHealthBar()
    {
        healthBarInstance = Instantiate(heathBarPrefab);
        healthBarInstance.transform.position = healthBarPoint.position;
        healthBarInstance.transform.SetParent(transform);
        healthBarCreated = true;
    }

    public void UpdateHealthBar()
    {
        if(healthBarCreated){
            float healthRatio = currentHealth/maxHealth;
            healthBarInstance.transform.GetChild(1).GetComponent<RectTransform>().localScale = new Vector3(healthRatio,1,1);
        }
    }

    public void DealDamage(float damage)
    {
        currentHealth -= damage;
        if(!healthBarCreated) CreateHealthBar();
        UpdateHealthBar();
    }

    public float GetHealth(){
        return currentHealth;
    }

    public void Die(){
        Destroy(healthBarInstance);
        Destroy(gameObject);
    }

}
