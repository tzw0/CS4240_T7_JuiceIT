using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HeldFruit : MonoBehaviour
{
    public HandInteraction_v2 hand;
    public float distance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GameObject handHitFruit = hand.GetHitFruit();
        if (handHitFruit == null || handHitFruit != gameObject) {
            Destroy(this);
        }
        gameObject.transform.position = Vector3.MoveTowards (hand.transform.position, hand.transform.position + hand.transform.forward * distance, Time.deltaTime * 100000);
    }

    void OnCollisionEnter(Collision collision) {
        collision.gameObject.GetComponent<Rigidbody>().useGravity = true;
    }

    public void SetHeld(HandInteraction_v2 hand) {
        this.hand = hand;
        this.distance = Vector3.Distance(hand.transform.position, transform.position);
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void SetUnHeld(Vector3 force) {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddForce(force);
        Destroy(this);
    }
}
