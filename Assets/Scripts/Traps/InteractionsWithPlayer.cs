using Unity.VisualScripting;
using UnityEngine;

public class InteractionsWithPlayer : MonoBehaviour
{
    PlayerHealth playerHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerHealth = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Damaging"))
            this.playerHealth.moneyHealth -= this.playerHealth.damage;
            Debug.Log(this.playerHealth.moneyHealth);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
       
    }


    //TODO méthode pour gérer le cooldown des attaques
    //rendre invincible certaines frames
}
