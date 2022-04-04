using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCard : MonoBehaviour
{
    public string fruitName;

    public Sprite transparentSprite;
    public Image fruitSprite;
    public GameObject progressBar;

    public GameObject bar;

    public List<GameObject> segments;
    public int total;
    private int current;

    void Start()
    {

    }

    void Update()
    {
    }

    public void Reset()
    {
        for (int i = segments.Count - 1; i >= 0; i--)
        {
            Destroy(segments[i]);
        } 
        fruitName = "";
        segments = new List<GameObject>();
        current = 0;
        total = 0;
        fruitSprite.sprite = transparentSprite;
    }
    public void Set(string fname, Sprite fimage, bool isHalf, int totalCount) 
    {
        fruitName = fname;
        current = 0;
        total = totalCount;
        segments = new List<GameObject>();
        // Debug.Log(fruitName + " " + total);
        fruitSprite.sprite = fimage;
        int offset = (total - 1) * 5;
        float width = progressBar.GetComponent<RectTransform>().sizeDelta.x - offset;
        for (int i = 0; i < total; i++)
        {
            GameObject segment = Instantiate(progressBar, bar.transform);
            segment.GetComponent<RectTransform>().sizeDelta = 
                new Vector2 (width/total,
                progressBar.GetComponent<RectTransform>().sizeDelta.y);
            segments.Add(segment);
        }
    }

    public void Add() {
        // Debug.Log("ADD: " + current + "/" + total);
        if (current < total)
        {
            current += 1;
            segments[current-1].GetComponent<Image>().color = new Color32(0, 255, 0, 180);
            // Debug.Log("Add slices: " + current);
        }
        else
        {
            // failed order
        }
    }
}
