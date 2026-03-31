using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(FlyingWardrobeAnimations))]
public class FlyingWardrobeLogic : MonoBehaviour
{
    public enum FacingDirection
    {
        Left,
        Right
    }

    [Header("Components References")]
    public FlyingWardrobeAnimations animations;

    [Header("Movements Parameters")]
    public float travellingDistance;
    public float velocity;
    public FacingDirection initialFacingDirection;
    private bool moveAbovePlayer;

    [Header("Attack Parameters")]
    public float attackSpeed;
    public int plateCount = 3;
    public Vector2 plateOffsetRange;
    public float spawnTriggerDistThresh = 0.5f;

    [Header("Plate The Plate")]
    public GameObject platePrefab;
    public float plateDamage;
    public float plateVelocity;
    public float plateDespawnDistance;

    [Header("States")]
    private float currentPositionX;
    private float initialPositionX;
    private bool canMove;
    private FacingDirection facingDirection;
    private float playerPosX;
    private float attackSpeedCounter;

    void Start()
    {
        this.initialPositionX = this.transform.position.x;
        this.currentPositionX = this.transform.position.x;

        this.canMove = true;

        this.animations = this.animations.GetComponent<FlyingWardrobeAnimations>();
        this.facingDirection = initialFacingDirection;
        this.moveAbovePlayer = false;
        this.attackSpeedCounter = this.attackSpeed;
    }

    void Update()
    {
        Move();
        FaceDirection();
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
            this.currentPositionX = Mathf.MoveTowards(this.currentPositionX, this.playerPosX, this.velocity * Time.deltaTime);
        }
        else if (this.facingDirection == FacingDirection.Right)
        {
            this.currentPositionX += this.velocity * Time.deltaTime;
        }
        else
        {
            this.currentPositionX -= this.velocity * Time.deltaTime;
        }

        var movement = new Vector3(this.currentPositionX, this.transform.position.y, this.transform.position.z);
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
            if (this.currentPositionX < this.playerPosX - 0.05f)
            {
                this.facingDirection = FacingDirection.Right;
            }
            else if (currentPositionX > playerPosX + 0.05f)
            {
                this.facingDirection = FacingDirection.Left;
            }
        }
        else
        {
            if (this.initialFacingDirection == FacingDirection.Right)
            {
                float targetX = this.initialPositionX + this.travellingDistance;
                if (this.currentPositionX < this.initialPositionX)
                {
                    this.facingDirection = FacingDirection.Right;
                }
                else if (currentPositionX > targetX)
                {
                    this.facingDirection = FacingDirection.Left;
                }
            }
            else
            {
                float targetX = this.initialPositionX - this.travellingDistance;
                if (this.currentPositionX > this.initialPositionX)
                {
                    this.facingDirection = FacingDirection.Left;
                }
                else if (this.currentPositionX < targetX)
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            this.moveAbovePlayer = true;
            this.playerPosX = collision.transform.position.x;

            if (this.attackSpeedCounter >= this.attackSpeed && this.IsNearPlayer())
            {
                this.SpawnPlates();

                this.attackSpeedCounter = 0f;
            }
            else
            {
                this.attackSpeedCounter += Time.deltaTime;
            }
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
        return Mathf.Abs(this.currentPositionX - this.playerPosX) <= Mathf.Abs(this.spawnTriggerDistThresh);
    }
}
