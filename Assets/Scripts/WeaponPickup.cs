using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class WeaponPickup : MonoBehaviour
{
    //it's a game object and not a Weapon script so it's easy to drag and drop it here
    //might need to change it 
    [SerializeField] GameObject weaponPrefab = default;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;

    private void Start()
    {
        if (weaponPrefab == null)
        {
            Debug.LogError("NO WEAPON PREFAB ASSIGNED AT WEAPON PICKUP");
            return;
        } else if (weaponPrefab.GetComponent<Weapon>() == null)
        {
            Debug.LogError("NOT A WEAPON AT WEAPON PICKUP");
            return;
        }

        //make pickup's sprite same as weapon's sprite
        //should have a shining effect though
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = weaponPrefab.GetComponent<SpriteRenderer>().sprite;

        //set collider's size to that of the sprite
        boxCollider2D = GetComponent<BoxCollider2D>();
        boxCollider2D.size = spriteRenderer.sprite.bounds.size;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            WeaponManager.Instance.TakeWeapon(weaponPrefab.GetComponent<Weapon>());
            Destroy(gameObject);
        }
    }


}
