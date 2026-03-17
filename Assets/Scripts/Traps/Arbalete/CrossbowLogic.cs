using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CrossbowLogic : MonoBehaviour
{
    [Header("Arrow")]
    public GameObject arrow;
    public float distance;
    public float distanceForDespawn;

    float arrowInitialPositionY;
    float initialPositionX;
    float currentPositionX;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Setup positions
        arrowInitialPositionY = arrow.transform.position.y;
        initialPositionX = arrow.transform.position.x;
        currentPositionX = arrow.transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        //Calculate and apply movement to the arrow
        currentPositionX += this.distance * Time.deltaTime;
        arrow.transform.position = new Vector3(currentPositionX, arrowInitialPositionY, 0f);

        //Despawn arrow if it goes farther away than distanceForDespawn value
        if (currentPositionX >= initialPositionX + distanceForDespawn) 
        {
            arrow.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        arrow.SetActive(false);
    }


    
}
