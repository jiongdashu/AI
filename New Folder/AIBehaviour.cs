using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBehaviour : MonoBehaviour
{
    public int scanSize = 10;
    private List<Element> elements = new List<Element>();
    private Stack<int> circleIndex = new Stack<int>();
    private int circleIndexNow;

    private Collider2D[] surrounds;
    AgentBehaviour agentBehaviour;
    LayerMask layerMask;
    private bool isFindPlayer;
    private bool isFindElement;

    public int getElementsNum()
    {
        return elements.Count;
    }
    public int getCircleElementsNum()
    {
        return circleIndex.Count;
    }

    public void TraceTarget(bool isRandom)
    {
        if (agentBehaviour.isArrived && isRandom)
        {
            Vector2 target;
            target = Random.insideUnitCircle * scanSize;
            agentBehaviour.SetTargetPosition(target);
            agentBehaviour.isArrived = false;
        }
        agentBehaviour.Arrive();
    }

    public bool isNeedFlee()
    {
        return false;
    }

    public bool isArrived()
    {
        return agentBehaviour.isArrived;
    }

    public bool isTargetPlayer()
    {
        return isFindPlayer;
    }

    public bool CheckForAround()
    {

        surrounds = Physics2D.OverlapCircleAll(transform.position, scanSize, layerMask);
        float dis = float.MaxValue;
        float distance;
        Element elementNear = null;
        print("Scan" + surrounds.Length);
        if (surrounds.Length > 0)
        {
            print("find");
            foreach (Collider2D collider in surrounds)
            {
                Element element = collider.GetComponent<Element>();
                distance = (collider.transform.position - transform.position).magnitude;


                if (dis > distance)
                {
                    dis = distance;
                    elementNear = element;
                    //agentBehaviour.SetTarget(collider.gameObject);
                }
            }
            if (!elementNear.is_Connected)
            {
                print("find Element");
                isFindElement = true;
                isFindPlayer = false;
            }
            else
            {
                print("find Player");
                isFindPlayer = true;
                isFindElement = false;
            }
            return true;
        }
        else
        {
            isFindElement = false;
            isFindPlayer = false;
            //agentBehaviour.ResetTarget();
            return false;
        }

    }
}
