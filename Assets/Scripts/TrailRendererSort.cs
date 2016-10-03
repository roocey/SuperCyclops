using UnityEngine;
using System.Collections;

public class TrailRendererSort : MonoBehaviour {
    TrailRenderer trail;

	// Use this for initialization
	void Start () {
        trail = this.GetComponent<TrailRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        trail.sortingOrder = 2;
      //this.enabled = false;
	
	}
}
