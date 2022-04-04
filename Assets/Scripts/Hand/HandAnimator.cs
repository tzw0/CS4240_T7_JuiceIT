using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HandAnimator : MonoBehaviour
{
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetGrip(bool b) {
        animator.SetBool("Grip", b);
    }                                                    

    public void SetTrigger(bool b) {
        animator.SetBool("Trigger", b);
    }

}
