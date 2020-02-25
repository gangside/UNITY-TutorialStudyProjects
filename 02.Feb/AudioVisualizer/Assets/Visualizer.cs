using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Visualizer : MonoBehaviour
{
    public float minHeight = 15.0f, maxHeight = 425.0f;
    public float sensitivity = 10.0f;

    public Color visualizerColor = Color.gray;
    [Space(15)]
    public AudioClip audioClip;
    public bool isLoop = true;
    [Space(15), Range(64,8192)]
    public int visualizerSimples = 64;

    VisualizerObject[] visualizerObjects;
    AudioSource audioSource;

    void Start()
    {
        visualizerObjects = GetComponentsInChildren<VisualizerObject>();

        if (!audioClip) return;

        audioSource = new GameObject("AudioSource").AddComponent<AudioSource>();
        audioSource.loop = isLoop;
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    void Update()
    {
        float[] spectrumData = audioSource.GetSpectrumData(visualizerSimples, 0, FFTWindow.Rectangular);

        for (int i = 0; i < visualizerObjects.Length; i++) {
            Vector2 newSize = visualizerObjects[i].GetComponent<RectTransform>().rect.size;
            newSize.y = Mathf.Lerp(newSize.y, minHeight + (spectrumData[i] * (maxHeight - minHeight) * 5.0f), sensitivity);
            newSize.y = Mathf.Clamp(newSize.y, minHeight, maxHeight);
            visualizerObjects[i].GetComponent<RectTransform>().sizeDelta = newSize;

            visualizerObjects[i].GetComponent<Image>().color = visualizerColor;
        }
    }
}
