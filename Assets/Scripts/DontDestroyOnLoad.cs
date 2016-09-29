using UnityEngine;
using System.Collections;

public class DontDestroyOnLoad : MonoBehaviour {
    static bool created = false;

    void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(this);
            created = true;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
