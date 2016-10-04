using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerMovementController))]
public class PlayerAnimationController : MonoBehaviour {

    Animator anim;
    PlayerMovementController pmc;

    // Use this for initialization
    void Start () {
        anim = this.GetComponent<Animator>();
        pmc = this.GetComponent<PlayerMovementController>();
    }

    // Update is called once per frame
    void Update () {
        if (pmc.isGrounded)
        {
            if (-0.9f > pmc.vertical && Mathf.Abs(pmc.horizontal) < 0.1f)
            {
                anim.SetInteger("Direction", 3);
            }
            else if (pmc.horizontal > 0)
            {
                anim.SetInteger("Direction", 1);
            }
            else if (pmc.horizontal < 0)
            {
                anim.SetInteger("Direction", -1);
            }
            else
            {
                anim.SetInteger("Direction", 0);
            }
        }
        else
        {
            if (pmc.horizontal >= 0)
            {
                anim.SetInteger("Direction", 2);
                anim.Play("playerJumping"); //overly redundant?
            }
            else if (pmc.horizontal < 0)
            {
                anim.SetInteger("Direction", -2);
                anim.Play("playerJumpingLeft"); //see above
            }
        }
	
	}
}
