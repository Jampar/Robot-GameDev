using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class Weapon : Interactable
{
    [Space]
    [Header("Weapon")]

    public PlayerCombat.WeaponSlot slot;

    public override void PerformInteraction()
    {
        base.PerformInteraction();
        PlayerCombat.EquipWeapon(this);
        interactable = false;
    }
}
