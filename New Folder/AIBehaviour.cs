using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CJS.AI;

public class AIBehaviour : MonoBehaviour
{
    public int scanSize = 10;
    public LayerMask layerMask;
    public Scanner scanner;
    private List<Element> elements = new List<Element>();
    private Stack<int> circleIndex = new Stack<int>();
    private int circleIndexNow;

    private Collider2D[] surrounds;
    AgentBehaviour agentBehaviour;   
    private Element targetElement;

    private void Awake()
    {
        //layerMask = LayerMask.NameToLayer("Element");
        agentBehaviour = GetComponent<AgentBehaviour>();
        scanner = GetComponentInChildren<Scanner>();
        scanner.TargetChange += TargetChanged;
    }

    private void TargetChanged()
    {
        SetTarget();
    }

    public int getElementsNum()
    {
        return elements.Count;
    }
    public int getCircleElementsNum()
    {
        return circleIndex.Count;
    }
    public void SetNextChangeTime()
    {       agentBehaviour.nextChangeTime = Time.deltaTime + agentBehaviour.changeDirectionSpeed;
       
    }
    public void RandomRun()
    {
        agentBehaviour.RandomWalk();
    }
    public void TraceTarget()
    {
        if (agentBehaviour.target != null)
        {
            agentBehaviour.Pursue();      
        }
          
    }

    public bool isNeedFlee()
    {
        return false;
    }

    public bool isArrived()
    {
        return agentBehaviour.isArrived;
    }
    public bool isFindTarget()
    {
        return scanner.IsFindElement || scanner.IsFindPlayer;
    }
    public bool isNeedPatrol()
    {
        return !scanner.IsFindElement && !scanner.IsFindPlayer;
    }

    public bool isTargetCapture()
    {
        return targetElement.is_Connected;       
    }

    public void SetTarget()
    {
        if (scanner.IsFindPlayer)
        {

        }
        if(scanner.IsFindElement)
        {
            Element taregt = scanner.GetBestTarget();
            agentBehaviour.SetTarget(taregt.gameObject);
        }
    }
   

    private void TargetConnected()
    {       
        agentBehaviour.ResetTarget();
    }
    /*
    public bool CheckForAround()
    {
        surrounds = Physics2D.OverlapCircleAll(transform.position, scanSize, layerMask);
        float dis = float.MaxValue;
        float distance;
        Element elementNear = null;
        Debug.DrawLine(transform.position, (Vector2)transform.position + Vector2.up * scanSize);
        if (surrounds.Length > 0)
        {
            print(surrounds[0]);
            foreach (Collider2D collider in surrounds)
            {
                print(collider);
                Element element = collider.GetComponent<Element>();
                distance = (collider.transform.position - transform.position).magnitude;
                if (dis > distance)
                {
                    dis = distance;
                    elementNear = element;
                    print(elementNear);
                }
            }
            if (elementNear)
            {
                agentBehaviour.SetTarget(elementNear.gameObject);
                targetElement = elementNear;
                if (!elementNear.is_Connected)
                {

                    isFindElement = true;
                    isFindPlayer = false;
                }
                else
                {

                    isFindPlayer = true;
                    isFindElement = false;
                }
                return true;
            }
            
            return false;
        }
        else
        {
            isFindElement = false;
            isFindPlayer = false;
            //agentBehaviour.ResetTarget();
            return false;
        }

    }*/
}
