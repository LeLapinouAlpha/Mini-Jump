using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    public Animation animator;

    public void PlayWalkAnimation()
    {
        animator.Play("EnemyRun");
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animation>();
    }
}
