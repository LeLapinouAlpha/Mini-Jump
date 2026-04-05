using UnityEngine;

[RequireComponent(typeof(AttackPlayer))]
public class PlateLogic : MonoBehaviour
{
    [Header("Parameters")]
    private float velocity;
    private float despawnDistance;

    [Header("States")]
    private float initialPositionY;
    private float currentPositionY;

    public bool HasCoveredDistanceForDespawn()
    {
        return this.currentPositionY >= this.initialPositionY + this.despawnDistance;
    }

    public void InitializeParameters(float damage, float vitesse, float despawnDistance)
    {
        this.velocity = vitesse;
        this.despawnDistance = despawnDistance;

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
            Object.Destroy(this.gameObject);
        }

        this.currentPositionY -= this.velocity * Time.deltaTime;
        this.transform.position = new Vector3(this.transform.position.x, this.currentPositionY, this.transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Ennemies"))
        {
            Object.Destroy(this.gameObject);
        }
    }
}
