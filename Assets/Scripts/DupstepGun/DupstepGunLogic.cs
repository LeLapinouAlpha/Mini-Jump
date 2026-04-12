using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DupstepGunLogic : MonoBehaviour
{
    //public float musicDuration;

    public float cooldown;
    public float distanceForDespawn;
    float counter;

    public Transform gunTransform;
    public AudioSource audioSource;
    InputAction shootAction;

    bool isEquipped;

    [Header("Projectile")]
    public GameObject spawningObject;
    public float vitesseHorizontale;
    public float vitesseVerticale;
    public float cooldownChangementColors;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.shootAction = InputSystem.actions.FindAction("Shoot");
    }

    // Update is called once per frame
    void Update()
    {
        if (this.isEquipped && this.shootAction.WasPressedThisFrame() && this.counter >= this.cooldown)
        {
            audioSource.mute = false;
            GameObject newGameObject = Instantiate(this.spawningObject, this.transform.position, this.transform.rotation, this.transform);
            newGameObject.GetComponent<ProjectileLogic>().vitesseHorizontale = this.vitesseHorizontale;
            newGameObject.GetComponent<ProjectileLogic>().vitesseVerticale = this.vitesseVerticale;
            newGameObject.GetComponent<ProjectileLogic>().distanceForDespawn = this.distanceForDespawn;
            newGameObject.GetComponent<ProjectileLogic>().cooldownChangementColors = this.cooldownChangementColors;

            this.counter = 0;
        }
        else
        {
            this.counter += Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isEquipped = true;
            Debug.Log("Dupstep Gun collided with the player");
            this.transform.SetParent(collision.transform);
            this.transform.position = gunTransform.position;
        }
    }
}
