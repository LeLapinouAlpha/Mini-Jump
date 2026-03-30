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
    Collider2D collider2d;

    float currentPositionX;
    float initialPositionX;
    bool canMove;

    public float travellingDistance;
    public float vitesse;
    public FacingDirection facingDirection;
    private FacingDirection initialFacingDirection;
    private bool moveAbovePlayer;
    private float playerPosX;

    void Start()
    {
        initialPositionX = this.transform.position.x;
        currentPositionX = this.transform.position.x;

        //animationsScript = this.GetComponent<LoucheAnimations>();
        collider2d = this.GetComponent<Collider2D>();

        canMove = true;

        this.animations = this.animations.GetComponent<FlyingWardrobeAnimations>();
        this.initialFacingDirection = facingDirection;
        this.moveAbovePlayer = false;
    }

    void Update()
    {
        Move(canMove);
        FaceDirection();
    }

    void Move(bool canMove)
    {
        if (canMove)
        {
            //animationsScript.PlayWalkAnimation(true);
            RightOrLeft();

            if (this.moveAbovePlayer)
            {
                this.currentPositionX = Mathf.MoveTowards(this.currentPositionX, this.playerPosX, this.vitesse * Time.deltaTime);
            }
            else if (this.facingDirection == FacingDirection.Right)
            {
                this.currentPositionX += this.vitesse * Time.deltaTime;
            }
            else
            {
                this.currentPositionX -= this.vitesse * Time.deltaTime;
            }

            // Always update position, this ensures vertical animations continue playing
            Vector3 movement = new Vector3(this.currentPositionX, this.transform.position.y, this.transform.position.z);

            if (this.animations.IsInState("Flying"))
            {
                movement += this.animations.GetVerticalOffset();
            }

            this.transform.position = movement;
        }
    }

    void RightOrLeft()
    {
        if (this.moveAbovePlayer)
        {
            // Update facing direction based on the player position relative to enemy
            if (currentPositionX < playerPosX - 0.05f)
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
                float targetX = initialPositionX + travellingDistance;
                if (currentPositionX < initialPositionX)
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
                float targetX = initialPositionX - travellingDistance;
                if (currentPositionX > initialPositionX)
                {
                    this.facingDirection = FacingDirection.Left;
                }
                else if (currentPositionX < targetX)
                {
                    this.facingDirection = FacingDirection.Right;
                }
            }
        }
    }

    void FaceDirection()
    {
        Vector3 currentScale = transform.localScale;

        // Assuming right means a negative X scale (since flipX = true was used for Right)
        if (this.facingDirection == FacingDirection.Right && currentScale.x > 0)
        {
            currentScale.x = -Mathf.Abs(currentScale.x);
            transform.localScale = currentScale;
            // The collider offset is now automatically flipped by the localScale
        }
        else if (this.facingDirection == FacingDirection.Left && currentScale.x < 0)
        {
            currentScale.x = Mathf.Abs(currentScale.x);
            transform.localScale = currentScale;
            // The collider offset is now automatically flipped by the localScale
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Move above the player
            this.moveAbovePlayer = true;
            this.playerPosX = collision.transform.position.x;

            //canMove = false;
            //if (attackSpeedCounter >= attackSpeed)
            //{
            //    attackSpeedCounter = 0;
            //    this.spriteRenderer.flipX = collision.transform.position.x > this.transform.position.x;
            //    this.animationsScript.animator.Play("Attack");
            //}
            //else
            //{
            //    attackSpeedCounter += Time.deltaTime;
            //}
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
            canMove = true;
            this.moveAbovePlayer = false;
        }
    }
}
