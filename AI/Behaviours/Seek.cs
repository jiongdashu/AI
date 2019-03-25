using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : AgentBehaviour
{
    public override Steering GetSteering()
    {
        return   GetSteering(target.transform.position);
       
    }

    public  Steering GetSteering(Vector2 target)
    {
        Vector2 direction = target - (Vector2)transform.position;
        direction.Normalize();
        steering.accel = direction * agent.maxAccel;
        return steering;
    }
}
