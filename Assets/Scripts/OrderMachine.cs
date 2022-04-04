using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderMachine : MonoBehaviour
{
    public Light orderLight;
    public GameObject OrderTicket;
    private int counter = 1;
    private GameObject currentTicket;
    public GameObject rewardPrefab;
    public Sprite gameOverFax;

    private Recipe previousRecipe;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PrintGameOver() {
        StartCoroutine(Blink(7));
        if (currentTicket != null) {
            currentTicket.GetComponent<Rigidbody>().AddForce(Quaternion.Euler(62, 180, 0) * -Vector3.forward * 10f);
            currentTicket.GetComponent<Rigidbody>().useGravity = true;
            Destroy(currentTicket, 3f);
        }

        GameObject ticket = Instantiate(OrderTicket);
        ticket.transform.parent = transform;
        ticket.transform.localScale = new Vector3(1.2f,2.04103f,5.658737f);
        ticket.GetComponent<OrderTicket>().GameOver(gameOverFax);
        ticket.transform.localPosition = new Vector3(0, -2, 0.45f);
        ticket.transform.rotation = Quaternion.Euler(80, 180, 0);
        ticket.GetComponent<Rigidbody>().velocity = Quaternion.Euler(62, 180, 0) * -Vector3.forward;
        StartCoroutine(StopMoving(ticket));
        currentTicket = ticket;
        counter = 1;
    }

    public void Flush() {
        StartCoroutine(Blink(7));
        if (currentTicket != null) {
            currentTicket.GetComponent<Rigidbody>().AddForce(Quaternion.Euler(62, 180, 0) * -Vector3.forward * 10f);
            currentTicket.GetComponent<Rigidbody>().useGravity = true;
            Destroy(currentTicket, 3f);
        }

        currentTicket = null;
    }

    public void PrintOrder(Recipe recipe, bool previousRecipeCompleted)
    {
        StartCoroutine(Blink(7));
        if (currentTicket != null) {
            currentTicket.GetComponent<Rigidbody>().AddForce(Quaternion.Euler(62, 180, 0) * -Vector3.forward * 10f);
            currentTicket.GetComponent<Rigidbody>().useGravity = true;
            Destroy(currentTicket, 3f);


            GameObject reward = Instantiate(rewardPrefab, transform);
            reward.transform.localScale = new Vector3(4f,19.98782f,6.668019f);
            reward.transform.localPosition = new Vector3(0,4,-0.8f);
            reward.transform.localRotation = Quaternion.Euler(0, 180, 0);
            reward.GetComponent<Rigidbody>().AddForce(new Vector3(0,1f,0));
            Destroy(reward, 4f);

            if (previousRecipeCompleted) {
                reward.GetComponent<RecipeScore>().Set(previousRecipe.reward);    
            } else {
                reward.GetComponent<RecipeScore>().Set(0);
            }
        }

        GameObject ticket = Instantiate(OrderTicket);
        ticket.transform.parent = transform;
        ticket.transform.localScale = new Vector3(1.2f,2.04103f,5.658737f);
        ticket.GetComponent<OrderTicket>().Set(counter, recipe);
        counter ++;
        ticket.transform.localPosition = new Vector3(0, -2, 0.45f);
        ticket.transform.rotation = Quaternion.Euler(80, 180, 0);
        ticket.GetComponent<Rigidbody>().velocity = Quaternion.Euler(62, 180, 0) * -Vector3.forward;
        StartCoroutine(StopMoving(ticket));
        currentTicket = ticket;
        previousRecipe = recipe;
    }

    IEnumerator StopMoving(GameObject ticket)
    {
        yield return new WaitForSeconds(0.5f);
        ticket.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
    }

    IEnumerator Blink(int timesLeft)
    {
        yield return new WaitForSeconds(0.1f);
        orderLight.enabled = !orderLight.enabled;
        if (timesLeft > 0) {
            StartCoroutine(Blink(timesLeft - 1));
        }
    }
}
