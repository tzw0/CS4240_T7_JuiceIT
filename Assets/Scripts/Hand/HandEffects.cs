using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandEffects : MonoBehaviour
{
    public GameObject laser;
    public List<GameObject> beamObjects;
    public List<GameObject> beamFrom;
    public List<GameObject> auraObjects;

    private List<LineRenderer> beams = new List<LineRenderer>();
    private GameObject to;

    private int beamIdx = -1; // -1 means not on

    // Start is called before the first frame update
    void Start()
    {   
        
    }

    void Awake() {
        for (int i = 0; i < beamObjects.Count; i++) {
            beams.Add(beamObjects[i].GetComponent<LineRenderer>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (beamIdx > -1) {
            SetBeam();
        }
    }

    public void Activate(int beamIdx, GameObject to) {
        this.to = to;
        this.beamIdx = beamIdx;
        laser.SetActive(false);
        beams[beamIdx].enabled = true;
        auraObjects[beamIdx].SetActive(true);
    }

    public void Deactivate() {
        if (beamIdx > -1) {
            beams[beamIdx].enabled = false;
            auraObjects[beamIdx].SetActive(false);
            beamIdx = -1;
            laser.SetActive(true);
        }
    }

    void SetBeam() {
        if (beams[beamIdx] != null) {
            beams[beamIdx].SetPosition(0, beamFrom[beamIdx].transform.position);
            beams[beamIdx].SetPosition(1, to.transform.position);
        }
    }
}
