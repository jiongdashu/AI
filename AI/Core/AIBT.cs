using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CJS;
using CJS.AI;
public class AIBT : MonoBehaviour
{
    public int ScanSize = 10;
    Root m_Ai = BT.Root();
    AgentBehaviour agentBehaviour;
    Player playerController;

    private Collider2D[] surrounds;
    private List<Player> surroundPlayers;
    private List<Element> surroundElements;
    private bool isFindPlayer;
    private bool isFindElement;
    LayerMask layerMask;
    private void OnEnable()
    {
        agentBehaviour = GetComponent<AgentBehaviour>();
        playerController = GetComponent<Player>();
        layerMask = LayerMask.NameToLayer("Elemnet");
        m_Ai.AddBehaviours(
            BT.Selector().AddBehaviours(
                 BT.Sequence().AddBehaviours(
                     BT.Condition(() => { return playerController.getCircleElementsNum() > 2; }),
                    //如果没有发现敌人和方块则自由移动
                     BT.Sequence().AddBehaviours(
                         //如果没有发现
                         BT.If(() => { return !isFindElement && !isFindPlayer; }).AddBehaviours(
                             //进行随机移动
                             BT.Call(agentBehaviour.RandomWalk),
                             //进行随搜
                             BT.Call(CheckForAround)
                             ),
                         //如果有发现
                         BT.If(() => { return isFindElement; }).AddBehaviours(
                             
                             BT.Call(agentBehaviour.Pursue)
                            
                             )

                         )
                     )
                )

        );
    }

    private void Update()
    {
        m_Ai.Tick();
    }


    public void CheckForAround()
    {
        surrounds = Physics2D.OverlapCircleAll(transform.position, ScanSize, layerMask);
        float dis = float.MaxValue;
        float distance;
        if (surrounds.Length >= 0)
        {
            foreach(Collider2D collider in surrounds)
            {
                Element element = collider.GetComponent<Element>();
                distance = (collider.transform.position - transform.position).magnitude;
                if (!element.is_Connected)
                {
                    print("find Element");
                    isFindElement = true;
                                   
                }
                else
                {
                    print("find Player");
                    isFindPlayer = false;
                }

                if (dis > distance)
                {
                    dis = distance;
                    agentBehaviour.SetTarget(collider.gameObject);
                }
            }
        }
        else
        {
            isFindElement = false;
            isFindPlayer = false;
            //agentBehaviour.ResetTarget();
        }

    }




}
