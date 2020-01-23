using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{
    float fadeTime = 2f;
    
    Image image;

    public Text endScore;

    void Start()
    {
        image = GetComponent<Image>();
        gameObject.SetActive(false);
    }

    void Update()
    {
        
    }

    public void ToggleEndScore(float score) {
        gameObject.SetActive(true);
        endScore.text = ((int)score).ToString();

        StartCoroutine(Fade());
    }

    IEnumerator Fade() {
        Debug.Log("Fade()");
        float percent = 0;
        float speed = 1 / fadeTime;

        Color initialColor = image.color;

        while (percent < 1) {
            percent += Time.deltaTime * speed;
            image.color = Color.Lerp(Color.clear, initialColor, percent);
            yield return null;
        }
    }

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Menu() {
        SceneManager.LoadScene("Menu");
    }
}
