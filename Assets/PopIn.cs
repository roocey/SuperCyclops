using UnityEngine;
using System.Collections;

public class PopIn : MonoBehaviour {
    bool ranOnce = false;

    void Update ()
    {
        if (transform.localScale.x <= 1.0f || transform.localScale.y <= 1.0f)
        {
            transform.localScale = new Vector3(transform.localScale.x + (Time.deltaTime * 2), transform.localScale.y + (Time.deltaTime * 2), transform.localScale.z);
            if (transform.localScale.x >= 1.0f || transform.localScale.y >= 1.0f)
            {
                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
        }
    }
	
	// Update is called once per frame
	void LateUpdate () {
        if (!ranOnce)
        {
            transform.localScale = new Vector3((1 / 3f), (1 / 3f), transform.localScale.z);
            ranOnce = true;
        }

    }
}
