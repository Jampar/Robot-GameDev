using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{

    float maxPlayerHealth = 100.0f;
    float currentPlayerHealth;

    float maxPlayerShield = 50.0f;
    float currentPlayerShield;

    public Animator healthAnim;

    // Start is called before the first frame update
    void Start()
    {
        //Setting Correct Start Values
        SetHealthValue(20.0f);
        SetShieldValue(0.0f);
    }

    // Update is called once per frame
    void Update()
    {

    }


    void UpdateStatUI(){
        int healthRounded = (int)currentPlayerHealth.ToNearestMultiple(20);
        healthAnim.SetTrigger(healthRounded.ToString() + " Percent");
    }

    public void ChangeHealthValue(float value){
        currentPlayerHealth += value;
        UpdateStatUI();
    }
    public void ChangeShieldValue(float value){
        currentPlayerShield += value;
        UpdateStatUI();
    }
    public void SetHealthValue(float value){
        currentPlayerHealth = value;
        UpdateStatUI();
    }
    public void SetShieldValue(float value){
        currentPlayerShield = value;
        UpdateStatUI();
    }
    public void ChangeHealthCap(float additionHealth){
        maxPlayerHealth += additionHealth;
    }
    public void ChangeShieldCap(float additionShield){
        maxPlayerHealth += additionShield;
    }
}

public static class FloatExtensions
{
    public enum ROUNDING { UP, DOWN, CLOSEST }

    public static float ToNearestMultiple(this float f, int multiple, ROUNDING roundTowards = ROUNDING.CLOSEST)
    {
        f /= multiple;

        return (roundTowards == ROUNDING.UP ? Mathf.Ceil(f) : (roundTowards == ROUNDING.DOWN ? Mathf.Floor(f) : Mathf.Round(f))) * multiple;
    }

    /// <summary>
    /// Using a multiple with a maximum of two decimal places, will round to this value based on the ROUNDING method chosen
    /// </summary>
    public static float ToNearestMultiple(this float f, float multiple, ROUNDING roundTowards = ROUNDING.CLOSEST)
    {
        f = float.Parse((f * 100).ToString("f0"));
        multiple = float.Parse((multiple * 100).ToString("f0"));

        f /= multiple;

        f = (roundTowards == ROUNDING.UP ? Mathf.Ceil(f) : (roundTowards == ROUNDING.DOWN ? Mathf.Floor(f) : Mathf.Round(f))) * multiple;

        return f / 100;
    }
}
