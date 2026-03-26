using UnityEngine;
using System;

public class LoucheLogic : MonoBehaviour
{
    LoucheAnimations animationsScript;

    float currentPositionX;
    float initialPositionX;
    bool moveRight = true;
    bool canMove;

    public float travellingDistance;
    public float vitesse;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialPositionX = this.transform.position.x;
        currentPositionX = this.transform.position.x;
        animationsScript = this.GetComponent<LoucheAnimations>();

        canMove = true;
        animationsScript.PlayWalkAnimation(true);
    }
    // Update is called once per frame
    void Update()
    {
        Move(canMove);
        FaceDirection();
    }

    void Move(bool canMove)
    {
        if (canMove)
        {
            animationsScript.PlayWalkAnimation(true);
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

    void FaceDirection()
    {
        if(moveRight && this.transform.localScale.x != -1)
        {
            this.transform.localScale = new Vector3(-1, this.transform.localScale.y, this.transform.localScale.z);
        }
        else if (!moveRight && this.transform.localScale.x != 1)
        {
            this.transform.localScale = new Vector3(1, this.transform.localScale.y, this.transform.localScale.z);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canMove = false;
            if(collision.transform.position.x > this.transform.position.x)
            {
                this.transform.localScale = new Vector3(1, this.transform.localScale.y, this.transform.localScale.z);
            }
            else
            {
                this.transform.localScale = new Vector3(-1, this.transform.localScale.y, this.transform.localScale.z);
            }
            this.animationsScript.animator.Play("Attack");
        }
    }
}