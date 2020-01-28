using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class CircleAnimation : MonoBehaviour
{
    RectTransform rect;

    Image img;
    Vector2 origSize;

    public float duration;
    public float delay;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        img = rect.GetComponent<Image>();
        img.DOFade(0, 0);

        origSize = rect.sizeDelta;
        rect.sizeDelta = origSize / 4f;

        StartCoroutine(Delay());
    }

    IEnumerator Delay() {
        yield return new WaitForSeconds(delay);
        Animate();
    }

    private void Animate() {
        Sequence s = DOTween.Sequence();
        s.Append(rect.DOSizeDelta(origSize, duration).SetEase(Ease.OutCirc));
        s.Join(img.DOFade(1, duration / 3));
        s.Join(img.DOFade(0, duration / 4).SetDelay(duration / 1.5f));
        s.SetLoops(-1);
    }
}
