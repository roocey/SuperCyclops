using UnityEngine;
using System.Collections;

public class LockCameraYPosition : MonoBehaviour {
    Camera cam;
    GameObject daddy;
    Vector3 newCamPosition;
    float takeCameraToDaddy = 13.0f;
    float counter = 0f;
    float camY = 7.75f;

	// Use this for initialization
	void Start () {
        cam = GetComponent<Camera>();
        daddy = transform.parent.gameObject;
    }

    // Update is called once per frame
    void LateUpdate () {
        counter += Time.deltaTime;
        if (counter >= 0.2f)
        {
            Debug.Log(daddy.transform.position.y);
            if (daddy.transform.position.y < 12.0f)
            {
                cam.transform.position = new Vector3(cam.transform.position.x, camY, cam.transform.position.z);
            }
            else
            {
                cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z);
            }
            //if (!daddy.isVisible)
            //{
            //  if (daddy.transform.position.y > camY)
            //{
            //  camY += 2.0f;
            //}
            //else
            //{
            //  camY -= 2.0f;
            //}
            //}
        }

        //counter += Time.deltaTime;
        //if (counter >= 0.33f)
        //{
        //  if (daddy.isVisible)
        //{
        //  cam.transform.position = new Vector3(cam.transform.position.x, 7.75f, cam.transform.position.z);
        //}
        //else
        //{
        //  cam.transform.position = new Vector3(cam.transform.position.x, takeCameraToDaddy, cam.transform.position.z);
        //}
        // }
    }
}
