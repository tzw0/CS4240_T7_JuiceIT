using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using TMPro;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public Text MassText;
    public Text HeightText;
    public TMP_Text MemoryText;
    public GameObject UI;
    private int mode = 0;
    private int massIsUpdating = 0;
    private int heightIsUpdating = 0;
    private float nextMassUpdateTime = 0.0f;
    private float nextHeightUpdateTime = 0.0f;
    private float updatePeriod = 0.1f;

    private int upperMass = 200;
    private int upperHeight = 300;
    private int lowerHeight = 120;
    private int lowerMass = 30;
    public GameObject startGame;
    public GameObject gameOver;
    public GameObject pause;
    public GameObject crateWallPrefab;
    private GameObject crateWall = null;
    private Flicker flicker;
    private Flicker1 flicker1;
    // Start is called before the first frame update
       public static UIManager Instance
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
        GameManager.Instance.LoadGame();
        MassText.text = GameManager.Instance.mass.ToString() + "kg";
        HeightText.text = GameManager.Instance.height.ToString() + "cm";
        flicker = (Flicker)FindObjectOfType<Flicker>();
        flicker1 = (Flicker1)FindObjectOfType<Flicker1>();
        
        startGame.SetActive(true);
        gameOver.SetActive(false);
        pause.SetActive(false);
        crateWall = Instantiate(crateWallPrefab, transform);
        crateWall.transform.localPosition = new Vector3(227, 479, 73);
        crateWall.transform.localScale = new Vector3(200,200,200);
        crateWall.transform.localRotation = Quaternion.Euler(0,90,0);
    }

    public bool userResume() {
        return mode != 2;
    }

    void Update()
    {   
        if (mode == 0) {

            if (((massIsUpdating == 1 && GameManager.Instance.mass < upperMass) || 
            (massIsUpdating == -1 && GameManager.Instance.mass > lowerMass))
             && Time.time > nextMassUpdateTime) {
                GameManager.Instance.mass += massIsUpdating;
                MassText.text = GameManager.Instance.mass.ToString() + "kg";
                nextMassUpdateTime = Time.time + updatePeriod;
                if (!gameObject.GetComponent<AudioSource>().isPlaying) gameObject.GetComponent<AudioSource>().Play();
            }

            if (((heightIsUpdating == 1 && GameManager.Instance.height < upperHeight) || 
                (heightIsUpdating == -1 && GameManager.Instance.height > lowerHeight))
                && Time.time > nextHeightUpdateTime) {
                GameManager.Instance.height += heightIsUpdating;
                HeightText.text = GameManager.Instance.height.ToString() + "cm";
                nextHeightUpdateTime = Time.time + updatePeriod;
                if (!gameObject.GetComponent<AudioSource>().isPlaying) gameObject.GetComponent<AudioSource>().Play();
            }

            if (SteamVR_Input.GetStateDown("MoveUp", SteamVR_Input_Sources.RightHand) &&
                GameManager.Instance.height < upperHeight) {
                GameManager.Instance.height ++;
                HeightText.text = GameManager.Instance.height.ToString() + "cm";
                heightIsUpdating = 1;
                nextHeightUpdateTime = Time.time + updatePeriod * 2;
                if (!gameObject.GetComponent<AudioSource>().isPlaying) gameObject.GetComponent<AudioSource>().Play();
            }

            if (SteamVR_Input.GetStateDown("MoveDown", SteamVR_Input_Sources.RightHand) &&
                GameManager.Instance.height > lowerHeight) {
                GameManager.Instance.height --;
                HeightText.text = GameManager.Instance.height.ToString() + "cm";
                heightIsUpdating = -1;
                nextHeightUpdateTime = Time.time + updatePeriod * 2;
                if (!gameObject.GetComponent<AudioSource>().isPlaying) gameObject.GetComponent<AudioSource>().Play();
            }

            if (SteamVR_Input.GetStateDown("MoveUp", SteamVR_Input_Sources.LeftHand) &&
                GameManager.Instance.mass < upperMass) {
                GameManager.Instance.mass++;
                MassText.text = GameManager.Instance.mass.ToString() + "kg";
                massIsUpdating = 1;
                nextMassUpdateTime = Time.time + updatePeriod * 2;
                if (!gameObject.GetComponent<AudioSource>().isPlaying) gameObject.GetComponent<AudioSource>().Play();
            }

            if (SteamVR_Input.GetStateDown("MoveDown", SteamVR_Input_Sources.LeftHand) &&
                GameManager.Instance.mass > lowerMass) {
                GameManager.Instance.mass--;
                MassText.text = GameManager.Instance.mass.ToString() + "kg";
                massIsUpdating = -1;
                nextMassUpdateTime = Time.time + updatePeriod * 2;
                if (!gameObject.GetComponent<AudioSource>().isPlaying) gameObject.GetComponent<AudioSource>().Play();
            }

            if (SteamVR_Input.GetStateUp("MoveDown", SteamVR_Input_Sources.LeftHand) || 
                SteamVR_Input.GetStateUp("MoveUp", SteamVR_Input_Sources.LeftHand) || 
                SteamVR_Input.GetStateUp("MoveLeft", SteamVR_Input_Sources.LeftHand) || 
                SteamVR_Input.GetStateUp("MoveRight", SteamVR_Input_Sources.LeftHand)) {
                massIsUpdating = 0;
            }

            if (SteamVR_Input.GetStateUp("MoveDown", SteamVR_Input_Sources.RightHand) || 
                SteamVR_Input.GetStateUp("MoveUp", SteamVR_Input_Sources.RightHand) || 
                SteamVR_Input.GetStateUp("MoveLeft", SteamVR_Input_Sources.RightHand) || 
                SteamVR_Input.GetStateUp("MoveRight", SteamVR_Input_Sources.RightHand)) {
                heightIsUpdating = 0;
            }

            if (SteamVR_Input.GetStateDown("GrabPinch", SteamVR_Input_Sources.LeftHand) ||
                SteamVR_Input.GetStateDown("GrabPinch", SteamVR_Input_Sources.RightHand)) {
                Debug.Log("Game Started");
                mode = 1;
                flicker.enabled = false;
                flicker1.enabled = false;
                UI.SetActive(false);
                foreach (Transform child in crateWall.transform) {
                    child.GetComponent<Rigidbody>().useGravity = true;
                    child.GetComponent<Rigidbody>().isKinematic = false;
                    child.GetComponent<Rigidbody>().AddForce(
                        new Vector3(Random.Range(-20,20),Random.Range(-20,20),Random.Range(-20,20)) * 20);
                }

                Destroy(crateWall, 3);
                Instructions.Instance.JumpTo(4);
                GameManager.Instance.BeginGame();
            }

            if (SteamVR_Input.GetStateDown("GrabGrip", SteamVR_Input_Sources.LeftHand) ||
                SteamVR_Input.GetStateDown("GrabGrip", SteamVR_Input_Sources.RightHand)) {
                Debug.Log("Instructions Started");
                Instructions.Instance.StartInstructions();
            }

        } else if (mode == 1 && GameManager.Instance.isGameOver()) {
            mode = 0;
            UI.SetActive(true);
            startGame.SetActive(false);
            gameOver.SetActive(true);

            EyeTrack eyeTrack = FindObjectsOfType(typeof(EyeTrack))[0] as EyeTrack;
            double avgLookTime = eyeTrack.GetLookTime().TotalSeconds / GameManager.Instance.GetTotalRecipes() * 1.0;
            MemoryText.text = avgLookTime.ToString("F1") + "s";

            Debug.Log(avgLookTime.ToString("F1") +  "s, total time: " +  eyeTrack.GetLookTime().TotalSeconds + ", recipes:" + GameManager.Instance.GetTotalRecipes());

            crateWall = Instantiate(crateWallPrefab, transform);
            crateWall.transform.localPosition = new Vector3(227, 479, 73);
            crateWall.transform.localScale = new Vector3(200,200,200);
            crateWall.transform.localRotation = Quaternion.Euler(0,90,0);

            flicker.enabled = true;
            flicker1.enabled = true;
            GameManager.Instance.InitialiseScoreCalorieCounter();
        } else if (mode == 2) {
            if (SteamVR_Input.GetStateDown("GrabPinch", SteamVR_Input_Sources.LeftHand) ||
                SteamVR_Input.GetStateDown("GrabPinch", SteamVR_Input_Sources.RightHand)) {
                Debug.Log("Game Started");
                flicker.enabled = false;
                flicker1.enabled = false;
                UI.SetActive(false);
                foreach (Transform child in crateWall.transform) {
                    child.GetComponent<Rigidbody>().useGravity = true;
                    child.GetComponent<Rigidbody>().isKinematic = false;
                    child.GetComponent<Rigidbody>().AddForce(
                        new Vector3(Random.Range(-20,20),Random.Range(-20,20),Random.Range(-20,20)) * 20);
                }

                Destroy(crateWall, 3);
                Instructions.Instance.JumpTo(4);
                GameManager.Instance.BeginGame();
            }

            if (SteamVR_Input.GetStateDown("GrabGrip", SteamVR_Input_Sources.LeftHand) ||
                SteamVR_Input.GetStateDown("GrabGrip", SteamVR_Input_Sources.RightHand)) {
                    Debug.Log("Continue Game");
                    mode = 1;
                    flicker.enabled = false;
                    flicker1.enabled = false;
                    UI.SetActive(false);
                    foreach (Transform child in crateWall.transform) {
                        child.GetComponent<Rigidbody>().useGravity = true;
                        child.GetComponent<Rigidbody>().isKinematic = false;
                        child.GetComponent<Rigidbody>().AddForce(
                            new Vector3(Random.Range(-20,20),Random.Range(-20,20),Random.Range(-20,20)) * 20);
                    }

                    Destroy(crateWall, 3);
                }
        }
    }
}
