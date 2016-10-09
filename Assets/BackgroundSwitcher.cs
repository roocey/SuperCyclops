using UnityEngine;
using System.Collections;

public class BackgroundSwitcher : MonoBehaviour {
    SpriteRenderer sr;

    public Sprite sp;

	// Use this for initialization
	void Start () {
        sr = GameObject.FindWithTag("Background").GetComponent<SpriteRenderer>();
        if (sr != null && sp)
        {
            sr.sprite = sp;
        }
    }
}
