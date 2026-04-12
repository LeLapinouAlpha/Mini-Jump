using System;
using UnityEngine;
using UnityEngine.InputSystem.Android;
using UnityEngine.U2D;

public class ProjectileLogic : MonoBehaviour
{
    float currentPositionX;
    float currentPositionY;
    float initialPositionX;

    SpriteRenderer spriteRenderer;
    System.Random random;
    public GameObject spawningObject;

    float colorR;
    float colorG;
    float colorB;

    public float cooldownChangementColors;
    float counterColors;

    public float vitesseHorizontale;
    public float vitesseVerticale;
    public float distanceForDespawn;

    bool moveRight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentPositionX = this.transform.position.x;
        currentPositionY = this.transform.position.y;
        initialPositionX = this.transform.position.x;

        this.moveRight = (this.transform.lossyScale.x > 0);

        random = new System.Random();
        spriteRenderer = GetComponent<SpriteRenderer>();
        ChangementCouleur();
    }

    // Update is called once per frame
    void Update()
    {
        if(this.cooldownChangementColors <= this.counterColors)
        {
            ChangementCouleur();
        }
        else
        {
            this.counterColors += Time.deltaTime;
        }

        if(Math.Abs(currentPositionX) >= Math.Abs(initialPositionX) + distanceForDespawn)
        {
            this.gameObject.SetActive(false);
        }
        Move();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Projectiles"))
        {
            this.spriteRenderer.enabled = false;
            this.GetComponent<Collider2D>().enabled = false;
            GameObject newGameObject = Instantiate(this.spawningObject, this.transform.position, this.transform.rotation, this.transform);
            this.GetComponent<ProjectileLogic>().enabled = false;
        }
    }

    void Move()
    {
        if (this.moveRight)
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
    void ChangementCouleur()
    {
        this.colorR = (float)(this.random.Next(0, 256));
        this.colorG = (float)(this.random.Next(0, 256));
        this.colorB = (float)(this.random.Next(0, 256));
        this.spriteRenderer.color = new Color(this.colorR / 255, this.colorG / 255, this.colorB / 255, 1f);
        this.counterColors = 0;
    }
}
