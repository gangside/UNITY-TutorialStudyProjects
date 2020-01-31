using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class CursorDetection : MonoBehaviour
{
    GraphicRaycaster gr;
    PointerEventData pointerEventData = new PointerEventData(null);

    public Transform currentCharacter;
    public Transform token;
    public Transform tokenPoint;

    public bool hasToken;

    void Start()
    {
        gr = GetComponentInParent<GraphicRaycaster>();
    }

    void Update()
    {
        pointerEventData.position = Camera.main.WorldToScreenPoint(transform.position);
        List<RaycastResult> results = new List<RaycastResult>();
        gr.Raycast(pointerEventData, results);

        TokenControl(token);

        if (hasToken) {
            if (results.Count > 0) {
                Transform raycaterCharacter = results[0].gameObject.transform;
                if (raycaterCharacter != currentCharacter) {

                    if(currentCharacter != null) {
                        currentCharacter.Find("Selected Border").GetComponent<Image>().DOKill();
                        currentCharacter.Find("Selected Border").GetComponent<Image>().color = Color.clear;
                    }
                    SetCurrentCharacter(raycaterCharacter);
                }
            }
            else {
                if (currentCharacter != null) {
                    currentCharacter.Find("Selected Border").GetComponent<Image>().DOKill();
                    currentCharacter.Find("Selected Border").GetComponent<Image>().color = Color.clear;
                    currentCharacter = null;
                }
            }
        }
    }

    void TokenControl(Transform token) {

        if (Input.GetKeyDown(KeyCode.Space)) {
            if (hasToken) {
                hasToken = false;
            }
            else {
                hasToken = true;
            }
        }

        if (hasToken) {
            token.position = tokenPoint.position;
        }
    }

    void SetCurrentCharacter(Transform t) {

        if (t != null) {
            t.Find("Selected Border").GetComponent<Image>().color = Color.white;
            t.Find("Selected Border").GetComponent<Image>().DOColor(Color.red, 0.8f).SetLoops(-1);
        }

        currentCharacter = t;

        //플레이어 슬롯을 채워줌
        if (t != null) {
            int index = t.GetSiblingIndex();
            Character character = SmashCSS.instance.characters[index];
            SmashCSS.instance.ShowCharacterInSlot(0, character);
            SmashCSS.instance.ConfirmCharacter(0, character);
        }
        else {
            SmashCSS.instance.ShowCharacterInSlot(0, null);
        }
    }

}
