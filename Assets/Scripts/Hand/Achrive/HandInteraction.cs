using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;


public class HandInteraction : MonoBehaviour
{   
    public float pullVelocity;
    public SteamVR_Input_Sources hand;
    public SteamVR_Behaviour_Pose controllerPose;


    private GameObject hitFruit;
    private Rigidbody hitRigidBody;
    private bool isPulling;
    private bool isPinned;
    private float hitObjectDistance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void UpdateRecticle() {
        // Ray from the controller
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
 
        // if its a hit
        if (Physics.Raycast(ray, out hit) && hit.transform.gameObject.CompareTag("Fruit")) {
            if (hitFruit != null && isPulling && hitFruit != hit.transform.gameObject) {
                StopPullingHitObject();
            } else if (hitFruit != hit.transform.gameObject) {
                isPinned = false;
            }
            hitFruit = hit.transform.gameObject;
            hitRigidBody = hitFruit.GetComponent<Rigidbody>();
            hitObjectDistance = Vector3.Distance(transform.position, hitFruit.transform.position);
        } else {
            // If not a hit
            if (hitFruit != null && isPulling) {
                StopPullingHitObject();
            }
            hitFruit = null;
            hitRigidBody = null;
            isPinned = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRecticle();

        if (hitFruit != null && (SteamVR_Input.GetStateDown("GrabPinch", hand))) {
            isPinned = false;
            PullHitObject();
        }


        if (isPulling && SteamVR_Input.GetStateUp("GrabPinch", hand)) {
            StopPullingHitObject();
        }

        if (!isPulling && hitFruit != null && SteamVR_Input.GetStateDown("GrabGrip", hand)) {
            //hitFruit.GetComponent<Rigidbody>().velocity = controllerPose.GetVelocity() * 100;
            // Destroy(hitFruit);
            // hitFruit = null;
            // hitRigidBody = null;
            isPinned = true;
        }

        if (isPinned && SteamVR_Input.GetStateUp("GrabGrip", hand)) {
            //hitFruit.GetComponent<Rigidbody>().velocity = controllerPose.GetVelocity() * 100;
            // Destroy(hitFruit);
            // hitFruit = null;
            // hitRigidBody = null;
            isPinned = false;
        }

        if (isPinned) {
            hitFruit.transform.position = Vector3.MoveTowards (transform.position, transform.position + transform.forward * hitObjectDistance, Time.deltaTime * 100000);
        }
    }


    void StopPullingHitObject(){
        hitRigidBody.AddForce(-(gameObject.transform.position - hitFruit.transform.position).normalized * pullVelocity * hitRigidBody.mass / 100);
        isPulling = false;
    }

    void PullHitObject() {
        isPulling = true;
        hitRigidBody.AddForce((gameObject.transform.position - hitFruit.transform.position).normalized * pullVelocity * hitRigidBody.mass / 100);
    }
}
