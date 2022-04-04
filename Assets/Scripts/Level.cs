using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level
{
    // [1,2,3,4,5]
    // [0.1, 0.]
    // index = rand(0, length of array-1)
    // lvl-1 100% 3x [3]
    // lvl-2 50% 3x, 50% 2x [2,3]
    // lvl-3 10% 3x, 70% 2x 20% 1x [2,2,2,2,2,2,2,3,1,1]
    public int[] duplicateArray;
    public int speed;
    public int correctFruitPercentage;
    public float recipeInterval;
    public float delay; //in seconds
    public int getDuplicates() {
        int rand = Random.Range(0, duplicateArray.Length);
        return duplicateArray[rand];
    }

    private float distanceFromSpawn = 8000f;

    public Level(int[] duplicateArray_, int speed_, int correctFruitPercentage_, 
    float delay_, float recipeInterval_) {
        duplicateArray = duplicateArray_;
        speed = speed_;
        correctFruitPercentage = correctFruitPercentage_;
        delay = delay_;
        recipeInterval = recipeInterval_;
    }

    public float getFruitTravelTime() {
        return distanceFromSpawn / speed + 1;
    }
}
