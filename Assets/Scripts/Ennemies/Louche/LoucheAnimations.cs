using UnityEngine;

public class LoucheAnimations : MonoBehaviour
{
    [Header("Components references")]
    public Animator animator;

    public void PlayWalkAnimation(bool value)
    {
        this.animator.SetBool("IsWalking", value);
    }

    void Start()
    {
        this.animator = this.GetComponent<Animator>();
    }
}
