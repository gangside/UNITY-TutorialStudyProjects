using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuideMessage : MonoBehaviour
{
    public Text contentText;
    public Button okButton;
  
    private string currentMessage;

    private void Start() {
        okButton.onClick.AddListener(() =>
        {
            this.gameObject.SetActive(false);
        });
    }

    public void ChangeMessage(string showingMessage) {
        currentMessage = showingMessage;
        contentText.text = currentMessage;
    }
}