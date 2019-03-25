using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementPool : MonoBehaviour
{

    public GameObject element;
    public int initCount = 10;

    private List<ElementObject> pool = new List<ElementObject>();

    private void Start()
    {
        for(int i = 0; i < initCount; i++)
        {
            ElementObject obj = CreateNewObject();
            pool.Add(obj);
        }
    }

    protected ElementObject CreateNewObject()
    {
        ElementObject newPoolObject = new ElementObject();
        newPoolObject.instance =Instantiate( element);
        newPoolObject.inPool = true;
        newPoolObject.instance.transform.SetParent(transform);
        newPoolObject.SetReferences();
        newPoolObject.Sleep();
        return newPoolObject;
    }

    public ElementObject Pop()
    {
        for(int i = 0; i < pool.Count; i++)
        {
            if (pool[i].inPool)
            {
                pool[i].inPool = false;
                pool[i].WakeUp(transform.position);
                return pool[i];
            }
        }

        ElementObject newPoolObject = new ElementObject();
        newPoolObject.instance = element;
        newPoolObject.inPool = false;
        newPoolObject.instance.transform.SetParent(transform);
        newPoolObject.SetReferences();
        pool.Add(newPoolObject);
        return newPoolObject;

    }

    public void Push(ElementObject element)
    {
        element.inPool = true;
        element.Sleep();
    }

}


public class ElementObject 
{

    public bool inPool;
    public GameObject instance;
    public Rigidbody2D rigidbody2D;
    public Element element;
    public ElementPool pool;

    public void SetReferences(ElementPool pool)
    {
        this.pool = pool;
        SetReferences();
    }
    public void SetReferences()
    {
        rigidbody2D = instance.AddComponent<Rigidbody2D>();
        rigidbody2D.gravityScale = 0f;
        element  = instance.GetComponent<Element>();
        element.elementObject = this;
    }
    public void WakeUp(Vector2 position)
    {
        instance.transform.position = position;
        
        instance.SetActive(true);
    }

    public void Sleep()
    {
        instance.SetActive(false);

    }

    public void ReturnToPool()
    {
        pool.Push(this);
        instance.AddComponent<Rigidbody2D>();
        instance.GetComponent<Rigidbody2D>().gravityScale = 0f;
    }

}
