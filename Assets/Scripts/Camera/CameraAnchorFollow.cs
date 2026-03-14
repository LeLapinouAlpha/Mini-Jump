using System;
using UnityEngine;

public class CameraAnchorFollow : MonoBehaviour
{
    [Header("Components references")]
    public CameraFollow cameraFollow;
    public Transform initialTarget;
    public Transform target;
    public Collider2D collider;
    private float initialCameraSize;
    public float cameraSize = 5;

    void Start()
    {
        this.initialTarget = this.cameraFollow.objectToFollow;
        this.target = this.transform.GetChild(0);
        this.collider = this.GetComponent<Collider2D>();
        this.initialCameraSize = this.cameraFollow.GetComponentInParent<Camera>().orthographicSize;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.cameraFollow != null)
        {
            this.cameraFollow.objectToFollow = this.target;
            this.cameraFollow.GetComponentInParent<Camera>().orthographicSize = this.cameraSize;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (this.cameraFollow != null)
        {
            this.cameraFollow.GetComponentInParent<Camera>().orthographicSize = this.initialCameraSize;
            this.cameraFollow.objectToFollow = this.initialTarget;
        }
    }
}
