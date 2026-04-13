using UnityEngine;

public class MovingPlateform : MonoBehaviour
{


    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 0.1f;

    private Vector3 nextPosition;

    // Start is called before the first frame update
    private void Start()
    {
        nextPosition = pointB.position;
        transform.position = nextPosition;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, nextPosition, moveSpeed * Time.deltaTime);

        if (transform.position == pointA.position)
        {
            nextPosition = pointB.position;
        }
        else if (transform.position == pointB.position)
        {
            nextPosition = pointA.position;
        }

    }
}

