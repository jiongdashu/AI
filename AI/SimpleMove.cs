using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMove : MonoBehaviour
{
    public float maxSpeed;
    // Start is called before the first frame update
    private Rigidbody2D rigidbody2D;


    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        rigidbody2D.velocity = Vector2.right * maxSpeed;
    }
}
