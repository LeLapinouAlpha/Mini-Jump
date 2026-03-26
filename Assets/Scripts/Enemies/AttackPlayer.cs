using Unity.VisualScripting;
using UnityEngine;

public class AttackPlayer : MonoBehaviour
{
    PlayerHealth playerHealth;
    public float damage;

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
        if (collision.gameObject.CompareTag("Player"))
            this.playerHealth.moneyHealth -= this.damage;
        Debug.Log(this.playerHealth.moneyHealth);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

    }


    //TODO méthode pour gérer le cooldown des attaques
    //rendre invincible certaines frames
}
