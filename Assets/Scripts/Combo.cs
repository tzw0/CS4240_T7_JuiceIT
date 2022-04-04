using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Combo : MonoBehaviour
{
    private TMP_Text combo;

    // TODO: replace this with comboCount from game manager
    // In slash and grab function where we check correct slashes:
    // comboCount++;
    // int addPoints = getComboPoints(comboCount);
    // updateComboUI(comboCount);
    // if (addPoints > 0) {
    //    score += addPoints;
    //    // show the Combo ui on hand here
    // }

    void Start() {
        combo = GetComponent<TMP_Text>();
    }

    public void updateComboUI(int comboCount) {
        // TODO: replace with combo from game manager
        combo.text = comboCount.ToString();
    }

    // Call this in game manager after adding the combo count
    public int getComboPoints(int comboCount) {
        if (comboCount % 5 == 0) {
            return comboCount * 2;
        }
        return 0;
    }
}
