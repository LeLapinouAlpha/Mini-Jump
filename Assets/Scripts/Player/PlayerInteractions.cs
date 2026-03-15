using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public PlayerHealth playerHealth;

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
        if(collision.gameObject.layer == 9)
            this.playerHealth.moneyHealth -= this.playerHealth.damage;
            Debug.Log(this.playerHealth.moneyHealth);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
       
    }


    //TODO méthode pour gérer le cooldown des attaques
    //rendre invincible certaines frames
}
