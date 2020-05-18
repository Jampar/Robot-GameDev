using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DamageMatrix
{
    static float[,] damageMatrix = new float[,] {
                                                {0.0f,20.0f},
                                                {0.0f,10.0f},
                                                {5.0f,20.0f},
                                                {5.0f,15.0f}
                                                };

    public enum DamageTypes
    {
        Impact,
        Explosive
    };

    public enum DamObTypes
    {
        Vegetation,
        Rock,
        Imp,
        Player
    };

    public static float GetDamage(DamObTypes objectType, DamageTypes damageType)
    {
        return damageMatrix[(int)objectType,(int)damageType];
        
    }
}
