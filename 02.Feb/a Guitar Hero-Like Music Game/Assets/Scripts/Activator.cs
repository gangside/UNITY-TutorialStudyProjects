using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activator : MonoBehaviour
{
    public bool createMode;

    public GameObject n;

    public KeyCode keyCode;
    public Color newColor;



    private bool active = false;

    private GameObject note;
    private GameManager gameManager;
    private SpriteRenderer sr;
    private Color old;


    private void Awake() {
        gameManager = FindObjectOfType<GameManager>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        old = sr.color;
    }

    void Update() {
        if (createMode && Input.GetKeyDown(keyCode)) {
            Instantiate(n, transform.position, Quaternion.identity);
            return;
        }

        Debug.Log(active);

        //키눌렀을때 컬러변환
        if (Input.GetKeyDown(keyCode)) {
            StartCoroutine(Pressed());
        }

        //노트타격
        if (Input.GetKeyDown(keyCode ) && active) {
            Destroy(note);
            gameManager.AddStreak();
            AddScore();
            active = false;
        }
        else if(Input.GetKeyDown(keyCode)&&!active){
            
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "WinNote") {

        }

        if(collision.gameObject.tag == "Note") {
            active = true;
            note = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        active = false;
        //gameManager.ResetStreak();
    }

    void AddScore() {
        PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score") + gameManager.GetScore());
    }


    IEnumerator Pressed() {
        sr.color = newColor;
        yield return new WaitForSeconds(0.05f);
        sr.color = old;
    }
}
