using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderTicket : MonoBehaviour
{
    public Text OrderNumber;
    public Text Cashier;
    public Text DateTimeText;
    public Text OrderName;
    public Image OrderLogo;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Set(int orderNumber, Recipe recipe) 
    {
        OrderNumber.text = "Order: #" + orderNumber.ToString();
        Cashier.text = "XXXX";
        DateTimeText.text = System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
        OrderName.text = recipe.name;
        OrderLogo.sprite = recipe.icon;
    }

    public void GameOver(Sprite image) 
    {
        OrderNumber.text = GameManager.Instance.maxWrongOrder + " Wrong Orders?!";
        Cashier.text = "XXXX";
        DateTimeText.text = System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
        OrderName.text = "Status: TERMINATED";
        OrderLogo.sprite = image;
    }
}
