using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Armoury", menuName = "Armoury", order = 1)]
public class ArmouryObject : ScriptableObject
{
    public List<Weapon> armoury = new List<Weapon>();
    public List<Ammo> ammoBag = new List<Ammo>();

}
