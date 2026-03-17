using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CrossbowLogic : MonoBehaviour
{
    [Header("Arrow")]
    public GameObject spawningObject;

    Vector3 spawningObjectPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

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
