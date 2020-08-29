using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DamageableObject : MonoBehaviour
{
    [Space]
    [Header("Damageable Object")]
    
    public string Name;
    public DamageMatrix.DamObTypes damObType;


    #region Health

    [Header("Health Variables")]

    public float currentHealth;
    public float maxHealth;

    #endregion


    #region Health Bar

    [Header("Health Bar")]

    public GameObject heathBarPrefab;
    public Transform healthBarPoint;
    [HideInInspector]   public GameObject healthBarInstance;
    bool healthBarCreated;

    #endregion   

    [Space]

    [HideInInspector]   public GameObject lastDamagedBy;




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
        Debug.Log(origin.name + " damaged: " + name + " for: "+ damage);

        lastDamagedBy = origin;
        currentHealth -= damage;

        if(!healthBarCreated) CreateHealthBar();
        UpdateHealthBar();

        if (currentHealth <= 0) Die();

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
