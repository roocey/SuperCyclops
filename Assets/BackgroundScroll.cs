using UnityEngine;
using System.Collections;

public class BackgroundScroll : MonoBehaviour {
    Renderer rend;

	// Use this for initialization
	void Start () {
        rend = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
        float horizontal = Input.GetAxis("Horizontal");
        Vector2 offset = new Vector2(Time.deltaTime * horizontal, 0);
        rend.material.mainTextureOffset = Vector2.Lerp(rend.material.mainTextureOffset, offset, Time.deltaTime);
	}
}
