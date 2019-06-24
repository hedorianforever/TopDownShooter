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
}
