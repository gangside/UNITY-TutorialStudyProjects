using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    int multiplier = 1;
    int streak = 0;


    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("Score", 0);
        PlayerPrefs.SetInt("RockMeter", 25);
        PlayerPrefs.SetInt("Streak", 0);
        PlayerPrefs.SetInt("HighStreak", 0);
        PlayerPrefs.SetInt("Mult", 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        ResetStreak();
        Destroy(collision.gameObject);
    }

    public void AddStreak() {
        if(PlayerPrefs.GetInt("RockMeter") + 1 < 50) {
            PlayerPrefs.SetInt("RockMeter", PlayerPrefs.GetInt("RockMeter")+1);
        }

        streak++;
        Debug.Log(streak);
        if (streak < 8) {
            multiplier = 1;
        }
        if(streak >= 8) {
            multiplier = 2;
        }
        if(streak >= 16) {
            multiplier = 3;
        }
        if(streak >= 24) {
            multiplier = 4;
        }

        if (streak > PlayerPrefs.GetInt("HighStreak")) {
            PlayerPrefs.SetInt("HighStreak", streak);
        }

        UpdateGUI();
    }

    public void ResetStreak() {
        streak = 0;
        multiplier = 1;
        PlayerPrefs.SetInt("RockMeter", PlayerPrefs.GetInt("RockMeter") - 2);
        if (PlayerPrefs.GetInt("RockMeter") < 0)
            Lose();
        UpdateGUI();
    }

    private void Lose() {
        Debug.Log("패배");
    }

    public void Win() {
        Debug.Log("승리");

    }

    void UpdateGUI() {
        PlayerPrefs.SetInt("Streak", streak);
        PlayerPrefs.SetInt("Mult", multiplier);
    }

    public int GetScore() {
        return 100 * multiplier;
    }
}
