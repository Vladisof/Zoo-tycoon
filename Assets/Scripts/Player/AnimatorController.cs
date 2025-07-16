using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions.Must;

public class AnimatorController : MonoBehaviour
{
    PlayerTouchMovement playerConntroller;
    Animator playerAnim;
    public float runningSpeed;
    public AudioManager audioManager;

    private void Awake()
    {
        playerAnim = GetComponent<Animator>();
        playerConntroller=GetComponent<PlayerTouchMovement>();
    }

    private void Update()
    {
        if(playerConntroller.currentSpeed > 0)
        {
            playerAnim.SetBool("IsitMoving", true);
            if(playerConntroller.currentSpeed < runningSpeed)
                playerAnim.SetBool("IsitFast", false);
             
            else
                playerAnim.SetBool("IsitFast", true);

        }
        else
        {
            playerAnim.SetBool("IsitMoving", false);
            playerAnim.SetBool("IsitFast", false);

        }
        
    }
}
