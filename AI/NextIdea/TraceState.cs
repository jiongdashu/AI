using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CJS.AI
{
    public class TraceState : FSMState
    {
        public FSMState PatrolSate;
        public FSMState FleeState;
        private AIBehaviour AIBehaviour;
        private bool isTatgetPlayer;
        private void Awake()
        {
            AIBehaviour = GetComponent<AIBehaviour>();

            FSMFnCondition fSMFnCondition = new FSMFnCondition();
            fSMFnCondition.fn = AIBehaviour.isNeedFlee;
            FSMTransition transition = new FSMTransition();
            transition.condition = fSMFnCondition;
            transition.target = FleeState;
            transitions.Add(transition);

           
            fSMFnCondition.fn = AIBehaviour.isArrived;
            transition.condition = fSMFnCondition;
            transition.target = PatrolSate;
            transitions.Add(transition);
        }
        
        public override void OnEnable()
        {
            print("FSM enter TraceState");
            isTatgetPlayer = AIBehaviour.isTargetPlayer();
            base.OnEnable();
        }

        public override void Update()
        {
            AIBehaviour.TraceTarget(false);
        }
    }
}

