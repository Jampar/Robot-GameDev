using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : DamageableObject
{
    bool dissolve = false;
    float cutOffValue = 0.0f;

    public float dissolveSpeed;

    public override void Die()
    {
        dissolve = true;
    }

    void Update()
    {
        if(dissolve)
        {
            cutOffValue += dissolveSpeed * Time.deltaTime;
            GetComponent<Renderer>().material.SetFloat("_Cutoff", cutOffValue);
            Destroy(healthBarInstance);
            Destroy(gameObject,dissolveSpeed);
        }
    }
}
