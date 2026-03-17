using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CrossbowLogic : MonoBehaviour
{
    [Header("Arrow")]
    public GameObject spawningObject;
    public float vitesse;
    public float distanceForDespawn;
    public float damage;

    Vector3 spawningObjectPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.spawningObjectPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
       
    }
   
    private void SpawnArrow()
    {
        Instantiate(this.spawningObject, this.spawningObjectPosition, Quaternion.identity, this.transform);
    }




}
