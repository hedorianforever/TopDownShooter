using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType { Infinite, Pistol, MachineGun, Shotgun, Explosive, Laser}

public class WeaponManager : Singleton<WeaponManager>
{
    [SerializeField]
    private int currentPistolAmmo = 0, currentMachineGunAmmo = 0, currentShotgunAmmo = 0, currentExplosiveAmmo = 0, currentLaserAmmo = 0;
    [SerializeField]
    private int maxPistolAmmo = 100, maxMachineGunAmmo = 200, maxShotgunAmmo = 50, maxExplosiveAmmo = 40, maxLaserAmmo = 80;

    public int GetCurrentAmmo(WeaponType wt)
    {
        switch (wt)
        {
            case WeaponType.Pistol: return currentPistolAmmo;
            case WeaponType.MachineGun: return currentMachineGunAmmo;
            case WeaponType.Shotgun: return currentShotgunAmmo;
            case WeaponType.Explosive: return currentExplosiveAmmo;
            case WeaponType.Laser: return currentLaserAmmo;
            default: return 1;
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
        }
    }

    public void AddAmmo(WeaponType wt, int value)
    {
        switch (wt)
        {
            case WeaponType.Pistol:
                currentPistolAmmo += value;
                Mathf.Clamp(currentPistolAmmo, 0, maxPistolAmmo);
                break;
            case WeaponType.MachineGun:
                currentMachineGunAmmo += value;
                Mathf.Clamp(currentMachineGunAmmo, 0, maxMachineGunAmmo);
                break;
            case WeaponType.Shotgun:
                currentShotgunAmmo += value;
                Mathf.Clamp(currentShotgunAmmo, 0, maxShotgunAmmo);
                break;
            case WeaponType.Explosive:
                currentExplosiveAmmo += value;
                Mathf.Clamp(currentExplosiveAmmo, 0, maxExplosiveAmmo);
                break;
            case WeaponType.Laser:
                currentLaserAmmo += value;
                Mathf.Clamp(currentLaserAmmo, 0, maxLaserAmmo);
                break;
        }
    }


}
