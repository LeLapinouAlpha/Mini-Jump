using UnityEngine;

public class Placementcaméragrotte : MonoBehaviour
{
    public Camera cameraPositionGrotte; // Variable camera
    

    private void OnTriggerEnter2D(Collider2D collision) // Fonction lié au collider 
    {

        if (collision.CompareTag("Grotte"))
        {
            cameraPositionGrotte.GetComponent<CameraFollow>().enabled = false;

            cameraPositionGrotte.transform.position = new Vector3(208f, -1f, -10f);

            cameraPositionGrotte.orthographicSize = 7;
        }
;
    }













}
