using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
namespace CJS {

    public class Player : MonoBehaviour
    {
        public int PlayerID;
        public GameObject pref;
        public float fireRate;      
        public float speed;
        public float rotateSpeed=45;
        [HideInInspector]
        public bool isAI = true;
        PlayerInput input;
        Rigidbody2D rigidBody;
        Camera main;

        private PhotonView photonView;
        private float zDisplacement;
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
            photonView = GetComponent<PhotonView>();
            input = GetComponent<PlayerInput>();
            rigidBody = GetComponent<Rigidbody2D>();
            
        }
        void Start()
        {
            
            
            
            circleIndex.Push(0);circleIndexNow = 0;
            main = Camera.main;
            zDisplacement = -main.transform.position.z;
            if (GameManager.Instance.isNet)
            {
                PlayerID = photonView.Owner.ActorNumber;
            }
            else
            {
                PlayerID = GameManager.Instance.playerNum;
                if (PlayerID == 0)
                {
                    isAI = false;
                    GameManager.Instance.SetCameraObject(gameObject.transform);
                }
                    
                GameManager.Instance.playerNum++;
            }
            Element circle = GetComponentInChildren<Element>();
            circle.initialnize(100, PlayerID);           
            elements.Add(circle);

            print(PlayerID + "is init");
            GameManager.Instance.RegistPlayer(PlayerID, this);
            if (photonView.IsMine)
            {
                GameManager.Instance.SetCameraObject(gameObject.transform);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!photonView.IsMine&&GameManager.Instance.isNet)
                return;                        
            timer += Time.deltaTime;
            if (!isAI)
                ProcessInput();

        }

        

        void FixedUpdate()
        {
            if (!photonView.IsMine && GameManager.Instance.isNet)
                return;
           
            Movement();
            Rotate();
        }


        #region AIBehaviour

        public int getElementsNum()
        {
            return elements.Count;
        }
        public int getCircleElementsNum()
        {
            return circleIndex.Count;
        }


        #endregion




        private void ProcessInput()
        {
             leftRotate = Input.GetKey(KeyCode.J);
             rightRotate = Input.GetKey(KeyCode.K);
             moveHorizontal = Input.GetAxis("Horizontal");
             moveVertical = Input.GetAxis("Vertical");

        }
        /*
        public void AddCircle(Element circle)
        {
            circles.Add(circle);
            circleNow = circles[circles.Count-1];
            print("ADD a new circle!!!!");

        }*/

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
            print("type"+element.type);
            if (element.type == 0)
            {
                circleIndex.Push(elements.Count - 1);
                circleIndexNow = elements.Count - 1;
                print("circleIndexNow" + circleIndexNow);
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
            if (photonView.IsMine && GameManager.Instance.isNet)
                PhotonNetwork.Destroy(element.gameObject);
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
            Vector2 movement = new Vector2(moveHorizontal,moveVertical);
            //rigidBody.velocity = movement * speed;
            rigidBody.AddForce(movement * speed);
            if (moveHorizontal != 0 || moveVertical != 0)
            {
                if (timer > 1 / fireRate)
                {
                    
                    timer = 0;
                    elements[circleIndexNow].TakeDamage(false, 1);
                    if (elements[circleIndexNow].health<= 0)
                    {
                        DeleteElement(elements[circleIndexNow]);
                        circleIndex.Pop();
                        circleIndexNow = circleIndex.Peek();
                        
                    }
                    
                }
            }
        
            /*
            if (input.isMouseClick)
            {
                if (timer > 1 / fireRate)
                {
                    fire();
                    timer = 0;
                    amount -= Mathf.RoundToInt(amount/10);
                    print(amount / initialAmount);
                    transform.localScale = new Vector3( amount/initialAmount, amount / initialAmount, 1);


                }
            
            }*/
        }

        /*
        private void fire()
        {
            Vector3 mousePos = MouseInWorldCoords();
            GameObject instance = Instantiate(pref, transform.position,transform.rotation) as GameObject;
           // Element element = instance.GetComponent<Element>();

            Rigidbody2D rigid = instance.GetComponent<Rigidbody2D>();
            // element.rigidbody.velocity = mousePos - transform.position;
            rigid.velocity = mousePos - transform.position;

            
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit backHit;
            if(Physics2D.Raycast())
            

        }*/

        
        private Vector3 MouseInWorldCoords()
        {
            var screenMousePos = Input.mousePosition;
            //Debug.Log(screenMousePos);
            screenMousePos.z = zDisplacement;
            return Camera.main.ScreenToWorldPoint(screenMousePos);
        }
    }
}




