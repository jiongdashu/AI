using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestList : MonoBehaviour
{
    List<GameObject> objs = new List<GameObject>();
    public GameObject[] objects1;
    public GameObject[] objects2 = new GameObject[10];
    public List<GameObject> obj3;
    public List<GameObject> obj4 = new List<GameObject>();
    // Stapublic GameObject[] objects1;rt is called before the first frame update
    void Start()
    {
        objs.Add(gameObject);
        print(obj3);
        obj3.Add(gameObject);
        print(obj3);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
