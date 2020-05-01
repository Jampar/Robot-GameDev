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

  Quaternion startRotation;
  Quaternion targetRotation;
  float rotationSpeed = 1.0f;

  void Awake(){
    player = GameObject.FindGameObjectWithTag("Player");
    rb = GetComponent<Rigidbody>();

    startRotation = transform.rotation;
    targetRotation = Random.rotation;
  }

  // Update is called once per frame
  void Update()
  {
    DropRotation();

    if(CheckForPlayerPickup()){
        ApplyDropType();
    }
  }

  bool CheckForPlayerPickup(){
    Collider[] colls = Physics.OverlapSphere(transform.position,collectRadius);
    foreach(Collider coll in colls){
      if(coll.gameObject == player){
        return true;
      }
    }
    return false;
  }
  void ApplyDropType(){
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
  void DropRotation(){
    transform.rotation = Quaternion.Slerp(startRotation, targetRotation, rotationSpeed * Time.deltaTime);
  }

  void OnDrawGizmos(){
    Color color = Color.yellow;
    color.a = 0.05f;

    Gizmos.color = color;
    Gizmos.DrawSphere(transform.position,collectRadius);
  }

}
