using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class Weapon : Interactable
{
    [Space]
    [Header("Weapon")]
    //The number of times the weapon fires in a second
    public float weaponFireRate;

    //Current cooldown timer for weapon fire rate
    float fireRateTimer = 0.0f;

    public int currentAmmoIndex;

    public float recoilAmount;

    public bool semiAutomatic;
    //The weapon can be fired
    bool canFire = true;
    //The weapon is currently being used
    bool selected;
    
    //The point the projectile launches from
    public Transform projectileFirePoint;
    //The velocity the projectile is launched at
    public float projectileVelocity;

    public AudioClip firingSound;
    public GameObject fireParticle;

    public int[] avaliableAmmoTypes;

    [HideInInspector]
    public List<Ammo> localAmmoSource = new List<Ammo>();

    [HideInInspector]
    public PlayerCombat playerCombat;

    public Sprite weaponIcon;

    void Update(){
        if(isSelected()){
            RunFireRateCooldown();
        }
    }

    public override void PerformInteraction()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombat>().AddToAvaliableWeapons(gameObject);
        Destroy(gameObject);
    }

    //Fire the weapon
    public void Fire()
    {
        if(canFire && GetAmmoCount() > 0 && localAmmoSource.Count > 0)
        {            
            //Instantiate a new projectile and store the projectile object
            Projectile projectileInstance = Instantiate(GetAmmoProjectile()).GetComponent<Projectile>();

            //Set the projectiles position and rotation ready to be lauched
            projectileInstance.SetLaunchPosition(projectileFirePoint);
            //Lauch the projectile
            projectileInstance.LaunchProjectile(projectileVelocity);

            projectileInstance.damageOrigin = playerCombat.gameObject;
            
            playerCombat.WeaponRecoil();
            
            if(firingSound != null)
            {
                GetComponent<AudioSource>().clip = firingSound;
                GetComponent<AudioSource>().Play();
            }

            ChangeAmmoCount(-1);

            //Reset the weapon fire rate timer
            ResetCooldownTimer();
        }
    }

    //Select the weapon
    public void SelectWeapon()
    {
        CreateLocalAmmoSource();
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

    void CreateLocalAmmoSource()
    {
        if(playerCombat == null) playerCombat = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombat>();
        
        foreach(Ammo ammoType in playerCombat.armouryObject.ammoBag){
            int ammoGlobalIndex = playerCombat.armouryObject.ammoBag.IndexOf(ammoType);
            
            foreach(int ammoIndex in avaliableAmmoTypes)
            {
                if(ammoGlobalIndex == ammoIndex)
                    localAmmoSource.Add(ammoType);
            }
            
        }
    }

    GameObject GetAmmoProjectile()
    {
        return localAmmoSource[currentAmmoIndex].projectile;
    }
    void ChangeAmmoCount(int change)
    {
        playerCombat.armouryObject.ammoBag.ToArray()[currentAmmoIndex].count += change;
    }
    int GetAmmoCount()
    {
        return playerCombat.armouryObject.ammoBag[currentAmmoIndex].count;
    }

    //Logic to control weapon fire rate
    public void RunFireRateCooldown(){
        if(fireRateTimer <= 0)
        {
            if(!semiAutomatic)
                canFire = true;
            else if(Input.GetButtonUp("Fire1"))
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
