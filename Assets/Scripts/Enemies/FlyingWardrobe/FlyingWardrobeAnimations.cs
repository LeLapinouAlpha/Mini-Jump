using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FlyingWardrobeAnimations : MonoBehaviour
{
    [Header("Components references")]
    public Animator animator;

    [Header("Vertical Movement Settings")]
    public float verticalAmplitude = 0.1f; // How high the wardrobe moves up and down
    public float verticalFrequency = 5f; // How fast the wardrobe moves up and down


    public bool IsInState(string stateName)
    {
        return this.animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

    public Vector3 GetVerticalOffset()
    {
        float normalizedTime = this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        float yOffset = Mathf.Sin(normalizedTime * verticalFrequency * 2 * Mathf.PI) * verticalAmplitude;
        return new Vector3(0f, yOffset, 0f);
    }

    void Start()
    {
        this.animator = this.animator.GetComponent<Animator>();
    }

    void Update()
    {
    }
}
