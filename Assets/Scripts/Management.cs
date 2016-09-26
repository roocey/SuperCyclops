using UnityEngine;
using System.Collections;

public class Management : MonoBehaviour {
    public int coinCounter = 0;

    void Awake ()
    {
        DontDestroyOnLoad(this);
    }
}
