using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CameraAnchorFollow : MonoBehaviour
{
    [Header("Components references")]
    public CameraFollow cameraFollow;
    public Transform initialTarget;
    public Transform target;

    void Start()
    {
        this.initialTarget = this.cameraFollow.objectToFollow;
        this.target = this.transform.GetChild(0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.cameraFollow != null)
        {
            this.cameraFollow.objectToFollow = this.target;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (this.cameraFollow != null)
        {
            this.cameraFollow.objectToFollow = this.initialTarget;
        }
    }
}
