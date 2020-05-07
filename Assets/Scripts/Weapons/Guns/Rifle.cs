using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : Weapon
{
    void Update(){
        if(isSelected()){
            RunFireRateCooldown();
        }
    }
}
