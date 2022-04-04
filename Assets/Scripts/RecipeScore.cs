using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeScore : MonoBehaviour
{
    private Color originalColor;
    public TMP_Text score;
    public Image recipeFail;
    // Start is called before the first frame update
    void Start()
    {
        originalColor = score.color; 
        Debug.Log("original score color: " + originalColor);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Set(int reward) {
        // score.CrossFadeColor(originalColor, 0, true, false);
        recipeFail.enabled = false;
        score.text = "+" + reward;
        score.fontSize = 100;
        if (reward == 0) {
            recipeFail.enabled = true;
            score.text = "";

            // score.CrossFadeColor(GameManager.Instance.WrongTextColor, 0, true, false);
            // score.color = new Color(0.25f, 0.f, 0.15f, 1);
            // score.text = "Too Slow!";
            // score.fontSize = 70;
        }
    }
}
