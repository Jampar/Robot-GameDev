using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Projectile : MonoBehaviour
{
    public void SetLaunchPosition(Transform launchPoint){
        transform.position = launchPoint.position;
        transform.rotation = launchPoint.rotation;
    }

    public void LaunchProjectile(float velocity){
        GetComponent<Rigidbody>().velocity = transform.forward * velocity;
        Destroy(gameObject,10);
    }
}
