using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CJS.AI
{
    public class FSMStateH : FSMState
    {
        public List<FSMState> states;
        public FSMState stateEntry;
        protected FSMState stateCurrent;

        public override void OnEnable()
        {
            if (stateCurrent == null)
            {
                stateCurrent = stateEntry;
            }
            stateCurrent.enabled = true;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            stateCurrent.enabled = false;
            foreach(FSMState s in states)
            {
                s.enabled = false;
            }
        }

    }
}

