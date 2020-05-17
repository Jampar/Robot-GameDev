using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tooltip", menuName = "Interactables/Tooltip", order = 1)]
public class InteractableTooltip : ScriptableObject
{
    public string Name;
    public string Description;
    public Sprite Icon;
}
