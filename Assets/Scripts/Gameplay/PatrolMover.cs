using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.Events;

public class PatrolMover : MonoBehaviour
{
    [Header("Movements Parameters")]
    public bool canMove;
    public bool followPlayer;
    public float travellingDistance;
    public float velocity;
    public FacingDirection initialFacingDirection;

    [Header("Events")]
    public UnityEvent<Vector3> onMovementComputed;

    [Header("States")]
    private Vector3 currentPosition;
    private Vector3 initialPosition;
    private FacingDirection facingDirection;
    public Vector3 playerPos;
    public bool moveAbovePlayer;


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
        this.InitializeStates();
    }

    private void RightOrLeft()
    {
        if (this.moveAbovePlayer)
        {
            const float eps = 5e-2f;
            if (this.currentPosition.x < this.playerPos.x - eps)
            {
                this.facingDirection = FacingDirection.Right;
            }
            else if (currentPosition.x > playerPos.x + eps)
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

    private void ComputeMovement()
    {
        if (!this.canMove)
        {
            return;
        }

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
        this.onMovementComputed?.Invoke(movement);
        this.currentPosition = this.transform.position;
    }

    void Update()
    {
        this.RightOrLeft();
        this.FaceDirection();
        this.ComputeMovement();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (this.followPlayer && collision.CompareTag("Player"))
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
