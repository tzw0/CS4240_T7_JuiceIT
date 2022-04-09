using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Instructions : MonoBehaviour
{
    private static Instructions _instance;
    public List<GameObject> instructionList;
    private int currentIndex = 0;
    private bool hideInstructions = true;
    private List<int> lockedIndexes = new List<int>{3, 11};
    public static Instructions Instance
    {
        get
        {
            return _instance;
        }
    }

   private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        foreach(GameObject i in instructionList) {
            i.SetActive(false);
        }
        currentIndex = 0;
        instructionList[currentIndex].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!hideInstructions &&
                (SteamVR_Input.GetStateDown("GrabGrip", SteamVR_Input_Sources.LeftHand) ||
                SteamVR_Input.GetStateDown("GrabGrip", SteamVR_Input_Sources.RightHand)) &&
                !lockedIndexes.Contains(currentIndex) ) {
                Debug.Log("Instructions next");
                Instructions.Instance.Next();
            }
    }

    public void StartInstructions() {
        if (hideInstructions) {
            hideInstructions = false;
        }
    }

    public void JumpTo(int i) {
        if (hideInstructions) {
            return;
        }
        instructionList[currentIndex].SetActive(false);
        currentIndex = i;
        instructionList[currentIndex].SetActive(true);
    }

    public void Next() {
        if (hideInstructions) {
            return;
        }
        if (currentIndex >= instructionList.Count - 1) {
            instructionList[currentIndex].SetActive(false);
            hideInstructions = true;
            EyeTrack eyeTrack = FindObjectsOfType(typeof(EyeTrack))[0] as EyeTrack;
            eyeTrack.Reset();
            return;
        }
        if (currentIndex >= 0) {
            instructionList[currentIndex].SetActive(false);
        }
        currentIndex++;
        instructionList[currentIndex].SetActive(true);
    }

    public bool isRunning() {
        return !hideInstructions;
    }

    public bool isAtIndex(int i) {
        // if is not running, it will return true
        return currentIndex == i;
    }
}
