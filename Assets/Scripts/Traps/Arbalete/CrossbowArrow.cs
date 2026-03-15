using UnityEngine;
using UnityEngine.UIElements;

public class Crossbowshoot : MonoBehaviour
{
    float initialPositionY;
    float initialPositionX;
    float currentPositionX;
    public float distance;
    public float distanceForDespawn;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ;
        this.initialPositionX = this.transform.position.x;
        this.initialPositionY = this.transform.position.y;
        this.currentPositionX = this.initialPositionX;
    }

    // Update is called once per frame
    void Update()
    {
        this.currentPositionX += this.distance * Time.deltaTime;
        this.GetComponentInChildren<Transform>().position = new Vector3(this.currentPositionX, this.initialPositionY, 0f);
        if (this.currentPositionX >= this.initialPositionX + this.distanceForDespawn) 
        {
            this.gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        this.gameObject.SetActive(false);
    }
}
