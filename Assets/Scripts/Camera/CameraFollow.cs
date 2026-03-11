using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Components References")]
    public Transform objectToFollow;

    [Header("Follow Settings")]
    public bool followHorizontal = true;
    public bool followVertical = false;
    public float timeOffset = 0f;

    [Header("Pixel Snapping Settings")]
    public bool pixelSnappingEnabled = false;
    public float pixelsPerUnit = 16f;

    [Header("States")]
    public Vector3 followTargetPos = Vector3.zero;
    public Vector3 velocity = Vector3.zero;

    private void Follow()
    {
        if (this.objectToFollow != null)
        {
            // Follow the target position, and optionnaly ignore the horizontal and/or vertical axis
            this.followTargetPos.x = this.followHorizontal ? this.objectToFollow.position.x : this.transform.position.x;
            this.followTargetPos.y = this.followVertical ? this.objectToFollow.position.y : this.transform.position.y;

            // Smoothly move the camera
            var pos = Vector3.SmoothDamp(this.transform.position, this.followTargetPos, ref this.velocity, this.timeOffset);

            // Pixel snapping to avoid subpixel rendering
            if (this.pixelSnappingEnabled)
            {
                pos.x = Mathf.Round(pos.x * pixelsPerUnit) / pixelsPerUnit;
                pos.y = Mathf.Round(pos.y * pixelsPerUnit) / pixelsPerUnit;
            }

            this.transform.position = pos;
        }
    }

    void Start()
    {
        this.followTargetPos.z = this.transform.position.z;

        Follow();
    }

    void LateUpdate()
    {
        Follow();
    }
}
