using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    public float meleeDamage;
    public Transform meleeOrigin;
    bool hurtPlayer = false;
    public bool isSwinging;

    private void OnCollisionEnter(Collision collision)
    {
        if (isSwinging)
        {
            if (collision.transform.tag == "Player" && hurtPlayer || collision.transform.tag != "Player")
            {
                DamageableObject damageableObject = collision.gameObject.GetComponent<DamageableObject>();
                if (damageableObject)
                {
                    damageableObject.DealDamage(meleeDamage, gameObject);
                }
            }
        }
    }
}
