using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : Weapon
{
    public enum AmmoType { Small, Large, Special };
    public AmmoType ammoType;

    public float fireRate;
    float fireWait;

    bool fireTimerStart;
    float fireTimer;

    bool canFire = true;
    public bool semiAutomatic;

    bool checkButtonRelease;
    bool hasBeenReleased;

    private void Start()
    {
        fireWait = (1 / fireRate);
    }

    private void Update()
    {
        if (fireTimerStart)
        {
            if (checkButtonRelease)
            {
                CheckFireButtonUp();
            }

            FireTimer();
        }
    }

    void FireTimer()
    {
        if (fireTimer <= 0)
        {
            if (semiAutomatic)
            {
                if (hasBeenReleased)
                {
                    EndFireTimer();
                }
            }
            else
            {
                EndFireTimer();
            }
        }
        else
        {
            fireTimer -= Time.deltaTime;
        }
    }
 
    void CheckFireButtonUp()
    {
        if (Input.GetButtonUp("Fire1"))
        {
            hasBeenReleased = true;
            checkButtonRelease = false;
        }
        else
        {
            hasBeenReleased = false;
        }
    }

    public void Fire()
    {
        if (canFire)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombat>().FireAnimation();
            Debug.Log(name + " Fired");
            StartFireTimer(); 
        }
    }

    void StartFireTimer()
    {
        fireTimerStart = true;
        fireTimer = fireWait;
        canFire = false;
        checkButtonRelease = true;
    }
    void EndFireTimer()
    {
        canFire = true;
        fireTimerStart = false;
    }
}
