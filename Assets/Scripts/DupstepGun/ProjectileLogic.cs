using UnityEngine;
using System;

public class ProjectileLogic : MonoBehaviour
{
    float currentPositionX;
    float currentPositionY;
    float initialPositionX;

    public float vitesseHorizontale;
    public float vitesseVerticale;
    public float distanceForDespawn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentPositionX = this.transform.position.x;
        currentPositionY = this.transform.position.y;
        initialPositionX = this.transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        if(Math.Abs(currentPositionX) >= Math.Abs(initialPositionX) + distanceForDespawn)
        {
            this.gameObject.SetActive(false);
        }

        if (this.transform.rotation.z > 0)
        {
            this.currentPositionX += this.vitesseHorizontale * Time.deltaTime;
            this.currentPositionY += this.vitesseVerticale * Time.deltaTime;

            this.transform.position = new Vector3(this.currentPositionX, this.currentPositionY, this.transform.position.z);
        }
        else
        {
            this.currentPositionX -= this.vitesseHorizontale * Time.deltaTime;
            this.currentPositionY += this.vitesseVerticale * Time.deltaTime;
            this.transform.position = new Vector3(this.currentPositionX, this.currentPositionY, this.transform.position.z);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        this.gameObject.SetActive(false);
    }
}
