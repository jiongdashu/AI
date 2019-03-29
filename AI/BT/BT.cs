using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CJS.AI
{

    public enum BTState
    {
        Success,
        Failure,
        Continue,
        Abort
    }

    public static class BT
    {
        public static Root Root() { return new Root(); }
        public static Sequence Sequence() { return new Sequence(); }
        public static Selector Selector(bool shuffle = false) { return new Selector(); }
       // public static Action RunCoroutine(System.Func<IEnumerator<BTState>> coroutine) { return new Action(coroutine); }
        public static Action Call(System.Action fn) { return new Action(fn); }
        public static ConditionalBranch If(System.Func<bool> fn) { return new ConditionalBranch(fn); }
        public static While While(System.Func<bool> fn) { return new While(fn); }
        public static Condition Condition(System.Func<bool> fn) { return new Condition(fn); }
        public static Repeat Repeat(int count) { return new Repeat(count); }
        public static Wait Wait(float seconds) { return new Wait(seconds); }
        public static Trigger Trigger(Animator animator, string name, bool set = true) { return new Trigger(animator, name, set); }
        public static WaitForAnimatorState WaitForAnimatorState(Animator animator, string name, int layer = 0) { return new WaitForAnimatorState(animator, name, layer); }
        public static SetBool SetBool(Animator animator, string name, bool value) { return new SetBool(animator, name, value); }
        public static SetActive SetActive(GameObject gameObject, bool active) { return new SetActive(gameObject, active); }
        public static WaitForAnimatorSignal WaitForAnimatorSignal(Animator animator, string name, string state, int layer = 0) { return new WaitForAnimatorSignal(animator, name, state, layer); }
        public static Terminate Terminate() { return new Terminate(); }
        public static Log Log(string msg) { return new Log(msg); }
        public static RandomSequence RandomSequence(int[] weights = null) { return new RandomSequence(weights); }

    }
    public abstract class Behaviour
    {
        public abstract BTState Tick();

    }

    public abstract class Composite : Behaviour
    {
        protected int activeChild;
        protected List<Behaviour> children = new List<Behaviour>();

        public virtual Composite AddBehaviour(Behaviour behaviour)
        {
            children.Add(behaviour);
            return this;

        }
        public virtual Composite AddBehaviours(params Behaviour[] behaviours)
        {
            foreach(Behaviour behaviour in behaviours)
            {
                children.Add(behaviour);
            }          
            return this;
        }

        public virtual void ResetChildren()
        {
            activeChild = 0;
            foreach(Behaviour behaviour in children)
            {
                Composite c = (Composite)behaviour;
                if (c != null)
                {
                    c.ResetChildren();
                }
            }
        }
        

    }

    public class Sequence : Composite
    {
        public override BTState Tick()
        {
            var childState = children[activeChild].Tick();
            if (childState == BTState.Success)
            {
                activeChild++;
                if (activeChild == children.Count)
                {
                    activeChild = 0;
                    return BTState.Success;
                }
                return BTState.Continue;
            }else if(childState == BTState.Failure)
            {
                activeChild = 0;
                return BTState.Failure;
            }
            else
            {
                return childState;
            }
            //throw new System.Exception("This should never happen, but clearly it has.");
        }
    }

    public class Selector : Composite
    {
        public override BTState Tick()
        {
            var childState = children[activeChild].Tick();
            if (childState == BTState.Failure)
            {
                activeChild++;
                if (activeChild == children.Count)
                {
                    activeChild = 0;
                    return BTState.Failure;
                }
                return BTState.Continue;
            }
            else if (childState == BTState.Success)
            {
                activeChild = 0;
                return BTState.Success;
            }
            else
            {
                return childState;
            }
        }
    }

    public class Action : Behaviour
    {
        System.Action fn;
        //Func:带返回参数的delegate，返回类型为泛型参数<T>
        System.Func<IEnumerator<BTState>> coroutineFactory;
        //申明一个迭代器用于迭代执行协程
        IEnumerator<BTState> coroutine;
        public Action(System.Action fn)
        {
            this.fn = fn;
        }

        public override BTState Tick()
        {
            if (fn != null)
            {
                fn();
                return BTState.Success;
            }
            else
            {
                if (coroutine == null)
                {
                    coroutine = coroutineFactory();
                }
                if (!coroutine.MoveNext())
                {
                    coroutine = null;
                    return BTState.Success;
                }
                var result = coroutine.Current;
                if(result == BTState.Continue)
                {
                    return BTState.Continue;
                }
                else
                {
                    coroutine = null;
                    return result;
                }
            }

        }
    }

    public class Condition : Behaviour
    {
        System.Func<bool> fn;
        public Condition(System.Func<bool> fn)
        {
            this.fn = fn;

        }
        public override BTState Tick()
        {
            if (fn())
            {
                return BTState.Success;
            }
            else
            {
                return BTState.Failure;
            }
            throw new System.NotImplementedException();
        }
    }
   /*
    * 执行一个block的行为，执行完返回success，否则返回continue
   */
    public class Block : Composite
    {
        public override BTState Tick()
        {
            switch (children[activeChild].Tick())
            {
                case BTState.Continue:
                    return BTState.Continue;
                default:
                    activeChild++;
                    if (activeChild == children.Count)
                    {
                        activeChild = 0;
                        return BTState.Success;
                    }
                    return BTState.Continue;
            }
        }
    }
    //检测条件，如果测试通过则执行一系列行为，否则返回失败
    public class ConditionalBranch : Block
    {
        public System.Func<bool> fn;
        bool tested = false;
        public ConditionalBranch(System.Func<bool> fn)
        {
            this.fn = fn;
        }
        public override BTState Tick()
        {
            //检测测试条件
            if (!tested)
            {
                tested = fn();
            }
            if (tested)
            {
                var result = base.Tick();
                if (result == BTState.Continue)
                    return BTState.Continue;
                else
                {
                    tested = false;
                    return BTState.Success;
                }
            }
            else
            {
                return BTState.Failure;
            }
        }
    }
    //如果条件成功则不断执行一系列的行为直到条件失败
    public class While : Block
    {
        public System.Func<bool> fn;

        public While(System.Func<bool> fn)
        {
            this.fn = fn;
        }

        public override BTState Tick()
        {
            if (fn())
                base.Tick();
            else
            {
                //if we exit the loop
                ResetChildren();
                return BTState.Failure;
            }

            return BTState.Continue;
        }

        public override string ToString()
        {
            return "While : " + fn.Method.ToString();
        }
    }

    public class Root : Composite
    {
        public bool isTerminated = false;

        public override BTState Tick()
        {
            if (isTerminated) return BTState.Abort;
            while (true)
            {
                switch (children[activeChild].Tick())
                {
                    case BTState.Continue:
                        return BTState.Continue;
                    case BTState.Abort:
                        isTerminated = true;
                        return BTState.Abort;
                    default:
                        activeChild++;
                        if (activeChild == children.Count)
                        {
                            activeChild = 0;
                            return BTState.Success;
                        }
                        continue;
                }
            }
        }
    }

    public class Repeat : Block
    {
        public int count = 1;
        int currentCount = 0;
        public Repeat(int count)
        {
            this.count = count;
        }
        public override BTState Tick()
        {
            if (count > 0 && currentCount < count)
            {
                var result = base.Tick();
                switch (result)
                {
                    case BTState.Continue:
                        return BTState.Continue;
                    default:
                        currentCount++;
                        if (currentCount == count)
                        {
                            currentCount = 0;
                            return BTState.Success;
                        }
                        return BTState.Continue;
                }
            }
            return BTState.Success;
        }

    }

    /// <summary>
    /// Pause execution for a number of seconds.
    /// </summary>
    public class Wait : Behaviour
    {
        public float seconds = 0;
        float future = -1;
        public Wait(float seconds)
        {
            this.seconds = seconds;
        }

        public override BTState Tick()
        {
            if (future < 0)
                future = Time.time + seconds;

            if (Time.time >= future)
            {
                future = -1;
                return BTState.Success;
            }
            else
                return BTState.Continue;
        }

        public override string ToString()
        {
            return "Wait : " + (future - Time.time) + " / " + seconds;
        }
    }


    public class Trigger : Behaviour
    {
        Animator animator;
        int id;
        string triggerName;
        bool set = true;

        //if set == false, it reset the trigger istead of setting it.
        public Trigger(Animator animator, string name, bool set = true)
        {
            this.id = Animator.StringToHash(name);
            this.animator = animator;
            this.triggerName = name;
            this.set = set;
        }

        public override BTState Tick()
        {
            if (set)
                animator.SetTrigger(id);
            else
                animator.ResetTrigger(id);

            return BTState.Success;
        }

        public override string ToString()
        {
            return "Trigger : " + triggerName;
        }
    }

    /// <summary>
    /// Set a boolean on an animator.
    /// </summary>
    public class SetBool : Behaviour
    {
        Animator animator;
        int id;
        bool value;
        string triggerName;

        public SetBool(Animator animator, string name, bool value)
        {
            this.id = Animator.StringToHash(name);
            this.animator = animator;
            this.value = value;
            this.triggerName = name;
        }

        public override BTState Tick()
        {
            animator.SetBool(id, value);
            return BTState.Success;
        }

        public override string ToString()
        {
            return "SetBool : " + triggerName + " = " + value.ToString();
        }
    }


    /// <summary>
    /// Wait for an animator to reach a state.
    /// </summary>
    public class WaitForAnimatorState : Behaviour
    {
        Animator animator;
        int id;
        int layer;
        string stateName;

        public WaitForAnimatorState(Animator animator, string name, int layer = 0)
        {
            this.id = Animator.StringToHash(name);
            if (!animator.HasState(layer, this.id))
            {
                Debug.LogError("The animator does not have state: " + name);
            }
            this.animator = animator;
            this.layer = layer;
            this.stateName = name;
        }

        public override BTState Tick()
        {
            var state = animator.GetCurrentAnimatorStateInfo(layer);
            if (state.fullPathHash == this.id || state.shortNameHash == this.id)
                return BTState.Success;
            return BTState.Continue;
        }

        public override string ToString()
        {
            return "Wait For State : " + stateName;
        }
    }

    /// <summary>
    /// Set a gameobject active flag.
    /// </summary>
    public class SetActive : Behaviour
    {

        GameObject gameObject;
        bool active;

        public SetActive(GameObject gameObject, bool active)
        {
            this.gameObject = gameObject;
            this.active = active;
        }

        public override BTState Tick()
        {
            gameObject.SetActive(this.active);
            return BTState.Success;
        }

        public override string ToString()
        {
            return "Set Active : " + gameObject.name + " = " + active;
        }
    }

    public class WaitForAnimatorSignal : Behaviour
    {
        internal bool isSet = false;
        string name;
        int id;

        public WaitForAnimatorSignal(Animator animator, string name, string state, int layer = 0)
        {
            this.name = name;
            this.id = Animator.StringToHash(name);
            if (!animator.HasState(layer, this.id))
            {
                Debug.LogError("The animator does not have state: " + name);
            }
            else
            {
                //SendSignal.Register(animator, name, this);
            }
        }

        public override BTState Tick()
        {
            if (!isSet)
                return BTState.Continue;
            else
            {
                isSet = false;
                return BTState.Success;
            }

        }

        public override string ToString()
        {
            return "Wait For Animator Signal : " + name;
        }
    }


    public class Terminate : Behaviour
    {

        public override BTState Tick()
        {
            return BTState.Abort;
        }

    }

    public class Log : Behaviour
    {
        string msg;

        public Log(string msg)
        {
            this.msg = msg;
        }

        public override BTState Tick()
        {
            Debug.Log(msg);
            return BTState.Success;
        }
    }


    public class RandomSequence : Block
    {
        int[] m_Weight = null;
        int[] m_AddedWeight = null;

        /// <summary>
        /// Will select one random child everytime it get triggered again
        /// </summary>
        /// <param name="weight">Leave null so that all child node have the same weight. 
        /// If there is less weight than children, all subsequent child will have weight = 1</param>
        public RandomSequence(int[] weight = null)
        {
            activeChild = -1;

            m_Weight = weight;
        }

        public override Composite AddBehaviours(params Behaviour[] children)
        {
            m_AddedWeight = new int[children.Length];

            for (int i = 0; i < children.Length; ++i)
            {
                int weight = 0;
                int previousWeight = 0;

                if (m_Weight == null || m_Weight.Length <= i)
                {//if we don't have weight for that one, we set the weight to one
                    weight = 1;
                }
                else
                    weight = m_Weight[i];

                if (i > 0)
                    previousWeight = m_AddedWeight[i - 1];

                m_AddedWeight[i] = weight + previousWeight;
            }

            return base.AddBehaviours(children);
        }

        public override BTState Tick()
        {
            if (activeChild == -1)
                PickNewChild();

            var result = children[activeChild].Tick();

            switch (result)
            {
                case BTState.Continue:
                    return BTState.Continue;
                default:
                    PickNewChild();
                    return result;
            }
        }

        void PickNewChild()
        {
            int choice = Random.Range(0, m_AddedWeight[m_AddedWeight.Length - 1]);

            for (int i = 0; i < m_AddedWeight.Length; ++i)
            {
                if (choice - m_AddedWeight[i] <= 0)
                {
                    activeChild = i;
                    break;
                }
            }
        }

        public override string ToString()
        {
            return "Random Sequence : " + activeChild + "/" + children.Count;
        }
    }



   
}

