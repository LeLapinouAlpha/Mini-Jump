using UnityEngine;
using UnityEngine.SceneManagement;
public class NiveauSuivant: MonoBehaviour
{

    public int BuildIndex ;  // Variable niveau suivant 
    

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        

        if (collision.CompareTag("Player"))
        {
            SceneManager.LoadScene(BuildIndex);
            
        }

    }


}
