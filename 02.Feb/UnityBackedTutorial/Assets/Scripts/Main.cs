using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public static Main Instance;

    public Web web;
    public UserInfo userInfo;
    
    //UI
    [Header("UI")]
    public Login login;
    public GameObject Inventory;
    public GameObject userProfile;
    
    void Awake() {
        Instance = this;
        web = GetComponent<Web>();
        userInfo = GetComponent<UserInfo>();
    }
}
