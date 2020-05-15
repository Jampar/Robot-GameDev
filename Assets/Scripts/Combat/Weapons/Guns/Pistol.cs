using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Weapon
{
    void Update()
    {
        if(isSelected()){
            RunFireRateCooldown();
        }
    }
}
