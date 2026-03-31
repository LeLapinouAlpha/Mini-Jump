using UnityEngine;

[RequireComponent(typeof(AttackPlayer))]
public class PlateLogic : MonoBehaviour
{
    [Header("Parameters")]
    private float vitesse;
    private float distanceForDespawn;

    [Header("States")]
    private float initialPositionY;
    private float currentPositionY;

    public bool HasCoveredDistanceForDespawn()
    {
        return this.currentPositionY >= this.initialPositionY + this.distanceForDespawn;
    }

    public void InitializeParameters(float damage, float vitesse, float distanceForDespawn)
    {
        this.vitesse = vitesse;
        this.distanceForDespawn = distanceForDespawn;

        this.GetComponent<AttackPlayer>().damage = damage;
    }

    void Start()
    {
        this.initialPositionY = this.transform.position.y;
        this.currentPositionY = this.transform.position.y;
    }

    void Update()
    {
        if (this.HasCoveredDistanceForDespawn())
        {
            this.gameObject.SetActive(false);
        }

        this.currentPositionY -= this.vitesse * Time.deltaTime;
        this.transform.position = new Vector3(this.transform.position.x, this.currentPositionY, this.transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        this.gameObject.SetActive(false);
    }
}
