using UnityEngine;
using System;

public class LoucheLogic : MonoBehaviour
{
    //Collider2D collider;
    //LoucheAnimations animationsScript;

    float currentPositionX;
    float initialPositionX;
    public float travellingDistance;
    public float vitesse;
    bool moveRight = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       // collider = this.GetComponent<Collider2D>();
        //animationsScript = this.GetComponent<LoucheAnimations>();

        //animationsScript.PlayWalkAnimation(true);

        initialPositionX = this.transform.position.x;
        currentPositionX = this.transform.position.x;
    }
    // Update is called once per frame
    void Update()
    {
        RightOrLeft();
        if (moveRight)
        {
            this.currentPositionX += this.vitesse * Time.deltaTime;
            this.transform.position = new Vector3(this.currentPositionX, this.transform.position.y, this.transform.position.z);
        }
        if (!moveRight)
        {
            this.currentPositionX -= this.vitesse * Time.deltaTime;
            this.transform.position = new Vector3(this.currentPositionX, this.transform.position.y, this.transform.position.z);
        }
    }

    void RightOrLeft()
    {
        if (currentPositionX <= initialPositionX)
        {
            moveRight = true;
        }
        if (currentPositionX >= initialPositionX + travellingDistance)
        {
            moveRight = false;
        }
    }
}