using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class KingdomButton : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Text text;
    public Image rect;
    public Image circle;

    public Color textColorWhenSelected;
    public Color rectColorWhenMoverOvered;

    // Start is called before the first frame update
    void Start() {
        rect.color = Color.clear;
        text.color = Color.white;
        circle.color = Color.white;
    }

    public void OnDeselect(BaseEventData eventData) {
        rect.DOColor(Color.clear, 0.1f);
        text.DOColor(Color.white, 0.1f);
        circle.DOColor(Color.white, 0.1f);
    }

    public void OnSelect(BaseEventData eventData) {
        rect.DOColor(Color.white, 0.1f);
        text.DOColor(textColorWhenSelected, 0.1f);
        circle.DOColor(Color.red, 0.1f);

        rect.transform.DOComplete();
        rect.transform.DOPunchScale(Vector3.one * 0.3f, 0.2f, 20, 1);
    }

    public void OnSubmit(BaseEventData eventData) {
        
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (EventSystem.current.currentSelectedGameObject != gameObject) {
            rect.DOColor(rectColorWhenMoverOvered, 0.2f);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (EventSystem.current.currentSelectedGameObject != gameObject) {
            rect.DOColor(Color.clear, 0.2f);
        }
    }
}
