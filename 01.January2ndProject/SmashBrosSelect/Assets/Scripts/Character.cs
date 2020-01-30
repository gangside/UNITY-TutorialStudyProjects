using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "new Character", menuName = "Character")]
public class Character : ScriptableObject
{
    public new string name;
    public Sprite sprite;
    public float artworkScale = 1;
}
