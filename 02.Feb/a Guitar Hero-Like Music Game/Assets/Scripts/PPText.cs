using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PPText : MonoBehaviour
{
    public string scorePrefKeyName;

    Text text;

    private void Awake() {
        text = GetComponent<Text>();
    }

    void Update()
    {
        text.text = PlayerPrefs.GetInt(scorePrefKeyName) + "";
    }
}
