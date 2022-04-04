using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Recipe", menuName = "Recipe/Recipe")]
public class Recipe : ScriptableObject
{
    public List<Fruit> fruits;
    public Sprite icon;
    public int reward;
}
