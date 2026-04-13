using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Components References")]
    public Transform objectToFollow;

    [Header("Follow Settings")]
    public bool followHorizontal = true;
    public bool followVertical = false;
    public float timeOffset = 0f;
    public float verticalOffset = 0f;

    [Header("Pixel Snapping Settings")]
    public bool pixelSnappingEnabled = false;
    public float pixelsPerUnit = 16f;

    [Header("States")]
    public Vector3 followTargetPos = Vector3.zero;
    public Vector3 velocity = Vector3.zero;
    public Vector3 logicalPosition = Vector3.zero;


    private void Follow()
    {
        if (this.objectToFollow != null)
        {
            // Compute target
            this.followTargetPos.x = this.followHorizontal ? this.objectToFollow.position.x : this.logicalPosition.x;
            this.followTargetPos.y = this.followVertical ? this.objectToFollow.position.y : this.logicalPosition.y;
            this.followTargetPos.z = this.logicalPosition.z;

            // Smooth logical camera movement
            this.logicalPosition = Vector3.SmoothDamp(
                this.logicalPosition,
                this.followTargetPos,
                ref this.velocity,
                this.timeOffset
            );

            var pos = this.logicalPosition;
            pos.y += this.verticalOffset;

            // Pixel snapping only for rendering
            if (this.pixelSnappingEnabled)
            {
                pos.x = Mathf.Round(pos.x * this.pixelsPerUnit) / this.pixelsPerUnit;
                pos.y = Mathf.Round(pos.y * this.pixelsPerUnit) / this.pixelsPerUnit;
            }

            this.transform.position = pos;
        }
    }

    void Start()
    {
        this.logicalPosition = this.transform.position;
        this.followTargetPos = this.transform.position;

        Follow();
    }

    void LateUpdate()
    {
        Follow();
    }
}
