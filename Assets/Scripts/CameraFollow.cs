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

            transform.position = Vector2.Lerp(transform.position, transformToFollow.position, speed);
        }
        //SNAPS CAMERA TO PIXEL PERFECT POSITION; DOESNT SEEM TO FIX ANYTHING
        double newX = transform.position.x - (transform.position.x % .0625);
        double newY = transform.position.y - (transform.position.y % .0625);
        transform.position = new Vector2((float)newX, (float)newY);
    }
}
