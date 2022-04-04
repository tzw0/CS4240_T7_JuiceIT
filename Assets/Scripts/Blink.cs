using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(blink());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator blink()
    {
        yield return new WaitForSeconds(0.3f);
        if (GetComponent<Light>().color != Color.clear) {
            GetComponent<Light>().color = Color.clear;
        } else {
            GetComponent<Light>().color = Color.white;
        }

        StartCoroutine(blink());
    }
}
