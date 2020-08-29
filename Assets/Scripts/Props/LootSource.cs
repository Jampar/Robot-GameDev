using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using cakeslice;

public class LootSource : Interactable
{
    public enum LootType {Ammo, Health}
    public LootType type;

    public RangedWeapon.AmmoType ammoType;

    public int lootCount;

    public override void PerformInteraction(){
        GetComponent<AudioSource>().Play();
            switch (type)
            {
                case LootSource.LootType.Ammo:

                    GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombat>().IncreaseAmmoCount(ammoType, lootCount);
                    tag = "Untagged";
                    Destroy(GetComponent<Outline>());
                    GetComponent<Renderer>().materials[1].DisableKeyword("_EMISSION");
                    Destroy(gameObject);
                    break;
            }
    }

}

