using UnityEngine;
using System.Collections;

public class SlopeFixer : MonoBehaviour {

    SpriteRenderer sr;
    public Sprite spLeft;
    public Sprite spRight;
    int check = 0;
    int counter = 0;

	// Use this for initialization
	void Start () {
        sr = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void LateUpdate () {
        counter++; 
        RaycastHit2D[] hitsUp = Physics2D.RaycastAll(transform.position, Vector2.up, 1.28f);

        foreach (RaycastHit2D hit in hitsUp)
        {
            if (hit.collider.gameObject != gameObject)
            {
                if (hit.collider.tag == "Left Slope" && sr.sprite != spLeft)
                {
                    sr.sprite = spLeft;
                    check++;
                }
                if (hit.collider.tag == "Right Slope" && sr.sprite != spRight)
                {
                    sr.sprite = spRight;
                    check++;
                }
            }
        }

        RaycastHit2D[] hitsLeft = Physics2D.RaycastAll(transform.position, -Vector2.right, 1.28f);
        foreach (RaycastHit2D hit in hitsLeft)
        {
            if (hit.collider.gameObject != gameObject)
            {
                if (hit.collider.tag == "Left Slope")
                {
                    //hit.collider.transform.Rotate(0, 180, 0);
                    hit.collider.tag = "Right Slope";
                    if (sr.sprite == spLeft)
                    {
                        sr.sprite = spRight;
                        check++;
                    }
                }
            }
        }

        if (check >= 2)
        {
            RaycastHit2D[] hitsUp2 = Physics2D.RaycastAll(transform.position, Vector2.up, 1.28f);
            foreach (RaycastHit2D hit in hitsUp2)
            {
                if (hit.collider.gameObject != gameObject)
                {
                    if (hit.collider.tag == "Left Slope")
                    {
                        //hit.collider.transform.Rotate(0, 180, 0);
                        hit.collider.tag = "Right Slope";
                    }
                }
            }
        }

        if (counter == 2)
        {
            this.enabled = false;
        }
    }
}
