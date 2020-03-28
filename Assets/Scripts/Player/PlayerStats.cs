using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{

    float maxPlayerHealth = 100.0f;
    float currentPlayerHealth;

    float maxPlayerShield = 40.0f;
    float currentPlayerShield;

    public Animator healthAnim;
    public Text healthText;

    public Animator shieldAnim;
    public Text shieldText;
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
        healthText.text = Mathf.RoundToInt(currentPlayerHealth).ToString();

        int shieldRounded = (int)currentPlayerShield.ToNearestMultiple(10,FloatExtensions.ROUNDING.UP)/10;
        print(shieldRounded);
        shieldAnim.SetInteger("ShieldState", shieldRounded);
        shieldText.text = Mathf.RoundToInt(currentPlayerShield).ToString();
    }

    public bool ChangeHealthValue(float value){
      if((currentPlayerHealth + value) <= maxPlayerHealth){
        currentPlayerHealth += value;
        UpdateStatUI();
        return true;
      }
      return false;
    }
    public bool ChangeShieldValue(float value){
      if((currentPlayerShield + value) <= maxPlayerShield){
        currentPlayerShield += value;
        UpdateStatUI();
        return true;
      }
      return false;
    }
    public void SetHealthValue(float value){
      if(value <= maxPlayerHealth){
        currentPlayerHealth = value;
        UpdateStatUI();
      }
    }
    public void SetShieldValue(float value){
      if(value <= maxPlayerShield){
        currentPlayerShield = value;
        UpdateStatUI();
      }
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
