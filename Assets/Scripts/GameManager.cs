using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Valve.VR;

public class GameManager : MonoBehaviour
{
    // public Recipe dummyRecipe;
    private ArrayList levelSettings;
    private int currentRecipeNumber = 0;
    private int currentLevel = -1;
    public int recipePerLevel;
    public int maxWrongOrder;
    private int wrongOrders = 0;
    private static GameManager _instance;
    private Recipe currentRecipe;
    public List<RecipeItem> currentRecipeList = new List<RecipeItem>();

    public SortedDictionary<string, int> currentRecipeProgress = new SortedDictionary<string, int>();
    public Dictionary<string, Sprite> currentRecipeImages = new Dictionary<string, Sprite>();
    public List<ItemCard> itemList;

    [SerializeField]
    private ItemCard emptyItem;

    public List<string> list;
    public SpawnFruits spawnFruit;
    public OrderMachine orderMachine;
    private List<TMP_Text> scoreTexts;
    private List<TMP_Text> calorieTexts;
    public double kcal = 0;

    public int score;
    public int wrongIngredientPenalty;
    public int mass;
    public int height;
    public int combo;
    public GameObject comboPrefab;
    public TMP_Text comboText;
    public AudioClip comboSound;
    public TMP_Text levelText;
    public AudioClip nextLevelSound;
    public AudioClip gameOverSound;

    private MovementTrack[] hands;

    private bool gameStarted = false;
    private bool atLastLevel = false;

    public static GameManager Instance
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

    public async void SetCurrentRecipe(Recipe recipe, int number, bool previousRecipeCompleted)
    {
        currentRecipeNumber = number;
        currentRecipe = recipe;
        currentRecipeList.Clear();
        currentRecipeProgress = new SortedDictionary<string, int>();
        currentRecipeImages = new Dictionary<string, Sprite>();
        for (int i = 0; i < itemList.Count; i++)
        {
            itemList[i].Reset();
        }

        orderMachine.PrintOrder(currentRecipe, previousRecipeCompleted);

        foreach (Fruit fruit in currentRecipe.fruits)
        {
            RecipeItem item = new RecipeItem(fruit.fruitPrefab, fruit.isHalf);
            item.fruit.name = item.fruit.name.Replace("(Clone)", "").Trim();
            currentRecipeList.Add(item);
            string keyName = item.fruit.name + (fruit.isHalf ? "half" : "");
            if (currentRecipeProgress.ContainsKey(keyName)) {
                currentRecipeProgress[keyName] += 1;
            } else {
                currentRecipeProgress.Add(
                    keyName, 
                    1   
                );
                currentRecipeImages.Add(
                    keyName,
                    fruit.icon
                );
            }
        }
        int count = 0;
        foreach(KeyValuePair<string, int> pair in currentRecipeProgress)
        {
            // for testing ultimate recipe
            if (count >= 5) {
                break;
            }
            itemList[count].Set(pair.Key, currentRecipeImages[pair.Key], pair.Key.Contains("half"), pair.Value);
            // Debug.Log("INIT: " + pair.Key + ": " + pair.Value + " " + itemList[count].fruitName + itemList[count].ToString());
            count += 1;
        }
    }

    private async void UpdateProgress(string key) {
        // Debug.Log("Update progress: " + key);
        foreach(ItemCard i in itemList)
        {
            // Debug.Log("Fruit name: " + i.fruitName);
            if (i.fruitName == key) {
                i.Add();
            }
        }
    }
    public void UpdateCutFruitForRecipe(GameObject fruit)
    {
        fruit.tag = "Untagged";
        RecipeItem cutFruit = new RecipeItem(fruit, true);
        cutFruit.fruit.name = cutFruit.fruit.name.Replace("(Clone)", "").Trim();
        // Debug.Log(fruit.name + " has been cut");
        if (currentRecipeList.Contains(cutFruit))
        {
            // Debug.Log("Successfully Removed " + cutFruit.fruit.name);
            currentRecipeList.Remove(cutFruit);
            combo ++;
            UpdateComboUI(fruit);
        }
        else
        {
            score -= wrongIngredientPenalty;
            combo = 0;
            if (score < 0) score = 0;
            UpdateComboUI(fruit);
            // Debug.Log("Wrong fruit cut. Score: " + score);
            // currentRecipe = null;
            // spawnFruit.StartNextRecipe();
            UpdateScoreUI();
            return;
        }

        string keyName = cutFruit.fruit.name + "half";
        // Debug.Log("update cut: " + keyName + " " + currentRecipeProgress.ContainsKey(keyName));
        if (currentRecipeProgress.ContainsKey(keyName)) {
            currentRecipeProgress[keyName] -= 1;
            UpdateProgress(keyName);
        }

        if (currentRecipeList.Count == 0 && currentRecipe != null)
        {
            score += currentRecipe.reward;
            currentRecipe = null;
            spawnFruit.StartNextRecipe(currentRecipeNumber);
            // Debug.Log("Completed recipe! Score: " + score);
        }

        UpdateScoreUI();
    }

    // true if correct fruit.
    public bool UpdateGrabFruitForRecipe(GameObject fruit)
    {
        // my and yj call this when yall grab a fruit
        RecipeItem cutFruit = new RecipeItem(fruit, false);
        cutFruit.fruit.name = cutFruit.fruit.name.Replace("(Clone)", "").Trim();
        if (!spawnFruit.isAFruit(fruit)) return true; // play the true sound, but ignore.

        if (currentRecipeList.Contains(cutFruit))
        {
            currentRecipeList.Remove(cutFruit);
            combo ++;
            UpdateComboUI(fruit);
        }
        else
        {
            combo = 0;
            score -= wrongIngredientPenalty;
            if (score < 0) score = 0;
            UpdateComboUI(fruit);
            // Debug.Log("Wrong fruit grabbed. Score: " + score);
            // currentRecipe = null;
            // spawnFruit.StartNextRecipe();
            UpdateScoreUI();
            return false;
        }

        // Debug.Log("update grab: " + cutFruit.fruit.name + " " + currentRecipeProgress.ContainsKey(cutFruit.fruit.name));
        if (currentRecipeProgress.ContainsKey(cutFruit.fruit.name)) {
            currentRecipeProgress[cutFruit.fruit.name] -= 1;
            UpdateProgress(cutFruit.fruit.name);
        }
        else
        {
            score -= wrongIngredientPenalty;
            if (score < 0) score = 0;
            // Debug.Log("Wrong fruit grabbed: Score: " + score);
            UpdateScoreUI();
            return false;
        }

        if (currentRecipeList.Count == 0 && currentRecipe != null)
        {
            score += currentRecipe.reward;
            currentRecipe = null;
            spawnFruit.StartNextRecipe(currentRecipeNumber);
            // Debug.Log("Completed recipe! Score: " + score);
        }
        
        UpdateScoreUI();
        return true;
    }

    void Start()
    {
        RatingManager.Instance.Set();

        levelSettings = new ArrayList();
        levelSettings.Add(new Level(new int[]{4}, 700, 30, 3f, 5));
        levelSettings.Add(new Level(new int[]{3}, 1000, 40, 1.3f, 3));
        levelSettings.Add(new Level(new int[]{2,2,3}, 2000, 40, 1f, 3));
        levelSettings.Add(new Level(new int[]{3,2}, 3000, 50, 0.8f, 3));
        levelSettings.Add(new Level(new int[]{3,3,2}, 4000, 50, 0.8f, 2));
        levelSettings.Add(new Level(new int[]{3,3,2}, 5000, 60, 0.7f, 2));

        // levelSettings.Add(new Level(new int[]{3}, 700, 40, 1.3f, 3));
        // levelSettings.Add(new Level(new int[]{2,3}, 1000, 30, 1, 3));
        // levelSettings.Add(new Level(new int[]{2,2,2,2,2,2,3,3,1,1}, 1500, 40, 0.9f, 3));
        // levelSettings.Add(new Level(new int[]{2,2,2,3,3,1}, 2000, 40, 0.8f, 5));
        // levelSettings.Add(new Level(new int[]{3,2,2,1}, 2500, 40, 0.7f, 5));
        // levelSettings.Add(new Level(new int[]{2,1}, 3000, 30, 0.6f, 5));
        // levelSettings.Add(new Level(new int[]{2,1,1}, 4000, 30, 0.8f, 5));

        InitialiseScoreCalorieCounter();

        hands = FindObjectsOfType(typeof(MovementTrack)) as MovementTrack[];
        if (hands.Length != 2) {
            Debug.LogError("Number of tracking hands is not 2!!!");
        }

        combo = 0;
        UpdateComboUI(null);

        
        //testing***
        // currentRecipe = dummyRecipe;
        // StartCoroutine(SimulateOrderTicket());
    }

    public void WrongOrder() {
        wrongOrders++;
        RatingManager.Instance.Set();
        if (wrongOrders == maxWrongOrder) {
            //set Game Over Screen
            orderMachine.PrintGameOver();
            AudioSource.PlayClipAtPoint(gameOverSound, orderMachine.transform.position);
        }

        combo = 0;
        UpdateComboUI(null);
    }

    public bool isGameOver() {
        return wrongOrders >= maxWrongOrder;
    }

    void FixedUpdate() {
        if (gameStarted && !isGameOver()) {
            kcal += (hands[0].GetKcal() + hands[1].GetKcal());
            hands[0].Reset();
            hands[1].Reset();
            UpdateCalorieUI();
        }
    }

    public void BeginGame() {
        atLastLevel = false;
        gameStarted = true;
        combo = 0;
        UpdateComboUI(null);
        spawnFruit.maxYHeight = height * 0.01f * 1.30f;
        spawnFruit.minYHeight = 1;
        SaveGame();
        // spawnFruit.xRadius = height * 0.01;

        score = 0;
        wrongOrders = 0;
        kcal = 0;
        RatingManager.Instance.Set();
        UpdateScoreUI();
        UpdateCalorieUI();
        orderMachine.Flush();
        currentRecipeNumber = 0;
        currentRecipe = null;
        currentLevel = Instructions.Instance.isRunning() ? -1 : 0;
        currentRecipeList.Clear();
        currentRecipeProgress = new SortedDictionary<string, int>();
        currentRecipeImages = new Dictionary<string, Sprite>();
        for (int i = 0; i < itemList.Count; i++)
        {
            itemList[i].Reset();
        }

        spawnFruit.StartNextRecipe(0);
        StartCoroutine(spawnFruit.SpawnFruit());
        EyeTrack eyeTrack = FindObjectsOfType(typeof(EyeTrack))[0] as EyeTrack;
        eyeTrack.Reset();
    }

    IEnumerator SimulateOrderTicket()
    {
        yield return new WaitForSeconds(5f);
        orderMachine.PrintOrder(currentRecipe, true);
        StartCoroutine(SimulateOrderTicket());
    }

    public int GetTotalRecipes() {
        return currentRecipeNumber;
    }

    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("Height"))
        {
            height = PlayerPrefs.GetInt("Height");
            mass = PlayerPrefs.GetInt("Mass");
            Debug.Log("Game data loaded!");
        }
        else
            Debug.LogError("There is no save data!");
    }

    void SaveGame()
    {
        PlayerPrefs.SetInt("Height", height);
        PlayerPrefs.SetInt("Mass", mass);
        PlayerPrefs.Save();
        Debug.Log("Game data saved!");
    }

    public void NextLevel() {
        if (currentLevel < levelSettings.Count - 1) {
            currentLevel++;
        }

        string[] difficultyList = new string[] {"tutorial", "easy", "normal", "hard", "expert","insane"};
        levelText.text = difficultyList[currentLevel];
    }

    public string getDifficulty() {
        if (atLastLevel) return "";
        if (currentLevel == levelSettings.Count - 1) {
            atLastLevel = true;
        }
        AudioSource.PlayClipAtPoint(nextLevelSound, orderMachine.transform.position);
        return levelText.text;
    }

    public Level GetLevel() {
        if (currentLevel > levelSettings.Count - 1) {
            currentLevel = levelSettings.Count - 1;
            return (Level) levelSettings[currentLevel];
        }
        return (Level) levelSettings[currentLevel];
    }

    public int GetWrongOrdersLeft() {
        return maxWrongOrder - wrongOrders;
    }

    private void UpdateScoreUI() {
        foreach (TMP_Text scoreText in scoreTexts) scoreText.text = score.ToString();
    }

    private void UpdateCalorieUI() {
        foreach (TMP_Text calorieText in calorieTexts) calorieText.text = ((int)kcal).ToString() + 
            "." + ((int)(kcal * 10) % 10).ToString() ;
    }

    private void UpdateComboUI(GameObject fruit) {
        if (combo % 5 == 0 && fruit != null) {
            AudioSource.PlayClipAtPoint(comboSound, fruit.transform.position);
            GameObject comboPrefabInstance = Instantiate(comboPrefab, fruit.transform.parent);
            comboPrefabInstance.transform.localPosition = fruit.transform.position;
            comboPrefabInstance.transform.localScale = new Vector3(2,2,2);
            comboPrefabInstance.GetComponent<Rigidbody>().AddForce(new Vector3(0,1,2));
            Destroy(comboPrefabInstance, 4f);
            comboPrefabInstance.GetComponent<ComboPoints>().Set(combo);
            score += combo * 2;
        }
        comboText.text =  "x" + combo;
    }
    

    public void InitialiseScoreCalorieCounter() {
        GameObject[] scoreGameObjects = GameObject.FindGameObjectsWithTag("score");
        GameObject[] calorieGameObjects = GameObject.FindGameObjectsWithTag("kcal");

        scoreTexts = new List<TMP_Text>();
        calorieTexts = new List<TMP_Text>();
        foreach(GameObject s in scoreGameObjects) {
            scoreTexts.Add(s.GetComponent<TMP_Text>());
        }
        foreach(GameObject s in calorieGameObjects) {
            calorieTexts.Add(s.GetComponent<TMP_Text>());
        }

        UpdateScoreUI();
        UpdateCalorieUI();
    }
}