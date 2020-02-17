using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfo : MonoBehaviour
{
    public string userID { get; private set; }
    public string userName;
    public string userPassword;
    public string Level;
    public string Coins;

    public void SetCredentials(string _userName, string _userPassword) {
        userName = _userName;
        userPassword = _userPassword;
    }

    public void SetID(string id) {
        userID = id;
    }
}
