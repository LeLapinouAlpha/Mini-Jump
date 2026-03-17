using UnityEngine;

public class CrossbowAnimations : MonoBehaviour
{
    public float reloadSpeed;
    float counter;
    [Header("Components references")]
    public Animator animator;

    public void PlayReloadAnimation(bool value)
    {
        this.animator.SetBool("IsReloading", value);
    }
    public void PlayShootingAnimation(bool value)
    {
        this.animator.SetBool("IsShooting", value);
    }


    void Start()
    {
        this.animator = this.GetComponent<Animator>();
    }
    void Update()
    {
        this.UpdateShootingAnimation();
       
    }
    void UpdateShootingAnimation()
    {
        if (this.animator != null)
        {
            if (this.counter >= this.reloadSpeed)
            {
                this.PlayShootingAnimation(this.counter >= this.reloadSpeed);
                this.counter = 0f;
            }
            else
            {
                this.counter += Time.deltaTime;
            }
        }
    }
}

