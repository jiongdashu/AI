using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CJS.AI
{
    public class TraceState : FSMState
    {
        public FSMState PatrolSate;
        public FSMState FleeState;
        private AIBehaviour aiBehaviour;
        private bool isTatgetPlayer;
        private void Awake()
        {
            aiBehaviour = GetComponent<AIBehaviour>();


            FSMFnCondition fSMFnCondition = new FSMFnCondition();
            fSMFnCondition.fn = aiBehaviour.isNeedPatrol;
            FSMTransition transition = new FSMTransition();
            transition.condition = fSMFnCondition;
            transition.target = PatrolSate;
            /*
            FSMFnCondition fSMFnCondition1 = new FSMFnCondition();
            fSMFnCondition1.fn = AIBehaviour.isArrived;
            FSMFnCondition fSMFnCondition2 = new FSMFnCondition();
            fSMFnCondition2.fn = AIBehaviour.isTargetCapture;
            ConditionOr conditionOr = new ConditionOr();
            conditionOr.condition1 = fSMFnCondition1;
            conditionOr.condition2 = fSMFnCondition2;
            FSMTransition transition = new FSMTransition();
            transition.condition = conditionOr;
            transition.target = PatrolSate;*/
            transitions.Add(transition);

            

           /*
            fSMFnCondition.fn = AIBehaviour.isArrived;
            transition.condition = fSMFnCondition;
            transition.target = PatrolSate;
            transitions.Add(transition);
            */
        }
        
        public override void OnEnable()
        {
            print("FSM enter TraceState");
            aiBehaviour.SetTarget();
            base.OnEnable();
        }

        public override void Update()
        {
            aiBehaviour.TraceTarget();
        }
    }
}

