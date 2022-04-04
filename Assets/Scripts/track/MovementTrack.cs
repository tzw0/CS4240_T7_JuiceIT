using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MovementTrack : MonoBehaviour
{
    public float threshold;
    private float upperBoundKcalPerUpdate = 0.5f; 
    private float maxKcalPerUpdate = 0.2f;
    private float avgKcalPerUpdate = 0.1f;
    private Vector3 previousPosition;
    private double lateralMovement;
    private double horizontalMovement;
    private double kcal;

    // Start is called before the first frame update
    void Start()
    {
        lateralMovement = 0;
        horizontalMovement = 0;
        kcal = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (previousPosition == null) {
            previousPosition = transform.position;
            return;
        }
        Vector3 newPos = transform.position;

        float newD = Vector3.Distance(newPos, previousPosition);
        if (newD > threshold) {
            double increaseLateral = Vector3.Distance(new Vector3(newPos.x, 0, newPos.z), new Vector3(previousPosition.x, 0, previousPosition.z));
            lateralMovement += increaseLateral * (increaseLateral/Time.deltaTime/Time.deltaTime);

            double horizontalLateral = Vector3.Distance(new Vector3(0, newPos.y, 0), new Vector3(0, previousPosition.y, 0));

            if (newPos.y - previousPosition.y > 0) {
                horizontalMovement += Math.Abs(horizontalLateral * (horizontalLateral/Time.deltaTime/Time.deltaTime - 9.81));
            } else {
                horizontalMovement += horizontalLateral * (horizontalLateral/Time.deltaTime/Time.deltaTime + 9.81);
            }

            // double updateKcal = (lateralMovement + horizontalMovement) * GameManager.Instance.mass * 5.335 / 100 * 0.239006 / 1000;
            // double updateKcal = (lateralMovement + horizontalMovement) * GameManager.Instance.mass * 2.295 / 100 * 0.239006 / 1000;
            double updateKcal = (lateralMovement + horizontalMovement) * GameManager.Instance.mass * 0.575 / 100 * 0.239006 / 1000;
            if (updateKcal < upperBoundKcalPerUpdate) {
                kcal = updateKcal > maxKcalPerUpdate ? avgKcalPerUpdate : updateKcal;
            }

            // Debug.Log("kcal in update loop: " + updateKcal);

            // Debug.Log((lateralMovement + horizontalMovement) * 63 * 5.335 / 100 * 0.239006 / 1000);
        }

        previousPosition = newPos;
    }

    public double GetLateralMovement() {
        return lateralMovement;
    }

    public double GetHorizontalMovement() {
        return horizontalMovement;
    }

    public double GetKcal() {  
        return kcal;
    }

    public void Reset() {
        lateralMovement = 0;
        horizontalMovement = 0;
        kcal = 0;
        // previousPosition = Vector3.zero;
    }
}
