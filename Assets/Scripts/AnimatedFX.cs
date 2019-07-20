using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatedFX : MonoBehaviour
{
    public void DestroyFX()
    {
        Destroy(gameObject);
    }
}
