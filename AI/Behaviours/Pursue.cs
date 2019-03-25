using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Pursue : Seek
{
    public float maxPrediction;

   
    private Vector2 currentTarget;
    private Vector2 nextTarget;
    private Rigidbody2D rigidbody2D;
    private Rigidbody2D targetRigid;

    protected override void Initialnize()
    {
        targetRigid = target.GetComponent<Rigidbody2D>();
        rigidbody2D = GetComponent<Rigidbody2D>();

    }
    public override Steering GetSteering()
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
        

        nextTarget =(Vector2)target.transform.position + targetRigid.velocity * prediction;
        return GetSteering(nextTarget);
        
    }
}
