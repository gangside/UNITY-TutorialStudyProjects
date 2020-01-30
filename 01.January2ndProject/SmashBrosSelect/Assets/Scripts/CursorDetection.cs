using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CursorDetection : MonoBehaviour
{
    GraphicRaycaster gr;
    PointerEventData pointerEventData = new PointerEventData(null);

    public Transform currentCharacter;

    void Start()
    {
        gr = GetComponentInParent<GraphicRaycaster>();
    }

    void Update()
    {
        pointerEventData.position = Camera.main.WorldToScreenPoint(transform.position);
        List<RaycastResult> results = new List<RaycastResult>();
        gr.Raycast(pointerEventData, results);

        if (results.Count > 0) {
            Transform raycaterCharacter = results[0].gameObject.transform;
            if (raycaterCharacter != currentCharacter) {

                SetCurrentCharacter(raycaterCharacter);
            }
        }
    }

    void SetCurrentCharacter(Transform t) {
        currentCharacter = t;
        TextMeshProUGUI name = currentCharacter.Find("Name Tag").GetComponentInChildren<TextMeshProUGUI>();

        print("currentCharacter : " + name.text);
    }
}
