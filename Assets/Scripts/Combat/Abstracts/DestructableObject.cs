using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : DamageableObject
{
    public override void Die()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            child.transform.SetParent(null);
            child.AddComponent<Rigidbody>();
        }

        base.Die();

    }
}
