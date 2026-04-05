using Assets.Scripts.Gameplay;
using Assets.Scripts.Utils;
using UnityEngine;

[RequireComponent(typeof(FlyingWardrobeAnimations)), RequireComponent(typeof(PeriodicAttack)), RequireComponent(typeof(PatrolMover))]
public class FlyingWardrobeLogic : MonoBehaviour, IAttackCondition
{
    [Header("Components References")]
    public FlyingWardrobeAnimations animations;
    public PatrolMover patrolMover;

    [Header("Attack Parameters")]
    public int plateCount = 3;
    public Vector2 plateOffsetRange;
    public float spawnTriggerDistThresh = 0.5f;

    [Header("Plate The Plate")]
    public GameObject platePrefab;
    public float plateDamage;
    public float plateVelocity;
    public float plateDespawnDistance;

    private void FindComponents()
    {
        this.animations = this.animations.GetComponent<FlyingWardrobeAnimations>();
        this.patrolMover = this.GetComponent<PatrolMover>();
    }

    void Start()
    {
        this.FindComponents();
    }

    private Vector3 GeneratePlatePosition()
    {
        float xOffset = Mathf.Abs(this.plateOffsetRange.x);
        float yOffset = Mathf.Abs(this.plateOffsetRange.y);
        Vector3 range = new(Random.Range(-xOffset, xOffset), Random.Range(-yOffset, yOffset), 0f);
        return this.transform.position + range;
    }

    private void SpawnPlates()
    {
        for (int i = 0; i < this.plateCount; i++)
        {
            var position = this.GeneratePlatePosition();
            GameObject plate = Instantiate(this.platePrefab, position, Quaternion.identity, this.transform.parent);
            plate.GetComponent<PlateLogic>().InitializeParameters(this.plateDamage, this.plateVelocity, this.plateDespawnDistance);
        }
    }

    private bool IsNearPlayer()
    {
        if (!this.patrolMover.moveAbovePlayer)
        {
            return false;
        }

        float distance = Mathf.Abs(this.transform.position.x - this.patrolMover.playerPos.x);

        return distance <= Mathf.Abs(this.spawnTriggerDistThresh);
    }

    public void Attack()
    {
        this.SpawnPlates();
    }

    public bool CanAttack()
    {
        return this.IsNearPlayer();
    }

    public void OnMovementComputed(Vector3 movement)
    {
        if (this.animations.IsInState("Flying"))
        {
            movement += this.animations.GetVerticalOffset();
        }
        this.transform.position = movement;
    }
}
