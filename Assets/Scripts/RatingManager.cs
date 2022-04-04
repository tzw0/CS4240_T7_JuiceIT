using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RatingManager : MonoBehaviour
{
    private List<GameObject> stars;
    public static RatingManager _instance;
    public TMP_Text ChalkTitle;
    public GameObject starObj;
    // Start is called before the first frame update
    void Start()
    {
        stars = new List<GameObject>();
    }

    public static RatingManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (Instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Set() {
        int numStars = GameManager.Instance.GetWrongOrdersLeft();
        while (numStars > stars.Count) {
            stars.Add(Instantiate(starObj, transform));
        }

        while (stars.Count > 0 && numStars < stars.Count) { //lost a star
            StartCoroutine(blinkStar(stars[stars.Count - 1]));
            stars.RemoveAt(stars.Count - 1);
        }

        if (numStars <= 0) {
            ChalkTitle.text = "GameOver";
        } else {
            ChalkTitle.text = "Our Rating";
        }
    }

    IEnumerator  blinkStar(GameObject star) {
        Image starImage = star.GetComponent<Image>();
        for (int i = 0; i < 3; i ++) {
            starImage.enabled = false;
            yield return new WaitForSeconds(0.3f);
            starImage.enabled = true;
            yield return new WaitForSeconds(0.3f);
        }
        Destroy(star);
    }
}
