using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CJS.AI
{
    public class PatrolState : FSMState
    {
        public FSMState TraceState;
        private AIBehaviour aiBehaviour ;

        private void Awake()
        {
            aiBehaviour = GetComponent<AIBehaviour>();

            FSMFnCondition fSMFnCondition = new FSMFnCondition();
            fSMFnCondition.fn = aiBehaviour.isFindTarget;
            FSMTransition transition = new FSMTransition();
            transition.condition = fSMFnCondition;
            transition.target = TraceState;

            transitions.Add(transition);
        }

        public override void OnEnable()
        {
            print("FSM enter PatrolState");
            
        }

        public override void Update()
        {
            aiBehaviour.RandomRun();
           
        }

        public override void OnDisable()
        {
            base.OnDisable();
        }
    }

}
