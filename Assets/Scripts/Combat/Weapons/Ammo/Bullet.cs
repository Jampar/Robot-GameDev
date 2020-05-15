using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Projectile
{
    void OnCollisionEnter(Collision coll){
        Hit(coll);
    }
}
