
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentBehaviour : MonoBehaviour
{
    public GameObject target;
    public int maxPrediction=10;
    [Header("Random Run")]
    public float changeDirectionSpeed;
    [HideInInspector]
    public float nextChangeTime;

    [Header ("Arrive")] 
    public float targetRadius;
    public float slowRadius;
    public float timeToTarget;
    [HideInInspector]
    public bool isArrived;
    [Header("Leave")]
    public float escapeRadius;
    public float dangerRadius;
    public float timeToLeave;

    protected Agent agent;
    protected Steering steering;
    
    protected Rigidbody2D rigidbody2D;
    private Rigidbody2D targetRigidbody2D;
    private void Awake()
    {
        agent = GetComponent<Agent>();
        rigidbody2D = GetComponent<Rigidbody2D>();

        steering = new Steering(Vector2.zero,0);        
        Initialnize();
    }
    protected virtual void Update()
    {
        agent.SetSteering(GetSteering());
        
    }

    protected virtual void Initialnize()
    {

    }
    public void SetTarget(GameObject t)
    {
        this.target = t;
        this.targetRigidbody2D = t.GetComponent<Rigidbody2D>();
    }
    public void ResetTarget()
    {
        this.target = null;
    }
    public void SetTargetPosition(Vector2 position)
    {
        this.target.transform.position = position;
    }
    

    public virtual Steering GetSteering()
    {
        return steering;
    }
    public void SetNextChangeTime()
    {
        
        nextChangeTime = Time.time + changeDirectionSpeed;
    }
    public void RandomWalk()
    {
        
        if (Time.time >= nextChangeTime)
        {
            nextChangeTime = Time.time + changeDirectionSpeed;
            Vector2 direction = Random.insideUnitCircle;
            steering.accel = direction * agent.maxAccel;
        }
         

    }

    public void Seek(Vector2 targetPosition)
    {
        Vector2 direction = targetPosition - (Vector2)transform.position;
        direction.Normalize();
        steering.accel = direction * agent.maxAccel;
       
    }

    public Steering Flee(Vector2 targetPosition)
    {
        Vector2 direction = targetPosition - (Vector2)transform.position;
        direction.Normalize();
        steering.accel = direction * agent.maxAccel;
        return steering;
    }

    public void Pursue()
    {
      
        //1.得到距离目标的距离与目标当前速度与方向
        Vector3 direction = target.transform.position - transform.position;
        float distance = direction.magnitude;
        float speed = rigidbody2D.velocity.magnitude;

        //2.计算预测目标值（注意如果速度很小的情况下给一个最大估计值要不然会出现预测相当大的的情况）
        float prediction;
        if (speed <= distance / maxPrediction)
            prediction = maxPrediction;
        prediction = distance / speed;

        Vector2 predictTarget;
        predictTarget = (Vector2)target.transform.position + targetRigidbody2D.velocity * prediction;
        Seek(predictTarget);

    }

    public void Arrive()
    {
        if (!target)
        {
            return;
        }
        Vector3 direction = target.transform.position - transform.position;
        float distance = direction.magnitude;
        //1.确定速度
        float speed=0;
        if (distance < targetRadius)
        {
            steering.ResetSteering();
            isArrived = true;
        }
        else if (distance > slowRadius)
        {
            speed = agent.maxSpeed;
        }
        else
        {
            speed = agent.maxSpeed * distance / slowRadius;
        }
        //2.指定理想速度
        Vector2 desiredVelocity = direction;
        desiredVelocity.Normalize();
        desiredVelocity *= speed;

        //3.计算实际速度
        steering.accel = desiredVelocity - rigidbody2D.velocity;
        steering.accel /= timeToTarget;
        steering.accel = Vector2.ClampMagnitude(steering.accel, agent.maxAccel);

        

    }

    public void Leave()
    {
        Vector3 direction =   transform.position - target.transform.position;
        float distance = direction.magnitude;
        //1.确定速度
        float speed = 0;
        float reduce = 0;
        if (distance > dangerRadius)
        {
            steering.ResetSteering();

        }
        else if (distance <escapeRadius)
        {
            reduce = 0;
        }
        else
        {
            reduce = agent.maxSpeed * distance / dangerRadius;
        }
        speed = agent.maxSpeed - reduce;

        //2.指定理想速度
        Vector2 desiredVelocity = direction;
        desiredVelocity.Normalize();
        desiredVelocity *= speed;

        //3.计算实际速度
        steering.accel = desiredVelocity - rigidbody2D.velocity;
        steering.accel /= timeToTarget;
        steering.accel = Vector2.ClampMagnitude(steering.accel, agent.maxAccel);

    }



}
