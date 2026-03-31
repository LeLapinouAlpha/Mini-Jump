using UnityEngine;

public class BlobLogic : MonoBehaviour
{
    Transform parentTransform;
    public bool isRight;

    float currentPositionX;
    float initialPositionX;
    float vitesse;
    float distanceForDespawn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialPositionX = this.transform.position.x;
        currentPositionX = this.transform.position.x;

        parentTransform = GetComponentInParent<Transform>();
        vitesse = GetComponentInParent<LoucheLogic>().vitesseBlob;
        distanceForDespawn = GetComponentInParent<LoucheLogic>().distanceForDespawn;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPositionX >= initialPositionX + distanceForDespawn)
        {
            this.gameObject.SetActive(false);
        }

        if (isRight)
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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        this.gameObject.SetActive(false);
    }
}
