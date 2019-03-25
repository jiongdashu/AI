using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[DefaultExecutionOrder(-100)]
public class PlayerInput : MonoBehaviour
{

    public float horizontal;
    public bool isMouseClick;

    bool readyToClear;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ClearInput();
        UpdateInputs();
        
    }
    //两个FixedUpdate之间可能有多个Update（）执行，为了不丢失输入信号，需要在两个FixedUpdate之间维持输入值
    void FixedUpdate()
    {
        //In FixedUpdate() we set a flag that lets inputs to be cleared out during the 
        //next Update(). This ensures that all code gets to use the current inputs
        readyToClear = true;
    }

    private void ClearInput()
    {
        if (!readyToClear)
        {
            return;
        }
        horizontal = 0;
        isMouseClick = false;

        readyToClear = false;

    }

    private void UpdateInputs()
    {
        horizontal = Input.GetAxis("Horizontal");
        isMouseClick = isMouseClick||Input.GetButtonDown("Fire1");

    }
}
