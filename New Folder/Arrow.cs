  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public bool isStatic;
    public bool isReference;

    private void Start()
    {
        if (isReference)
        {
            var shootScript = FindObjectOfType<ShootTrajectory>();
            shootScript.RegisterArrow(this);
        }
    }
    
}
