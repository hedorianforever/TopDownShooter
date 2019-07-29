using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform transformToFollow = default;
    [SerializeField] float speed = 0.125f;

    //in case I want to clamp the map
    //[SerializeField] float minX, maxX, minY, maxY;

    private void Start()
    {
        transform.position = transformToFollow.position;
    }

    private void Update()
    {
        if (transformToFollow == null)
        {
            transformToFollow = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    private void LateUpdate()
    {
        if (transformToFollow != null)
        {
            //float clampedX = Mathf.Clamp(transformToFollow.position.x, minX, maxX);
            //float clampedY = Mathf.Clamp(transformToFollow.position.y, minY, maxY);

            //if following the player, the camera will follow a mid point between the mouse and the player
            if (transformToFollow.tag == "Player")
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transformToFollow.position;
                Vector2 midPoint = (Vector2)transformToFollow.position + (mousePos * .25f);

                transform.position = midPoint;
            }
            else
            {
                transform.position = Vector2.Lerp(transform.position, transformToFollow.position, speed);
            }
        }
        //SNAPS CAMERA TO PIXEL PERFECT POSITION; DOESNT SEEM TO FIX ANYTHING
        double newX = transform.position.x - (transform.position.x % .0625/4);
        double newY = transform.position.y - (transform.position.y % .0625/4);
        transform.position = new Vector2((float)newX, (float)newY);
    }
}
