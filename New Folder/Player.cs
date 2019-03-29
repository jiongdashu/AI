//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CJS {

    public class Player : MonoBehaviour
    {
        public int PlayerID;     
        public float energyCostSpeed;      
        public float speed;
        public float rotateSpeed=45;
        
        [Header("AI pram")]
        //public int scanSize;

        PlayerInput input;
        Rigidbody2D rigidBody;
        Camera main;

        
        
        private List<Element> elements = new List<Element>();
        private Stack<int> circleIndex = new Stack<int>();
        private int  circleIndexNow;

        

        private float moveHorizontal;
        private float moveVertical;
        private bool leftRotate;
        private bool rightRotate;

        float timer;
        // Start is called before the first frame update
        private void Awake()
        {
           
            input = GetComponent<PlayerInput>();
            rigidBody = GetComponent<Rigidbody2D>();
            
        }
        void Start()
        {
            
            circleIndex.Push(0);circleIndexNow = 0;                      
            PlayerID = GameManager.Instance.playerNum;
            if (PlayerID == 0)
            {             
              GameManager.Instance.SetCameraObject(gameObject.transform);
            }
                    
                
            Element circle = GetComponentInChildren<Element>();
            circle.initialnize(100, PlayerID);           
            elements.Add(circle);

            print(PlayerID + "is init");
            GameManager.Instance.RegistPlayer(PlayerID, this);
           

        }

        // Update is called once per frame
        void Update()
        {                             
            timer += Time.deltaTime;            
            ProcessInput();

        }

        

        void FixedUpdate()
        {
           
            Movement();
            Rotate();
        }

        private void ProcessInput()
        {
             leftRotate = Input.GetKey(KeyCode.J);
             rightRotate = Input.GetKey(KeyCode.K);
             moveHorizontal = Input.GetAxis("Horizontal");
             moveVertical = Input.GetAxis("Vertical");

        }
       

        public Element GetElement(int index)
        {
            return elements[index];
        }
        //thisElement:已经存在的element
        public void AddElement(Element element )
        {
            /*
            int index = elements.IndexOf(thisElement);
            if (index != -1)
            {
                elements[index].AddChildren(otherElement);
            }*/
            
            elements.Add(element);
            if (element.type == 0)
            {
                circleIndex.Push(elements.Count - 1);
                circleIndexNow = elements.Count - 1;               
            }
            

        }

        public void DeleteElement(Element element)
        {

            foreach(Element child in element.children)
            {
                child.PlayerID = 0;
                child.is_Connected = false;
                child.gameObject.transform.SetParent(null);
                elements.Remove(child);
                
            }
            elements.Remove(element);
            
        }
        private void Rotate()
        {
            
            if (leftRotate)

                rigidBody.AddTorque(rotateSpeed);
            else if (rightRotate)
                rigidBody.AddTorque(-rotateSpeed);
        }
        private void Movement()
        {
            Vector2 movement = new Vector2(moveHorizontal, moveVertical);
            //rigidBody.velocity = movement * speed;
            rigidBody.AddForce(movement * speed);
            if (moveHorizontal != 0 || moveVertical != 0)
            {
                if (timer > energyCostSpeed)
                {

                    timer = 0;
                    elements[circleIndexNow].TakeDamage(false, 1);
                    if (elements[circleIndexNow].health <= 0)
                    {
                        DeleteElement(elements[circleIndexNow]);
                        circleIndex.Pop();
                        circleIndexNow = circleIndex.Peek();

                    }

                }
            }

        }
     }
}




