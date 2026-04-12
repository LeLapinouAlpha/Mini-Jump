using System;
using UnityEngine;

public class DanceFloor : MonoBehaviour
{
    public float danceDuration;
    public float danceSpeed;
    float counter;
    float timeLived;
    int index;

    // Update is called once per frame
    void Update()
    {
        if (danceDuration <= timeLived)
        {
            Destroy(this.gameObject);
        }
        else
        {
            timeLived = Time.deltaTime;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ennemies"))
        {
            if (this.counter > this.danceSpeed)
            {
                this.counter = 0;
                switch (index)
                {
                    case 0:
                        collision.GetComponent<SpriteRenderer>().flipX = true;
                        index++;
                        break;
                    case 1:
                        collision.GetComponent<SpriteRenderer>().flipY = true;
                        index++;
                        break;
                    case 2:
                        collision.GetComponent<SpriteRenderer>().flipX = false;
                        index++;
                        break;
                    case 3:
                        collision.GetComponent<SpriteRenderer>().flipY = false;
                        this.index = 0;
                        break;
                }
            }
            else
            {
                this.counter += Time.deltaTime;
            }
        }
    }
}