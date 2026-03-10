using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Components references")]
    public Transform objectToFollow;

    [Header("Settings")]
    public bool followHorizontal = true;
    public bool followVertical = false;
    public float timeOffset = 0f;

    [Header("States")]
    public Vector3 followTargetPos = Vector3.zero;
    public Vector3 velocity = Vector3.zero;

    private void Follow()
    {
        if (this.objectToFollow != null)
        {
            this.followTargetPos.x = this.followHorizontal ? this.objectToFollow.position.x : this.transform.position.x;
            this.followTargetPos.y = this.followVertical ? this.objectToFollow.position.y : this.transform.position.y;

            this.transform.position = Vector3.SmoothDamp(this.transform.position, this.followTargetPos, ref this.velocity, this.timeOffset);
        }
    }

    void Start()
    {
        this.followTargetPos.z = this.transform.position.z;

        Follow();
    }

    void Update()
    {
        Follow();
    }
}
