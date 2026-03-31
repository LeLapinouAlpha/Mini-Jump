using Assets.Scripts.Gameplay;
using Assets.Scripts.Utils;
using UnityEngine;

[RequireComponent(typeof(FlyingWardrobeAnimations)), RequireComponent(typeof(PeriodicAttack))]
public class FlyingWardrobeLogic : MonoBehaviour, IAttackCondition
{
    [Header("Components References")]
    public FlyingWardrobeAnimations animations;

    [Header("Movements Parameters")]
    public float travellingDistance;
    public float velocity;
    public FacingDirection initialFacingDirection;

    [Header("Attack Parameters")]
    public int plateCount = 3;
    public Vector2 plateOffsetRange;
    public float spawnTriggerDistThresh = 0.5f;

    [Header("Plate The Plate")]
    public GameObject platePrefab;
    public float plateDamage;
    public float plateVelocity;
    public float plateDespawnDistance;

    [Header("States")]
    private bool canMove;
    private Vector3 currentPosition;
    private Vector3 initialPosition;
    private FacingDirection facingDirection;
    private Vector3 playerPos;
    private bool moveAbovePlayer;

    private void FindComponents()
    {
        this.animations = this.animations.GetComponent<FlyingWardrobeAnimations>();
    }

    private void InitializeStates()
    {
        this.initialPosition = this.transform.position;
        this.currentPosition = this.initialPosition;
        this.canMove = true;
        this.facingDirection = this.initialFacingDirection;
        this.moveAbovePlayer = false;
    }

    void Start()
    {
        this.FindComponents();
        this.InitializeStates();
    }

    private Vector3 GeneratePlatePosition()
    {
        float xOffset = Mathf.Abs(this.plateOffsetRange.x);
        float yOffset = Mathf.Abs(this.plateOffsetRange.y);
        var range = new Vector3(UnityEngine.Random.Range(-xOffset, xOffset), UnityEngine.Random.Range(-yOffset, yOffset), 0f);
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
        return Mathf.Abs(this.currentPosition.x - this.playerPos.x) <= Mathf.Abs(this.spawnTriggerDistThresh);
    }

    private void Move()
    {
        if (!this.canMove)
        {
            return;
        }

        RightOrLeft();

        if (this.moveAbovePlayer)
        {
            this.currentPosition.x = Mathf.MoveTowards(this.currentPosition.x, this.playerPos.x, this.velocity * Time.deltaTime);
        }
        else if (this.facingDirection == FacingDirection.Right)
        {
            this.currentPosition.x += this.velocity * Time.deltaTime;
        }
        else
        {
            this.currentPosition.x -= this.velocity * Time.deltaTime;
        }

        var movement = new Vector3(this.currentPosition.x, this.transform.position.y, this.transform.position.z);
        if (this.animations.IsInState("Flying"))
        {
            movement += this.animations.GetVerticalOffset();
        }

        this.transform.position = movement;
    }

    private void RightOrLeft()
    {
        if (this.moveAbovePlayer)
        {
            // Update facing direction based on the player position relative to enemy
            if (this.currentPosition.x < this.playerPos.x - 0.05f)
            {
                this.facingDirection = FacingDirection.Right;
            }
            else if (currentPosition.x > playerPos.x + 0.05f)
            {
                this.facingDirection = FacingDirection.Left;
            }
        }
        else
        {
            if (this.initialFacingDirection == FacingDirection.Right)
            {
                float targetX = this.initialPosition.x + this.travellingDistance;
                if (this.currentPosition.x < this.initialPosition.x)
                {
                    this.facingDirection = FacingDirection.Right;
                }
                else if (currentPosition.x > targetX)
                {
                    this.facingDirection = FacingDirection.Left;
                }
            }
            else
            {
                float targetX = this.initialPosition.x - this.travellingDistance;
                if (this.currentPosition.x > this.initialPosition.x)
                {
                    this.facingDirection = FacingDirection.Left;
                }
                else if (this.currentPosition.x < targetX)
                {
                    this.facingDirection = FacingDirection.Right;
                }
            }
        }
    }

    private void FaceDirection()
    {
        Vector3 currentScale = this.transform.localScale;
        if (this.facingDirection == FacingDirection.Right && currentScale.x > 0)
        {
            currentScale.x = -Mathf.Abs(currentScale.x);
            this.transform.localScale = currentScale;
        }
        else if (this.facingDirection == FacingDirection.Left && currentScale.x < 0)
        {
            currentScale.x = Mathf.Abs(currentScale.x);
            this.transform.localScale = currentScale;
        }
    }

    void Update()
    {
        Move();
        FaceDirection();
    }

    public void Attack()
    {
        this.SpawnPlates();
    }

    public bool CanAttack()
    {
        return this.IsNearPlayer();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            this.moveAbovePlayer = true;
            this.playerPos.x = collision.transform.position.x;
        }
        else
        {
            this.moveAbovePlayer = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            this.canMove = true;
            this.moveAbovePlayer = false;
        }
    }
}
