using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CJS.AI
{
    public class FSMState : MonoBehaviour
    {
        
        public List<FSMTransition> transitions = new List<FSMTransition>();


        public virtual void OnEnable()
        {
            //state initialize
        }

        public virtual void OnDisable()
        {
            //state finalize
        }

        public virtual void Update()
        {
            //agent behaviour
        }

        private void LateUpdate()
        {
            foreach(FSMTransition t in transitions)
            {
                if (t.condition.Test())
                {
                    t.target.enabled = true;
                    this.enabled = false;
                    return;
                }
            }
        }
    }
}
