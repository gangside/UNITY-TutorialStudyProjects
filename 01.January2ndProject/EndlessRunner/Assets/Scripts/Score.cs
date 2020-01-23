using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Text scoreText;
    
    float score = 0f;
    int difficultyLevel = 1;
    int maxDifficultyLevel = 10;
    int scoreToNextLevel = 10;
    bool isDead = false;

    public DeathMenu deathMenu;
    PlayerMotor player;

    void Start()
    {
        player = FindObjectOfType<PlayerMotor>();
        player.OnDeath += PlayerDeath;
    }

    void Update()
    {
        if (isDead) {
            return;
        }

        score += Time.deltaTime;
        if(score > scoreToNextLevel) {
            LevelUpDifficulty();
        }
        scoreText.text = ((int)score * difficultyLevel).ToString();
    }

    private void LevelUpDifficulty() {
        if(difficultyLevel == maxDifficultyLevel) {
            return;
        }

        scoreToNextLevel *= 2;
        difficultyLevel++;

        Debug.Log(difficultyLevel);

        player.SetSpeed(difficultyLevel);
    }

    public void PlayerDeath() {
        isDead = true;
        deathMenu.ToggleEndScore((int)score * difficultyLevel);
    }
}
