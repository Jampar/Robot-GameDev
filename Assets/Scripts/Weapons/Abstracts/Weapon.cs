using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    //Base damage the weapon's projectile delivers 
    public float weaponDamage;
    //The number of times the weapon fires in a second
    public float weaponFireRate;

    //Current cooldown timer for weapon fire rate
    float fireRateTimer = 0.0f;

    //The weapon can be fired
    bool canFire;
    //The weapon is currently being used
    bool selected;
    
    //The projectile object the weapon fires
    public Projectile projectile;
    //The point the projectile launches from
    public Transform projectileFirePoint;
    //The velocity the projectile is launched at
    public float projectileVelocity;

    void Update(){
        if(isSelected()){
            RunFireRateCooldown();
        }
    }

    //Fire the weapon
    public void Fire()
    {
        if(canFire)
        {
            Debug.Log(gameObject.name + " Fired");
            
            //Instantiate a new projectile and store the projectile object
            Projectile projectileInstance = Instantiate(projectile.gameObject).GetComponent<Projectile>();

            //Set the projectiles position and rotation ready to be lauched
            projectileInstance.SetLaunchPosition(projectileFirePoint);
            //Lauch the projectile
            projectileInstance.LaunchProjectile(projectileVelocity);

            //Reset the weapon fire rate timer
            ResetCooldownTimer();
        }
    }

    //Select the weapon
    public void SelectWeapon()
    {
        selected = true;
    }
    //Unselect the weapon
    public void UnselectWeapon()
    {
        selected = false;
    }
    //Returns selected boolean
    public bool isSelected()
    {
        return selected;
    }

    //Logic to control weapon fire rate
    public void RunFireRateCooldown(){
        if(fireRateTimer <= 0)
        {
            canFire = true;
        }
        else
        {
            fireRateTimer -= Time.deltaTime;
        }
    }

    //Reset the fire rate timer
    void ResetCooldownTimer(){
        canFire = false;
        fireRateTimer = weaponFireRate;
    }
}
