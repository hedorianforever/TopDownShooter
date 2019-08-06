using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum WeaponType { Infinite, Pistol, MachineGun, Shotgun, Explosive, Laser, Sniper }

public class WeaponManager : Singleton<WeaponManager>
{
    [SerializeField]
    private int currentPistolAmmo = 0, currentMachineGunAmmo = 0, currentShotgunAmmo = 0, currentExplosiveAmmo = 0, currentLaserAmmo = 0, currentSniperAmmo = 0;

    [SerializeField]
    private int maxPistolAmmo = 100, maxMachineGunAmmo = 200, maxShotgunAmmo = 50, maxExplosiveAmmo = 40, maxLaserAmmo = 80, maxSniperAmmo = 60;

    [Header("Weapon related variables")]
    [SerializeField] List<Weapon> ownedWeapons = new List<Weapon>();
    //[SerializeField] Transform playerWeaponSlot = default;

    private Player player;
    private int equippedWeaponIndex = 0;
    private Weapon equippedWeapon;
    private bool isOnCooldown = false;
    private UIManager uiManager;

    private void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log("Level Loaded");
        //Debug.Log(scene.name);

        //if this weapon manager comes from the tutorial or reset scenes, destroy it to not keep the weapons   
        if (SceneManager.GetActiveScene().name == "Main Menu" || SceneManager.GetActiveScene().name == "Win Scene" || SceneManager.GetActiveScene().name == "Lose Scene")
        {
            Destroy(gameObject);
        }

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }
        if (player != null)
        {
            player.ChangeWeapon(ownedWeapons[equippedWeaponIndex]);
            equippedWeapon = ownedWeapons[equippedWeaponIndex];
            uiManager = UIManager.Instance;
            UpdateWeaponUI();
        }
    }

    public void InitPlayer(Player player)
    {
        this.player = player;
        player.ChangeWeapon(ownedWeapons[equippedWeaponIndex]);
        equippedWeapon = ownedWeapons[equippedWeaponIndex];
        UpdateWeaponUI();
    }

    public int GetCurrentAmmo(WeaponType wt)
    {
        switch (wt)
        {
            case WeaponType.Pistol: return currentPistolAmmo;
            case WeaponType.MachineGun: return currentMachineGunAmmo;
            case WeaponType.Shotgun: return currentShotgunAmmo;
            case WeaponType.Explosive: return currentExplosiveAmmo;
            case WeaponType.Laser: return currentLaserAmmo;
            case WeaponType.Sniper: return currentSniperAmmo;
            default: return 99;
        }
    }

    public int GetMaxAmmo(WeaponType wt)
    {
        switch (wt)
        {
            case WeaponType.Pistol: return maxPistolAmmo;
            case WeaponType.MachineGun: return maxMachineGunAmmo;
            case WeaponType.Shotgun: return maxShotgunAmmo;
            case WeaponType.Explosive: return maxExplosiveAmmo;
            case WeaponType.Laser: return maxLaserAmmo;
            case WeaponType.Sniper: return maxSniperAmmo;
            default: return 99;
        }
    }

    public void UseAmmo(WeaponType wt, int value)
    {
        switch (wt)
        {
            case WeaponType.Pistol:
                currentPistolAmmo -= value;
                break;
            case WeaponType.MachineGun:
                currentMachineGunAmmo -= value;
                break;
            case WeaponType.Shotgun:
                currentShotgunAmmo -= value;
                break;
            case WeaponType.Explosive:
                currentExplosiveAmmo -= value;
                break;
            case WeaponType.Laser:
                currentLaserAmmo -= value;
                break;
            case WeaponType.Sniper:
                currentSniperAmmo -= value;
                break;
        }
        UpdateWeaponUI();
    }

    public void AddAmmo(WeaponType wt, int value)
    {
        switch (wt)
        {
            case WeaponType.Pistol:
                currentPistolAmmo += value;
                currentPistolAmmo = Mathf.Clamp(currentPistolAmmo, 0, maxPistolAmmo);
                break;
            case WeaponType.MachineGun:
                currentMachineGunAmmo += value;
                currentMachineGunAmmo = Mathf.Clamp(currentMachineGunAmmo, 0, maxMachineGunAmmo);
                break;
            case WeaponType.Shotgun:
                currentShotgunAmmo += value;
                currentShotgunAmmo = Mathf.Clamp(currentShotgunAmmo, 0, maxShotgunAmmo);
                break;
            case WeaponType.Explosive:
                currentExplosiveAmmo += value;
                currentExplosiveAmmo = Mathf.Clamp(currentExplosiveAmmo, 0, maxExplosiveAmmo);
                break;
            case WeaponType.Laser:
                currentLaserAmmo += value;
                currentLaserAmmo = Mathf.Clamp(currentLaserAmmo, 0, maxLaserAmmo);
                break;
            case WeaponType.Sniper:
                currentSniperAmmo += value;
                currentSniperAmmo = Mathf.Clamp(currentSniperAmmo, 0, maxSniperAmmo);
                break;
            default:
                break;
        }
        UpdateWeaponUI();
    }

    public void TakeWeapon(Weapon weapon)
    {
        //test if already have that weapon. shouldnt happen, but just in case
        if (ownedWeapons.Contains(weapon))
        {
            EquipWeapon(weapon);
            return;
        }

        //add to owned weapons
        ownedWeapons.Add(weapon);
        equippedWeaponIndex++;
        EquipWeapon(weapon);
    }

    public void EquipWeapon(Weapon weapon)
    {
        equippedWeapon = weapon;
        player.ChangeWeapon(weapon);
        UpdateWeaponUI();
    }

    public void ChangeWeapon(float scrollWheelInput)
    {
        //this function is never called if the scrollwheelinput is 0
        Debug.Log("changing weapon");
        //go to previous weapon
        if (scrollWheelInput < 0)
        {
            //if first weapon, go to the last
            if (equippedWeaponIndex == 0)
            {
                equippedWeaponIndex = ownedWeapons.Count - 1;
            }
            else
            {
                equippedWeaponIndex--;
            }
        }
        //go to next weapon
        else
        {
            if (equippedWeaponIndex == ownedWeapons.Count - 1)
            {
                equippedWeaponIndex = 0;
            }
            else
            {
                equippedWeaponIndex++;
            }
        }

        EquipWeapon(ownedWeapons[equippedWeaponIndex]);
    }

    public void SetCooldown(float cooldownTime)
    {
        isOnCooldown = true;
        StartCoroutine(SetCooldownRoutine(cooldownTime));
    }

    IEnumerator SetCooldownRoutine(float cooldownTime)
    {
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
    }

    public bool IsOnCooldown()
    {
        return isOnCooldown;
    }

    public WeaponType GetEquippedWeaponType()
    {
        return equippedWeapon.MyWeaponType;
    }

    public void UpdateWeaponUI()
    {
        uiManager.UpdateEquippedWeaponUI(equippedWeapon, GetCurrentAmmo(equippedWeapon.MyWeaponType), GetMaxAmmo(equippedWeapon.MyWeaponType));
    }

}
