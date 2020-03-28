using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : MonoBehaviour
{

  public enum DropType
  {
    Health,
    Shield,
    Ammo,
  };

  public DropType type;

  public float dropAmount;

  GameObject player;
  bool playerInRange;

  float collectRadius = 1.0f;

  Rigidbody rb;

  void Awake(){
    player = GameObject.FindGameObjectWithTag("Player");
    rb = GetComponent<Rigidbody>();
  }

  // Update is called once per frame
  void Update()
  {
    Collider[] colls = Physics.OverlapSphere(transform.position,collectRadius);
    foreach(Collider coll in colls){
      if(coll.gameObject == player){
        if(type == DropType.Health){
          if(player.GetComponent<PlayerStats>().ChangeHealthValue(dropAmount)){
            Destroy(gameObject);
          }
        }
        if(type == DropType.Shield){
          if(player.GetComponent<PlayerStats>().ChangeShieldValue(dropAmount)){
            Destroy(gameObject);
          }
        }
      }
    }
  }
  void OnDrawGizmos(){
    Color color = Color.yellow;
    color.a = 0.05f;

    Gizmos.color = color;
    Gizmos.DrawSphere(transform.position,collectRadius);
  }

}
