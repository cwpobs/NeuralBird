using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColumnController : MonoBehaviour
{
    [SerializeField] private Transform columnCentralPoint;
   
    void Start()
    {
        
    }

   
    void Update()
    {
        
    }

    public Vector2 GetCentralPointPosition()
    {
        return columnCentralPoint.transform.position;
    }
}
