using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{

    float maxPlayerHealth = 100.0f;
    float currentPlayerHealth;

    float maxPlayerShield = 50.0f;
    float currentPlayerShield;

    GameObject healthBar;

    // Start is called before the first frame update
    void Start()
    {
        //Setting Correct Start Values
        currentPlayerHealth = 20.0f;
        currentPlayerShield = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void ChangeHealthValue(float value){
        currentPlayerHealth += value;
    }
    public void ChangeShieldValue(float value){
        currentPlayerShield += value;
    }
    public void ChangeHealthCap(float additionHealth){
        maxPlayerHealth += additionHealth;
    }
    public void ChangeShieldCap(float additionShield){
        maxPlayerHealth += additionShield;
    }



}
