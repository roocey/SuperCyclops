using UnityEngine;
using System.Collections;

public class RotateObject : MonoBehaviour {
    //rotate this object's z-axis by 180 degrees if a ground block is detected beneath it
    //object rotation is done instead of sprite y-flipping to move the collider as well as the sprite
    int layerMask;

	// Use this for initialization
	void Start () {
        layerMask = LayerMask.GetMask("Ground");
    }

    // Update is called once per frame
    void Update () {
        RaycastHit2D hitUp = Physics2D.Raycast(transform.position, Vector2.up, 1f, layerMask);
        RaycastHit2D hitDown = Physics2D.Raycast(transform.position, -Vector2.up, 1f, layerMask);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, 1f, layerMask);
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, -Vector2.right, 1f, layerMask);

        if (hitUp.collider != null)
        {
            if (transform.rotation.z != 180)
            {
                transform.Rotate(0, 0, 180);
                this.enabled = false;
                return;
            }
        }
        if (hitDown.collider != null)
        {
            if (transform.rotation.z != 0)
            {
                transform.Rotate(0, 0, 0);
                this.enabled = false;
                return;
            }
        }
        if (hitRight.collider != null)
        {
            if (transform.rotation.z != 90)
            {
                transform.Rotate(0, 0, 90);
                this.enabled = false;
                return;
            }
        }
        if (hitLeft.collider != null)
        {
            if (transform.rotation.z != 270)
            {
                transform.Rotate(0, 0, 270);
                this.enabled = false;
                return;
            }
        }
        this.enabled = false;

    }
}
