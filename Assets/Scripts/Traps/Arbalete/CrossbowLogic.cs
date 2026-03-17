using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CrossbowLogic : MonoBehaviour
{
    [Header("Arrow")]
    public GameObject spawningObject;

    public int childIndex;
    Vector3 spawningObjectPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (spawningObject == null)
        {
            spawningObject = this.transform.GetChild(childIndex).gameObject;
        }
        this.spawningObjectPosition = this.spawningObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
       
    }
   
    private void SpawnArrow()
    {
        GameObject newArrow = Instantiate(this.spawningObject, this.spawningObjectPosition, Quaternion.identity, this.transform);
        newArrow.SetActive(true);
    }




}
