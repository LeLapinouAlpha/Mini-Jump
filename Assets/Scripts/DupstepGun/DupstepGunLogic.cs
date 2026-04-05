using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DupstepGunLogic : MonoBehaviour
{
    public float musicDuration;

    public float cooldown;
    float counter;

    public Transform gunTransform;

    [Header("Keybinds")]
    public InputAction shootAction;

    bool isEquipped;

    [Header("Projectile")]
    public GameObject spawningObject;
    public float vitesse;
    public float distanceForDespawn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.shootAction = InputSystem.actions.FindAction("Shoot");
    }

    // Update is called once per frame
    void Update()
    {
        if(isEquipped && this.shootAction.WasPressedThisFrame() && counter >= cooldown)
        {
            GameObject newGameObject = Instantiate(this.spawningObject, this.transform.position, Quaternion.identity, this.transform);
        }
        else
        {
            counter += Time.deltaTime;
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
