using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CJS
{
    public class ObjectPool<TPool,TObject> : MonoBehaviour 
        where TObject:PoolObject<TPool,TObject>, new()
        where TPool : ObjectPool<TPool, TObject>

    {
        public GameObject prefab;
        public int initialPoolCount = 10;

        public Queue<TObject> pool = new Queue<TObject>();

        protected virtual void Start()
        {
            for (int i = 0; i < initialPoolCount; i++)
            {
                print("make");
                TObject newPoolObject = CreatNewPoolObject();
                pool.Enqueue(newPoolObject);

            }
        }

        private TObject CreatNewPoolObject()
        {
            TObject newPoolObject = new TObject();
            newPoolObject.instance = Instantiate(prefab);
            newPoolObject.instance.transform.SetParent(transform);
            newPoolObject.SetReferences(this as TPool);
            newPoolObject.Sleep();
            return newPoolObject;
           
        }

        public virtual TObject Pop()
        {
            if (pool.Count != 0)
            {
                TObject obj = pool.Dequeue();
                obj.WakeUp();
                return obj;
            }
            TObject newPoolObject = CreatNewPoolObject();
            pool.Enqueue(newPoolObject);            
            newPoolObject.WakeUp();
            return newPoolObject;
        }
        public virtual void Push(TObject poolObject)
        {
            pool.Enqueue(poolObject);
            poolObject.Sleep();
        }

    }

    [System.Serializable]
    public abstract class PoolObject<TPool,TObject>
        where TObject : PoolObject<TPool, TObject>,new()
        where TPool:ObjectPool<TPool,TObject>
    {
        public GameObject instance;
        public TPool pool;

        public void SetReferences(TPool pool)
        {
            this.pool = pool;
            SetReferences();

        }
        public virtual void SetReferences()
        {

        }

        public virtual void WakeUp()
        {

        }
        public virtual void Sleep()
        {

        }
        public virtual void ReturnToPool()
        {
            pool.Push(this as TObject);
            
        }

    }
}





