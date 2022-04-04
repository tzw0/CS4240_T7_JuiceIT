using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandColliderDestroy : MonoBehaviour
{
    public HandInteraction_v2 handInteraction;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other) {
        if (handInteraction.CheckIsFruit(other.gameObject.tag)) {
            handInteraction.HandOnCollision(other.gameObject);
            Destroy(other.gameObject);
        }
    }
}
