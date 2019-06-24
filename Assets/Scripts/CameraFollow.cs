using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform transformToFollow = default;
    [SerializeField] float speed = 0.125f;

    private void Start()
    {
        transform.position = transformToFollow.position;
    }

    private void Update()
    {
        if (transformToFollow != null)
        {
            transform.position = Vector2.Lerp(transform.position, transformToFollow.position, speed);
        }
    }

    private void LateUpdate()
    {
        //SNAPS CAMERA TO PIXEL PERFECT POSITION; DOESNT SEEM TO FIX ANYTHING
        double newX = transform.position.x - (transform.position.x % .0625);
        double newY = transform.position.y - (transform.position.y % .0625);
        transform.position = new Vector2((float)newX, (float)newY);
    }
}
