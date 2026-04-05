using UnityEngine;
using System;

public class ProjectileLogic : MonoBehaviour
{
    float currentPositionX;
    float initialPositionX;

    public float vitesse;
    public float distanceForDespawn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentPositionX = this.transform.position.x;
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
            this.currentPositionX += this.vitesse * Time.deltaTime;
            this.transform.position = new Vector3(this.currentPositionX, this.transform.position.y, this.transform.position.z);
        }
        else
        {
            this.currentPositionX -= this.vitesse * Time.deltaTime;
            this.transform.position = new Vector3(this.currentPositionX, this.transform.position.y, this.transform.position.z);
        }
    }
}
