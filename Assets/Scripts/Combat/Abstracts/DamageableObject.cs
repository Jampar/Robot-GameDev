using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DamageableObject : MonoBehaviour
{
    [Space]
    [Header("DamageableObject")]
    public string Name;
    public DamageMatrix.DamObTypes damObType;
    public float currentHealth;
    public float maxHealth;

    public GameObject heathBarPrefab;
    public Transform healthBarPoint;
    [HideInInspector]
    public GameObject healthBarInstance;
    bool healthBarCreated;

    [HideInInspector]
    public GameObject damagedBy;
    
    public void CreateHealthBar()
    {
        healthBarInstance = Instantiate(heathBarPrefab);
        healthBarInstance.transform.position = healthBarPoint.position;
        healthBarInstance.transform.Find("Name").GetComponent<Text>().text = Name;
        healthBarInstance.transform.SetParent(transform);
        healthBarInstance.transform.localRotation = Quaternion.Euler(Vector3.zero);
        healthBarCreated = true;
    }

    public void UpdateHealthBar()
    {
        if(healthBarCreated && healthBarInstance != null){
            float healthRatio = currentHealth/maxHealth;
            healthBarInstance.transform.Find("HealthGreen").GetComponent<RectTransform>().localScale = new Vector3(healthRatio,1,1);
        }
    }

    public void DealDamage(float damage,GameObject origin)
    {
        damagedBy = origin;
        currentHealth -= damage;
        if(!healthBarCreated) CreateHealthBar();
        UpdateHealthBar();
        if(currentHealth <= 0) Die();
    }

    public float GetHealth(){
        return currentHealth;
    }
    public float GetMaxHealth(){
        return maxHealth;
    }


    public virtual void Die(){
        Destroy(healthBarInstance);
        Destroy(gameObject);
    }

}
