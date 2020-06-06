using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : DamageableObject
{
    [SerializeField]
    GameObject explosion;
    [SerializeField]
    float explosion_radius;
    
    [SerializeField]
    Color gizmoColor;

    public override void Die()
    {
        GetComponent<Animator>().SetTrigger("Explode");
        Destroy(healthBarInstance);
    }

    public void Explode()
    {
        GameObject explosion_instance = Instantiate(explosion);
        explosion_instance.transform.position = transform.position;

        Collider[] colls = Physics.OverlapSphere(transform.position,explosion_radius);
        foreach(Collider coll in colls)
        {
            if(coll.GetComponent<DamageableObject>() && coll.gameObject != damagedBy && coll.gameObject != gameObject)
            {
                DamageableObject damageableObject = coll.GetComponent<DamageableObject>();
                float damage = DamageMatrix.GetDamage(damObType,DamageMatrix.DamageTypes.Explosive);
                damageableObject.DealDamage(damage,gameObject);
            }

            if(coll.GetComponent<Rigidbody>()){
                coll.GetComponent<Rigidbody>().AddExplosionForce(10,transform.position,explosion_radius);
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmos() {
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position,explosion_radius);
    }
}
