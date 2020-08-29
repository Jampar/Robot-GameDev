using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasPlant : DamageableObject
{

    public Color gizmoColor;
    public float damageRadius;
    public float damageRate;

    public GameObject deathParticle;

    // Update is called once per frame
    void Update()
    {
        DealDamageToSurroundings();
    }

    void DealDamageToSurroundings()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, damageRadius);
        foreach(Collider coll in colls)
        {
            DamageableObject damageableObject = coll.GetComponent<DamageableObject>();
            if(damageableObject != null && damageableObject.gameObject != gameObject)
            {
                damageableObject.DealDamage(damageRate * Time.deltaTime, gameObject);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, damageRadius);
    }

    public override void Die()
    {
        GameObject particle = Instantiate(deathParticle);
        particle.transform.SetParent(null);
        particle.transform.position = transform.position;
        Destroy(particle);

        base.Die();
    }
}
