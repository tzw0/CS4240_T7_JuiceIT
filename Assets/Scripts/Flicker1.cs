using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flicker1 : MonoBehaviour
{
    bool stop = false;
    // Start is called before the first frame update
    // void Start()
    // {
    //     StartCoroutine(flicker());
    // }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable() {
        StartCoroutine(flicker());
        // stop = false;
    }

    void OnDisable() {
        stop = true;
    }

    IEnumerator flicker()
    {
        yield return new WaitForSeconds(0.3f);
        if (GetComponent<Text>().color != new Color(239/255f,126/255f,155/255f,1)) {
            GetComponent<Text>().color = new Color(239/255f,126/255f,155/255f,1);
        } else {
            GetComponent<Text>().color = new Color(1,1,1,1);
        }

        // if (!stop)
            StartCoroutine(flicker());
    }
}
