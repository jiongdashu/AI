using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public float maxSpeed;
    public float maxAccel;

    private float rotation;
    private Vector2 velocity;
    private Rigidbody2D rigidbody2D;
    private Steering m_Steering;
    private Vector2 m_PreviousPosition;
    private Vector2 m_CurrentPosition;
    private Vector2 m_Movement;
    private void Awake()
    {
        velocity = Vector3.zero;
        rigidbody2D = GetComponent<Rigidbody2D>();
    }
    protected virtual void Update()
    {
        Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + (Vector3)m_Steering.accel, Color.red);
    }
    protected virtual void FixedUpdate()
    {
        ProcessMovement();
        ProcessRotation();
    }
    private void LateUpdate()
    {
        velocity += m_Steering.accel * Time.deltaTime;
        rotation += m_Steering.angular * Time.deltaTime;
        if (velocity.magnitude > maxSpeed)
        {
            velocity.Normalize();
            velocity = velocity * maxSpeed;
        }
        if (m_Steering.accel.magnitude == 0)
        {
            velocity = Vector2.zero;
        }
        if (m_Steering.angular == 0)
        {
            rotation = 0.0f;
        }

    }

    public void SetSteering(Steering steer)
    {
        m_Steering = steer;
    }

    private void ProcessMovement()
    {
        /*
        m_PreviousPosition = rigidbody2D.position;
        m_Movement = velocity * Time.deltaTime;
        m_CurrentPosition = m_PreviousPosition + m_Movement;
        */

        rigidbody2D.AddForce(m_Steering.accel);
        m_Movement = Vector2.zero;

    }
    private void ProcessRotation()
    {
        float rot = rigidbody2D.rotation + rotation*Time.deltaTime;
        rigidbody2D.MoveRotation(rot);

    }

}

public class Steering {
    public float angular;
    public Vector2 accel;

    public Steering(Vector2 accel,float angular)
    {
        this.accel = accel;
        this.angular = angular;
    }
    public void SetSteering(float a,Vector2 ac)
    {
        angular = a;
        accel = ac;

    }
    public void ResetSteering()
    {
        angular = 0.0f;
        accel = Vector2.zero;
    }
}
