using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    [SerializeField] float ghostDelay = .5f;
    [SerializeField] float ghostLifetime = .5f;

    [SerializeField] GameObject ghost = default;

    [HideInInspector]
    public bool shouldMakeGhost = false;

    private float ghostDelaySeconds;

    private void Start()
    {
        ghostDelaySeconds = ghostDelay;    
    }

    private void Update()
    {
        if (shouldMakeGhost)
        {
            if (ghostDelaySeconds > 0)
            {
                ghostDelaySeconds -= Time.deltaTime;
            }
            else
            {
                //Generate a ghost
                GameObject currentGhost = Instantiate(ghost, transform.position, transform.rotation) as GameObject;
                Sprite currentSprite = GetComponent<SpriteRenderer>().sprite;
                currentGhost.GetComponent<SpriteRenderer>().sprite = currentSprite;
                currentGhost.GetComponent<SpriteRenderer>().flipX = GetComponent<SpriteRenderer>().flipX;
                Destroy(currentGhost, ghostLifetime);
                ghostDelaySeconds = ghostDelay;
            }
        }
        
    }
}
