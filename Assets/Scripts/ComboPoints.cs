using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ComboPoints : MonoBehaviour
{
    public TMP_Text comboText;
    public TMP_Text scoreText;
    public TMP_Text wrongIngredientText;
    private Color originalColor;
    // Start is called before the first frame update
    void Start()
    {
        originalColor = comboText.color;   
        Debug.Log("original combo color: " + originalColor);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Set(int combo) {
        // comboText.CrossFadeColor(originalColor, 0, true, false);
        comboText.fontSize = 36;
        scoreText.fontSize = 100;
        comboText.text = "COMBO x" + combo;
        scoreText.text = "+" + (combo * 2);
        wrongIngredientText.text="";


        if (combo == 0) {
            // comboText.CrossFadeColor(GameManager.Instance.WrongTextColor, 0, true, false);
            wrongIngredientText.text="Wrong Ingredient";
            comboText.text="";

            if (GameManager.Instance.wrongIngredientPenalty == 0 || GameManager.Instance.score <= 0) {
                scoreText.text = "";
            } else {
                scoreText.text = "-" + GameManager.Instance.wrongIngredientPenalty;
            }
        }
    }
}
