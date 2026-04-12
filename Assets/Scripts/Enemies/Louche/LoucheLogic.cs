using UnityEngine;
using System;
using Unity.VisualScripting;

public class LoucheLogic : MonoBehaviour
{
    public PlayerHealth playerHealth;
    LoucheAnimations animationsScript;
    SpriteRenderer spriteRenderer;
    Collider2D collider2d;
    Vector2 initialCollider2dOffset;

    float currentPositionX;
    float initialPositionX;
    bool moveRight = true;
    bool canMove;

    public float travellingDistance;
    public float vitesse;
    public float attackSpeed;
    float attackSpeedCounter;

    [Header("Blob The blob")]
    public GameObject spawningObject;
    public float damage;
    public float vitesseBlob;
    public float distanceForDespawn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialPositionX = this.transform.position.x;
        currentPositionX = this.transform.position.x;

        animationsScript = this.GetComponent<LoucheAnimations>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        collider2d = this.GetComponentInChildren<Collider2D>();
        initialCollider2dOffset = collider2d.offset;

        canMove = true;
        attackSpeedCounter = attackSpeed;
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
            else
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
        if (moveRight && !this.spriteRenderer.flipX)
        {
            this.spriteRenderer.flipX = true;
            this.collider2d.offset = new Vector2(this.initialCollider2dOffset.x, this.initialCollider2dOffset.y);
        }
        else if (!moveRight && this.spriteRenderer.flipX)
        {
            this.spriteRenderer.flipX = false;
            this.collider2d.offset = new Vector2(this.initialCollider2dOffset.x * (-1), this.initialCollider2dOffset.y);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canMove = false;
            if (attackSpeedCounter >= attackSpeed)
            {
                attackSpeedCounter = 0;
                this.spriteRenderer.flipX = collision.transform.position.x > this.transform.position.x;
                this.animationsScript.animator.Play("Attack");
            }
            else
            {
                attackSpeedCounter += Time.deltaTime;
            }
        }

    }
    public void SpawnBlob()
    {
        GameObject newGameObject = Instantiate(this.spawningObject, this.transform.position, Quaternion.identity, this.transform);
        newGameObject.GetComponent<AttackPlayer>().damage = this.damage;
        newGameObject.GetComponent<BlobLogic>().isRight = this.moveRight;
        newGameObject.GetComponent<SpriteRenderer>().flipX = this.moveRight;
        if (!this.moveRight)
        {
            newGameObject.GetComponent<Collider2D>().offset = new Vector2(newGameObject.GetComponent<Collider2D>().offset.x * (-1), newGameObject.GetComponent<Collider2D>().offset.y);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canMove = true;
        }
    }
}