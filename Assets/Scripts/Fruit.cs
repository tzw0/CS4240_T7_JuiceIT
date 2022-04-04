using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Fruit", menuName = "Recipe/Fruit")]
public class Fruit : ScriptableObject
{
    public GameObject fruitPrefab;
    public bool isHalf;
    public Sprite icon;
    public AudioSource source;
    public AudioClip audio;
}
