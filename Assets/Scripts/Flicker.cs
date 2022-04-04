using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flicker : MonoBehaviour
{
    bool stop = false;
    // Start is called before the first frame update
    // void Start()
    // {
    //     StartCoroutine(flicker2());
    // }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable() {
        StartCoroutine(flicker2());
        // stop = false;
    }

    void OnDisable() {
        stop = true;
    }

    IEnumerator flicker2()
    {
        yield return new WaitForSeconds(0.3f);
        if (GetComponent<Image>().color != Color.clear) { //new Color(1,1,1,1)) {
            GetComponent<Image>().color = Color.clear;//new Color(1,1,1,1);
        } else {
            GetComponent<Image>().color = new Color(239/255f,126/255f,155/255f,1);
        }

        // if (!stop)
            StartCoroutine(flicker2());
    }
}
