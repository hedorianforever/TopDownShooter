using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Animator anim;
    [SerializeField] AudioClip hoverSound;
    [SerializeField] AudioClip clickSound;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    //Detect if the Cursor starts to pass over the GameObject
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        //Output to console the GameObject's name and the following message
        //Debug.Log("Cursor Entering " + name + " GameObject");
        anim.SetBool("hovering", true);
        AudioManager.Instance.PlayClip(hoverSound, 1, false); 
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        //Output the following message with the GameObject's name
        //Debug.Log("Cursor Exiting " + name + " GameObject");
        anim.SetBool("hovering", false);
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        AudioManager.Instance.PlayClip(clickSound, 1, false);
    }


    
}
