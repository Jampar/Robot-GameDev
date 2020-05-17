using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LootSource))]
public class LootSourceEditor : Editor {
 
    LootSource source;

    void OnEnable()
    {
        source = (LootSource)target;
    }

    public override void OnInspectorGUI()
    {
        source.tooltip = (InteractableTooltip)EditorGUILayout.ObjectField("Tooltip",source.tooltip, typeof(ScriptableObject), true);
        source.tooltipPoint = (Transform)EditorGUILayout.ObjectField("Tooltip Point",source.tooltipPoint, typeof(Transform), true);
        source.tooltipGameobject = (GameObject)EditorGUILayout.ObjectField("Tooltip GameObject",source.tooltipGameobject, typeof(GameObject), true);

        source.type = (LootSource.LootType)EditorGUILayout.EnumPopup("Loot Type", source.type);
        switch(source.type)
        {
            case LootSource.LootType.Ammo:
                source.ammoIndex = EditorGUILayout.IntField("Ammo Index", source.ammoIndex);
                break;

            case LootSource.LootType.Health:
                break;

        }
        source.lootCount = EditorGUILayout.IntField("Loot Count", source.lootCount);
    }

 }
