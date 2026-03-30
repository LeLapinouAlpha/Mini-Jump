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
    SpriteRenderer spriteRenderer;
    Collider2D collider2d;
    Vector2 initialCollider2dOffset;

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
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        collider2d = this.GetComponent<Collider2D>();
        initialCollider2dOffset = collider2d.offset;

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

            if (this.facingDirection == FacingDirection.Right)
            {
                this.currentPositionX += this.vitesse * Time.deltaTime;
            }
            else
            {
                this.currentPositionX -= this.vitesse * Time.deltaTime;
            }

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
        if (this.initialFacingDirection == FacingDirection.Right)
        {
            float targetX = !this.moveAbovePlayer ? initialPositionX + travellingDistance : initialPositionX + Mathf.Abs(playerPosX - initialPositionX);
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
            float targetX = !this.moveAbovePlayer ? initialPositionX - travellingDistance : initialPositionX - Mathf.Abs(playerPosX - initialPositionX);
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
        }
    }
}
