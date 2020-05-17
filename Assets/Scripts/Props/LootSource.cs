using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LootSource : Interactable
{
    public enum LootType {Ammo, Health}
    public LootType type;

    public int ammoIndex;

    public int lootCount;

}

