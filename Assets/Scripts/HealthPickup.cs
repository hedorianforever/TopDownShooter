using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class HealthPickup : MonoBehaviour
{
    [SerializeField] int healAmount = 3;
    [Range(0, 1)] //if zero, there will be no health drops, if 1, there can be health drops all the time
    [SerializeField] float playerHealthPercentageThreshold = .7f;
    [SerializeField] float timeToBeginFadeAway = 5;
    [SerializeField] float timeToFadeAway = 1.6f;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Player playerScript = player.GetComponent<Player>();
            //if player's health is bigger than the threshold, destroy the pickup (it shouldn't be dropped)
            if (playerScript.GetCurrentHealth() > playerScript.GetMaxHealth() * playerHealthPercentageThreshold)
            {
                Destroy(gameObject);
            }
        }
        Invoke("FadeAway", timeToBeginFadeAway);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Player player = collision.GetComponent<Player>();
            if (player.IsFullHealth())
            {
                return;
            } else
            {
                player.Heal(healAmount);
                Destroy(gameObject);
            }
        }
    }

    private void FadeAway()
    {
        StartCoroutine(FadeAwayRoutine());
    }

    IEnumerator FadeAwayRoutine()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color originalColor = sr.color;

        float t = 0;
        //blink until disappearing
        while (t <= timeToFadeAway)
        {
            if (sr.color == originalColor)
            {
                sr.color = Color.clear;
            } else
            {
                sr.color = originalColor;
            }
            yield return new WaitForSeconds(.3f);
            t += .3f;
        }
        Destroy(gameObject);
    }
}
