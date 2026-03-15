using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    PlayerHealth playerHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
       // if ()
        //{
            this.playerHealth.moneyHealth -= this.playerHealth.damage;
        //}
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
       
    }
}
