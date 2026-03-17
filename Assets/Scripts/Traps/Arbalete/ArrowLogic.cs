using UnityEngine;

public class ArrowLogic : MonoBehaviour
{
    public float distance;
    public float distanceForDespawn;

    float initialPositionY;
    float initialPositionX;
    float currentPositionX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialPositionY = this.transform.position.y;
        initialPositionX = this.transform.position.x;
        currentPositionX = this.transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        //Calculate and apply movement to the 
        this.currentPositionX += this.distance * Time.deltaTime;
        this.transform.position = new Vector3(this.currentPositionX, this.initialPositionY, 0f);

        //Despawn arrow if it goes farther away than distanceForDespawn value
        if (currentPositionX >= initialPositionX + distanceForDespawn)
        {
            this.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        this.gameObject.SetActive(false);
    }

}
