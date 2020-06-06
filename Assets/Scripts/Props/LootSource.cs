using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using cakeslice;

public class LootSource : Interactable
{
    public enum LootType {Ammo, Health}
    public LootType type;

    public int ammoIndex;

    public int lootCount;

    public override void PerformInteraction(){
        LootSource interactableLootSource = GetComponent<LootSource>();
        GetComponent<AudioSource>().Play();
            switch (interactableLootSource.type)
            {
                case LootSource.LootType.Ammo:

                    GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombat>().IncreaseAmmoCount(interactableLootSource.ammoIndex,interactableLootSource.lootCount);
                    tag = "Untagged";
                    Destroy(GetComponent<Outline>());
                    GetComponent<Renderer>().materials[1].DisableKeyword("_EMISSION");
                    Destroy(interactableLootSource);
                    break;
            }
    }

}

