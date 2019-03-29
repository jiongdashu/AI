using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CJS.AI
{


    public class FSM : MonoBehaviour
    {
       
    }


    public class FSMFnCondition : FSMCondition
    {
        public Func<bool> fn;

        public override bool Test()
        {
            if (fn != null)
            {
                return fn();
            }else
                return false;

        }
    }
    public class ConditionFloatBetween : FSMCondition
    {
        public float min;
        public float max;
        public float test;

        public override bool Test()
        {
            return test >= min && test <= max;
            
        }
    }

    public class ConditionAnd : FSMCondition
    {
        public FSMCondition condition1;
        public FSMCondition condition2;

        public override bool Test()
        {
            return condition1.Test() && condition2.Test();

        }
    }

    public class FSMCondition {
        
        public virtual bool Test()
        {
            return false;
        }

    }

    [Serializable]
    public class FSMTransition
    {
       public FSMCondition condition;
       public FSMState target;

    }

    
}
