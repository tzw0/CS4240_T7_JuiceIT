using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpawnFruits : MonoBehaviour
{
    public float speed;
    private float delay;
    public int fruitLimit;
    public float maxYHeight;
    public float minYHeight;
    public float xRadius;
    private float timeFuitExists;

    private Transform spawnPoint;
    private bool startNextRecipe;
    private int completedRecipeNumber;

    private List<Recipe> allRecipeList;
    private int correctFruitPercentage;
    public List<GameObject> allFruits;

    public AudioClip ticketChangeSound;
    public AudioClip wrongRecipeSound;
    public AudioClip nextLevelSound;
    private List<string> allFruitTags;
    public GameObject difficultyPrefab;


    private Dictionary<int, bool> recipeCompletionStatus = new Dictionary<int, bool>(); //flag (0 or 1), recipe number
    // public bool repeat; // set this to true if you want the recipes to loop

    public bool isAFruit(GameObject fruit) {
        // Debug.Log("fruit tag: " + fruit.tag);
        foreach (string tag in allFruitTags) {
            if (fruit.tag == tag) {
                // Debug.Log("   -> is a fruit");
                return true;
            }
        }

        // Debug.Log("   -> not a fruit");
        return false;
    }

    void Start()
    {
        spawnPoint = gameObject.transform;
        // StartCoroutine(SpawnFruit());
        var allRecipes = Resources.LoadAll("recipes", typeof(Recipe));

        allRecipeList = new List<Recipe>();
        foreach(var recipe in allRecipes) {
            allRecipeList.Add(recipe as Recipe);
        }

        allFruitTags = new List<string>();
        foreach(GameObject fruit in allFruits) {
            allFruitTags.Add(fruit.tag);
            // Debug.Log("fruit tag: " + fruit.tag);
        }
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int rand = Random.Range(i, list.Count);
            list[i] = list[rand];
            list[rand] = temp;
        }
    }

    public void StartNextRecipe(int completedRecipeNumber_)
    {
        if (!startNextRecipe)
        {
            startNextRecipe = true;
        }
        completedRecipeNumber = completedRecipeNumber_;
    }

    public IEnumerator SpawnFruit()
    {
        // List<Recipe> repeatList = new List<Recipe>();
        // foreach (Recipe recipe in recipeList)
        // {
        //     repeatList.Add(recipe);
        // }

        int recipeCounter = 0;
        int duplicates = 1;
        bool orderSuccess = true;
        float recipeInterval = 0;
        recipeCompletionStatus.Clear();

        Debug.Log("SpawmFruit Called. is game over?: " + GameManager.Instance.isGameOver().ToString());

        while (!GameManager.Instance.isGameOver())
        {
            recipeCounter++;
            if (recipeCompletionStatus.ContainsKey(recipeCounter)) {
                recipeCompletionStatus[recipeCounter] = false;
            } else {
                recipeCompletionStatus.Add(recipeCounter, false);
            }
            
            if ((recipeCounter % GameManager.Instance.recipePerLevel == 0 ||
                recipeCounter == 1)) {
                Level level = GameManager.Instance.GetLevel();
                GameManager.Instance.NextLevel();
                GameObject difficultyObj = Instantiate(difficultyPrefab, 
                    new Vector3(0, 1f, 7), Quaternion.identity);
                Rigidbody rb = difficultyObj.GetComponent<Rigidbody>();
                rb.AddForce(new Vector3(0, 1, 0) * 3.5f);
                difficultyObj.transform.GetChild(0).GetComponent<TMP_Text>().text = GameManager.Instance.getDifficulty();
                Destroy(difficultyObj, 2f);
                GetComponent<AudioSource>().clip = nextLevelSound;
                GetComponent<AudioSource>().Play();
                speed = level.speed;
                correctFruitPercentage = level.correctFruitPercentage;
                delay = level.delay;
                duplicates = level.getDuplicates();
                timeFuitExists = level.getFruitTravelTime();
                recipeInterval = level.recipeInterval;
            }

            int recipeIndex = Random.Range(0, allRecipeList.Count);
            Recipe recipe = allRecipeList[recipeIndex];
            Debug.Log("Current recipe: " + recipe.name);
            
            StartCoroutine(DelaySetCurrentRecipe(recipe, timeFuitExists, recipeCounter));
            //recipeInterval is also the time player has from order received to first fruit.
            yield return new WaitForSeconds(recipeInterval);
            yield return new WaitUntil(() => (!Instructions.Instance.isRunning()));
            List<Fruit> fruits = recipe.fruits;
            float recipeLength = fruits.Count;
            // Debug.Log("init correct fruits");
            // initialise correct fruits to spawn
            List<GameObject> fruitsToSpawn = new List<GameObject>();
            foreach (Fruit fruit in fruits)
            {
                for (int i = 0; i < duplicates; i++) {
                    fruitsToSpawn.Add(fruit.fruitPrefab);
                }
            }
            // Debug.Log("init wrong fruits");
            // initialise wrong fruits to spawn
            int numberOfWrongFruits = Mathf.RoundToInt(recipeLength / correctFruitPercentage * (100 - correctFruitPercentage));
            int allFruitsLength = allFruits.Count;
            for (int i = 0; i < numberOfWrongFruits; i++)
            {
                int fruitIndex = Random.Range(0, allFruitsLength);
                fruitsToSpawn.Add(allFruits[fruitIndex]);
            }
            // shuffle fruits 
            ShuffleList(fruitsToSpawn);

            bool evalCompleted = false;
            // spawning starts here
            List<GameObject> fruitsToDestroy = new List<GameObject>();
            foreach (GameObject fruit in fruitsToSpawn)
            {
                //only terminate the loop if the recipe completed is the current recipe
                //(if it is the previous recipe that is completed during the loop, we should not break)
                if (completedRecipeNumber == recipeCounter)
                {
                    // GetComponent<AudioSource>().clip = ticketChangeSound;
                    // GetComponent<AudioSource>().Play();
                    // evalCompleted = true;
                    // Debug.Log("break during spawn: " + recipeCounter + " " + recipeCompletionStatus.ContainsKey(recipeCounter));
                    // recipeCompletionStatus[recipeCounter] = true;
                    break;
                }

                if (GameManager.Instance.isGameOver()) {
                    evalCompleted = true;
                    break;
                }

                float yPos = Random.Range(minYHeight, maxYHeight);
                float xPos = spawnPoint.position.x + Random.Range(-xRadius, xRadius);

                GameObject spawnFruit = Instantiate(fruit, new Vector3(xPos, yPos, spawnPoint.position.z), Quaternion.identity);
                Rigidbody rb = spawnFruit.GetComponent<Rigidbody>();
                rb.AddForce((Vector3.zero - spawnPoint.position) * speed);
                // Destroy(spawnFruit, timeFuitExists + 3);
                fruitsToDestroy.Add(spawnFruit);
                yield return new WaitForSeconds(delay);
            }

            // Debug.Log("starting coroutines: " + recipeCounter);
            //Evaluate once the last fruit from fruitsToSpawn reach user.
            StartCoroutine(CheckRecipeCorrectness(recipeCounter, fruitsToDestroy));
            StartCoroutine(CheckRecipeWrongness(timeFuitExists, recipeCounter));

            // if (!evalCompleted) {
            //     Debug.Log("starting coroutines: " + recipeCounter);
            //     //Evaluate once the last fruit from fruitsToSpawn reach user.
            //     StartCoroutine(CheckRecipeCorrectness(recipeCounter));
            //     StartCoroutine(CheckRecipeWrongness(timeFuitExists, recipeCounter));
            // }

            // loop baby
            // if (repeat && recipeList.Count == 0)
            // {
            //     foreach (Recipe repeat in repeatList)
            //     {
            //         recipeList.Add(repeat);
            //     }
            // }


            // need to account for amount of time fruits travel. 
            // Only print order when fruit is almost approaching.
        }
    }

    private IEnumerator CheckRecipeCorrectness(int thenRecipeCounter, List<GameObject> fruitsToDestroy) {
        // Debug.Log("Check recipe correctness: " + thenRecipeCounter + ", " + recipeCompletionStatus.ContainsKey(thenRecipeCounter));
        yield return new WaitUntil(() => ((recipeCompletionStatus.ContainsKey(thenRecipeCounter) && 
            recipeCompletionStatus[thenRecipeCounter]) || completedRecipeNumber == thenRecipeCounter ||
        GameManager.Instance.isGameOver()));
        if (!GameManager.Instance.isGameOver() && completedRecipeNumber == thenRecipeCounter) {
            GetComponent<AudioSource>().clip = ticketChangeSound;
            GetComponent<AudioSource>().Play();
            recipeCompletionStatus[thenRecipeCounter] = true;
        }
        foreach(GameObject fruit in fruitsToDestroy) {
            Destroy(fruit);
        }
        // Debug.Log("check recipe correctness completed: " + thenRecipeCounter);
    }
    private IEnumerator CheckRecipeWrongness(float thenTimeFuitExists, int thenRecipeCounter) {
        yield return new WaitForSeconds(thenTimeFuitExists - 1.1f);
        if (recipeCompletionStatus.ContainsKey(thenRecipeCounter)) {
            recipeCompletionStatus[thenRecipeCounter] = true;
            if (!GameManager.Instance.isGameOver() &&
                completedRecipeNumber != thenRecipeCounter) {
                GetComponent<AudioSource>().clip = wrongRecipeSound;
                GetComponent<AudioSource>().Play();
                GameManager.Instance.WrongOrder();
            }
        } 
        // Debug.Log("check recipe wrongness completed: " + thenRecipeCounter);
    }

    private IEnumerator DelaySetCurrentRecipe(Recipe recipe, float thenTimeFuitExists, int thenRecipeCounter) {
        // yield return new WaitForSeconds(thenTimeFuitExists + 2);
        // Debug.Log("Delay set recipe: " + thenRecipeCounter);
        yield return new WaitUntil(() => (thenRecipeCounter == 1 || recipeCompletionStatus[thenRecipeCounter-1] ||
        GameManager.Instance.isGameOver()));
        if (!GameManager.Instance.isGameOver()) {
            if (thenRecipeCounter != 1 && completedRecipeNumber != thenRecipeCounter-1) {
                recipeCompletionStatus.Remove(thenRecipeCounter-1);
            }
            Debug.Log("Set current recipe: " + thenRecipeCounter);
            if (!GameManager.Instance.isGameOver()) {
                GameManager.Instance.SetCurrentRecipe(recipe, thenRecipeCounter, completedRecipeNumber >= thenRecipeCounter-1);
            }
            Debug.Log("Finish set current recipe" + thenRecipeCounter);
        }
    }
}
