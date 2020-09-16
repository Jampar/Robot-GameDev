using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backpack : Interactable
{
    public override void PerformInteraction()
    {
        base.PerformInteraction();
        PlayerCombat.EquipBackpack(gameObject);
        MiniRobot.returnToSlot = true;
        interactable = false;
    }
}
