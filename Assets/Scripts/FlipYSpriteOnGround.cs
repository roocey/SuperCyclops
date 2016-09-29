using UnityEngine;
using System.Collections;

public class FlipYSpriteOnGround : MonoBehaviour {
    //rotate this object's z-axis by 180 degrees if a ground block is detected beneath it
    //object rotation is done instead of sprite y-flipping to move the collider as well as the sprite
    int layerMask;
    FlipYSpriteOnGround flip;

	// Use this for initialization
	void Start () {
        layerMask = LayerMask.GetMask("Ground");
        flip = GetComponent<FlipYSpriteOnGround>();
    }

    // Update is called once per frame
    void Update () {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 1f, layerMask);
        if (hit.collider != null)
        {
            if (transform.rotation.z != 180)
            {
                transform.Rotate(0, 0, 180);
            }
        }
        flip.enabled = false;

    }
}
