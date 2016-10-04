using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerMovementController))]
public class PlayerCameraController : MonoBehaviour {

    Camera cam;
    EndLevel endOfLevel;
    PlayerMovementController pmc;
    Renderer rend;

    float camX;
    float camY;
    float playerStartX;
    float endOfLevelX;

	// Use this for initialization
	void Start () {
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        endOfLevel = (EndLevel)FindObjectOfType(typeof(EndLevel));
        endOfLevelX = endOfLevel.gameObject.transform.position.x - 9.0f;
        playerStartX = transform.position.x + 13.6f;
        pmc = this.GetComponent<PlayerMovementController>();
        rend = this.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update () {
        camX = transform.position.x + 5;
        if (playerStartX > camX)
        {
            camX = playerStartX;
        }
        if (camX > endOfLevelX)
        {
            camX = endOfLevelX;
        }
        //cam.transform.position = new Vector3(cam.transform.position.x, camY, cam.transform.position.z);
        Vector3 nextCamPosition = new Vector3(camX, camY, cam.transform.position.z);
        if (rend.isVisible)
        {
            cam.transform.position = Vector3.Lerp(cam.transform.position, nextCamPosition, Time.deltaTime * 1.33f);
        }
        else
        {
            cam.transform.position = Vector3.Lerp(cam.transform.position, nextCamPosition, Time.deltaTime * 2.67f);
        }
        if (pmc.isGrounded)
        {
            camY = transform.position.y - 0.5f;
            if (-0.9f > pmc.vertical && Mathf.Abs(pmc.horizontal) < 0.1f)
            {
                camY = transform.position.y - 1.75f;
            }
            if (camY < 7.75f)
            {
                camY = 7.75f;
            }
        }
        else
        {
            if (!rend.isVisible)
            {
                camY = transform.position.y;
            }
        }
        if (playerStartX - 16 > transform.position.x || transform.position.x > endOfLevelX + 11)
        {
            //Debug.Log("Out of bounds - reloading level!");
            pmc.RestartLevel();
        }
    }
}
