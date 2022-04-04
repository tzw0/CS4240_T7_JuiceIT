using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

enum HandStatus
{
    Free,
    Pulling,
    Holding,
}

public class HandInteraction_v2 : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject spawnPoint;
    public float pullVelocity;
    public SteamVR_Input_Sources hand;
    public SteamVR_Action_Vibration hapticAction;
    public SteamVR_Action_Boolean xxxarr;
    public HandAnimator animator;
    public HandEffects effects;
    public List<string> tags;
    private GameObject hitFruit;
    private Rigidbody hitRigidBody;
    private HashSet<string> tagSet = new HashSet<string>();

    private HandStatus status = HandStatus.Free;
    private bool handIsColliding;
    public AudioClip grabCorrectAudio;
    public AudioClip grabWrongAudio;
    public AudioClip grabPullAudio;
    public AudioClip grabRepositionAudio;

    // Start is called before the first frame update
    void Start()
    {
        foreach(string tag in tags) {
            tagSet.Add(tag);
        }
    }

    // Update is called once per frame
    void Update()
    {   
        handIsColliding = false;

        if (status == HandStatus.Free) {
            UpdateRecticle();
        } else {
            if (hitFruit != null) {
                hapticAction.Execute(0, 1, 150, 1, hand);
            } else {
                status = HandStatus.Free;
                effects.Deactivate();
            }
        }

        if (SteamVR_Input.GetStateDown("GrabPinch", hand)) {
            if (status == HandStatus.Free && hitFruit != null) {
                AudioSource.PlayClipAtPoint(grabPullAudio, transform.position, 1f);
                PullHitObject();
                effects.Activate(0, hitFruit);
            }
            animator.SetTrigger(true);
        }

        if (SteamVR_Input.GetStateUp("GrabPinch", hand)) {
            if (status == HandStatus.Pulling) {
                StopPullingHitObject();
                effects.Deactivate();
            }
            animator.SetTrigger(false);
        }

        if (SteamVR_Input.GetStateDown("GrabGrip", hand)) {
            if (status == HandStatus.Free && hitFruit != null) {
                status = HandStatus.Holding;
                effects.Activate(1, hitFruit);

                hitFruit.AddComponent<HeldFruit>().SetHeld(this);
                AudioSource.PlayClipAtPoint(grabRepositionAudio, transform.position, 1f);
            }
            animator.SetGrip(true);
        }

        if (SteamVR_Input.GetStateUp("GrabGrip", hand)) {
            if (status == HandStatus.Holding) {
                status = HandStatus.Free;
                effects.Deactivate();
                if (hitFruit != null) {
                    float spawnSpeed = spawnPoint.GetComponent<SpawnFruits>().speed;
                    hitFruit.GetComponent<HeldFruit>().SetUnHeld((Vector3.zero - spawnPoint.transform.position) * spawnSpeed);
                }
            }
            animator.SetGrip(false);
        }

    }

    private void UpdateRecticle() {
        // Ray from the controller
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
 
        // if its a hit
        if (Physics.Raycast(ray, out hit) && CheckIsFruit(hit.transform.gameObject.tag) && !hit.transform.gameObject.GetComponent<Rigidbody>().useGravity) { 
            hitFruit = hit.transform.gameObject;
            hitRigidBody = hitFruit.GetComponent<Rigidbody>();
        } else {
            hitFruit = null;
            hitRigidBody = null;
        }
    }

    void StopPullingHitObject(){
        hitRigidBody.isKinematic = true;
        hitRigidBody.isKinematic = false;
        float spawnSpeed = spawnPoint.GetComponent<SpawnFruits>().speed;
        hitRigidBody.AddForce((Vector3.zero - spawnPoint.transform.position) * spawnSpeed);
        status = HandStatus.Free;
    }

    void PullHitObject() {
        status = HandStatus.Pulling;
        hitRigidBody.AddForce((gameObject.transform.position - hitFruit.transform.position).normalized * pullVelocity * hitRigidBody.mass / 100);
        // if (!AudioSource.isPlaying)
        // {
            AudioSource.PlayClipAtPoint(grabPullAudio, transform.position, 1f);
        // }
    }

    public void HandOnCollision(GameObject fruit) {
        if (handIsColliding) {
            return;
        }
        handIsColliding = true;
        bool isCorrect = GameManager.Instance.UpdateGrabFruitForRecipe(fruit);
        if (isCorrect) {
            AudioSource.PlayClipAtPoint(grabCorrectAudio, transform.position, 1f);
        } else {
            AudioSource.PlayClipAtPoint(grabWrongAudio, transform.position, 1f);
        }
        status = HandStatus.Free;
        effects.Deactivate();
    }

    public bool CheckIsFruit(string tag) {
        return tagSet.Contains(tag);
    }

    public GameObject GetHitFruit() {
        return hitFruit;
    }
}
