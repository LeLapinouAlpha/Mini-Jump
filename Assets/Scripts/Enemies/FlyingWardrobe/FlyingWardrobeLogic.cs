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
        if (currentPositionX < initialPositionX)
        {
            this.facingDirection = FacingDirection.Right;
        }
        else if (currentPositionX > initialPositionX + travellingDistance)
        {
            this.facingDirection = FacingDirection.Left;
        }
    }

    void FaceDirection()
    {
        if (this.facingDirection == FacingDirection.Right && !this.spriteRenderer.flipX)
        {
            this.spriteRenderer.flipX = true;
            this.collider2d.offset = new Vector2(this.initialCollider2dOffset.x, this.initialCollider2dOffset.y);
        }
        else if (this.facingDirection == FacingDirection.Left && this.spriteRenderer.flipX)
        {
            this.spriteRenderer.flipX = false;
            this.collider2d.offset = new Vector2(this.initialCollider2dOffset.x * (-1), this.initialCollider2dOffset.y);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canMove = false;
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

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canMove = true;
        }
    }
}
