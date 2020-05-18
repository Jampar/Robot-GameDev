using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(Rigidbody))]
public abstract class Projectile : MonoBehaviour
{
    public DamageMatrix.DamageTypes damageType;
    public bool isExplosive;
    public bool effectedByGravity;

    [Header("Only needed if explosive")]
    public GameObject explosionParticle;
    public float explosionRadius;
    public float explosionDamage;

    public GameObject damageOrigin;

    public void SetLaunchPosition(Transform launchPoint){
        transform.position = launchPoint.position;
        transform.rotation = launchPoint.rotation;
        GetComponent<Rigidbody>().useGravity = effectedByGravity;
    }

    public void LaunchProjectile(float velocity){
        GetComponent<Rigidbody>().velocity = transform.forward * velocity;
        Destroy(gameObject,2);
    }

    public void Hit(Collision collision)
    {   
        ApplyDamage(collision.gameObject,damageType,damageOrigin);
        if(isExplosive) Explode();
        Destroy(gameObject,0f);
    }

    public void Explode()
    {
        if(isExplosive)
        {
            GameObject explosion = Instantiate(explosionParticle);
            explosion.transform.position = transform.position;
            Destroy(explosion,1);

            Collider[] colls = Physics.OverlapSphere(transform.position,explosionRadius);
            foreach(Collider coll in colls)
            {
                ApplyDamage(coll.gameObject,DamageMatrix.DamageTypes.Explosive,damageOrigin);
            }
        }
    }

    void ApplyDamage(GameObject applicant, DamageMatrix.DamageTypes type,GameObject origin)
    {
        if(applicant.GetComponent<DamageableObject>())
        {

            DamageableObject damageable = applicant.GetComponent<DamageableObject>();
            float damage = DamageMatrix.GetDamage(damageable.damObType,type);
            
            damageable.DealDamage(damage, origin);
            
            if(damageable.GetHealth() <= 0.0f)
            {
                damageable.Die();
            }
        }
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position,0.2f);

        Gizmos.color = Color.red + Color.yellow;
        if(isExplosive) Gizmos.DrawWireSphere(transform.position,explosionRadius);
    }
}
