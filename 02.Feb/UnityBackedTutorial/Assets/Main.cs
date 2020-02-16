using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public static Main Instance;

    [HideInInspector]
    public Web web;
    
    void Awake() {
        Instance = this;
        web = GetComponent<Web>();
    }
}
