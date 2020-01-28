using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Text highscore;

    private void Start() {
        highscore.text = "Highscore : " + PlayerPrefs.GetInt("Highscore");
    }

    public void Play() {
        SceneManager.LoadScene("Game");
    }
}
