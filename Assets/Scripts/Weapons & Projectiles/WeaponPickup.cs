using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class WeaponPickup : MonoBehaviour
{
    //it's a game object and not a Weapon script so it's easy to drag and drop it here
    //might need to change it 
    [SerializeField] Weapon weaponPrefab = default;
    [SerializeField] GameObject sparkVFX = default;
    [SerializeField] private AudioClip pickupSFX = default;


    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;
    

    private void Start()
    {
        if (weaponPrefab == null)
        {
            //Debug.LogError("NO WEAPON PREFAB ASSIGNED AT WEAPON PICKUP");
            return;
        } else if (weaponPrefab.GetComponent<Weapon>() == null)
        {
            //Debug.LogError("NOT A WEAPON AT WEAPON PICKUP");
            return;
        }

        SetWeapon(weaponPrefab);
    }

    public void SetWeapon(Weapon weaponPrefab)
    {
        this.weaponPrefab = weaponPrefab;
        //make pickup's sprite same as weapon's sprite
        //should have a shining effect though
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = weaponPrefab.GetComponent<SpriteRenderer>().sprite;

        //set collider's size to that of the sprite
        boxCollider2D = GetComponent<BoxCollider2D>();
        boxCollider2D.size = spriteRenderer.sprite.bounds.size;

        //change spark vfx size to fit the weapon
        var shape = sparkVFX.GetComponent<ParticleSystem>().shape;
        shape.scale = spriteRenderer.sprite.bounds.size;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            AudioManager.Instance.PlayClip(pickupSFX, 1, false);
            WeaponManager.Instance.TakeWeapon(weaponPrefab.GetComponent<Weapon>());
            Destroy(gameObject);
        }
    }


}
