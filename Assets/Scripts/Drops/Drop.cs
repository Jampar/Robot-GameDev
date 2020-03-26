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

  float attractionForce = 2.0f;

  void Awake(){
    player = GameObject.FindGameObjectWithTag("Player");
  }

  // Update is called once per frame
  void Update()
  {

    if(Vector3.Distance(transform.position, player.transform.position) < 1f){

      if(type == DropType.Health){
        player.GetComponent<PlayerStats>().ChangeHealthValue(dropAmount);
      }
      if(type == DropType.Shield){
        player.GetComponent<PlayerStats>().ChangeShieldValue(dropAmount);
      }
      Destroy(gameObject);

    }

  }

}
