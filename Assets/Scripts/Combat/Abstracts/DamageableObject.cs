using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DamageableObject : MonoBehaviour
{
    [Header("Damageable Object")]
    [Space]

    #region Health

    public float currentHealth;
    public float maxHealth;

    #endregion  


    [Space]

    [HideInInspector]   public GameObject lastDamagedBy;


    public void DealDamage(float damage,GameObject origin)
    {        
        Debug.Log(origin.name + " damaged: " + name + " for: "+ damage);

        lastDamagedBy = origin;
        currentHealth -= damage;

        if (currentHealth <= 0) Die();

    }

    public float GetHealth(){
        return currentHealth;
    }

    public float GetMaxHealth(){
        return maxHealth;
    }


    public virtual void Die(){
        Destroy(gameObject);
    }

}
