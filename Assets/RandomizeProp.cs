using UnityEngine;
using System.Collections;

public class RandomizeProp : MonoBehaviour {
    SpriteRenderer sr;
    public Sprite alt;

	// Use this for initialization
	void Start () {
        sr = GetComponent<SpriteRenderer>();
        if (Random.Range(0, 2) == 1)
        {
            sr.sprite = alt;
        }
        if (Random.Range(0, 2) == 1)
        {
            sr.flipX = true;
        }	
        else
        {
            sr.flipX = false;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
