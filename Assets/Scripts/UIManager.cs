using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EZCameraShake;

public class UIManager : Singleton<UIManager>
{
    [Header("General variables")]
    [SerializeField] Text healthText = default;
    [SerializeField] Image hurtPanel = default;
    [SerializeField] Image dashCooldownImage = default;
    [SerializeField] Image equippedWeaponImage = default;
    [SerializeField] Text equippedWeaponAmmoText = default;

    [Header("Boss variables")]
    [SerializeField] Image bossHealthFill = default;

    public IEnumerator DashCooldownRoutine(float dashCooldown)
    {
        float fillPercent = 0;
        dashCooldownImage.fillAmount = 0;

        while (fillPercent < dashCooldown)
        {
            fillPercent += Time.deltaTime;
            //already appears fully charged for player
            if (fillPercent / dashCooldown >= .92f)
            {
                dashCooldownImage.fillAmount = 1;
                break;
            }
            dashCooldownImage.fillAmount = Mathf.Lerp(0, 1, fillPercent / dashCooldown);
            yield return null;
        }
    }

    public void UpdatePlayerHealthUI(float currentHealth, float maxHealth)
    {
        healthText.text = Mathf.Clamp(currentHealth, 0, 1000).ToString() + "/" + maxHealth.ToString();

        //if (currentHealth <= maxHealth / 2)
        //{
        //    //change sprite to half a heart
        //    //TODO
        //}
    }

    public IEnumerator PlayerHurtRoutine()
    {
        CameraShaker.Instance.ShakeOnce(4f, 10f, .1f, .3f);

        hurtPanel.color = new Color(hurtPanel.color.r, hurtPanel.color.g, hurtPanel.color.b, .4f);

        yield return new WaitForSeconds(.1f);

        hurtPanel.color = new Color(hurtPanel.color.r, hurtPanel.color.g, hurtPanel.color.b, 0);
    }

    public void UpdateEquippedWeaponUI(Weapon equippedWeapon, int currentAmmo, int maxAmmo)
    {
        if (equippedWeapon == null)
        {
            return;
        }

        equippedWeaponImage.sprite = equippedWeapon.GetComponent<SpriteRenderer>().sprite;
        if (equippedWeapon.MyWeaponType == WeaponType.Infinite)
        {
            equippedWeaponAmmoText.resizeTextForBestFit = true;
            equippedWeaponAmmoText.text = "∞";
        }
        else
        {
            equippedWeaponAmmoText.resizeTextForBestFit = false;
            equippedWeaponAmmoText.text = currentAmmo.ToString() + "/" + maxAmmo.ToString();
        }
    }

    public IEnumerator UpdateBossHealth(float maxHealth, float currentHealth)
    {
        if (bossHealthFill == null)
        {
            Debug.Log("TRYING TO UPDATE NON EXISTENT BOSS HEALTH");
            yield break;
        }

        bossHealthFill.color = Color.red;

        bossHealthFill.fillAmount = currentHealth / maxHealth;
        yield return new WaitForSeconds(.1f);

        bossHealthFill.color = Color.white;
    }
}
