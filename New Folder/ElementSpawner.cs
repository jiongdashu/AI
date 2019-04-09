using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CJS
{
    public class ElementSpawner : ObjectPool<ElementSpawner,ElementObject>
    {
        public int elementAmout = 10;
        public int worldSize = 20;
        public int removeDelay = 1;
        private float time;
   

        // Update is called once per frame

        private Dictionary<int, ElementPool> Pools = new Dictionary<int, ElementPool>();
        private CircleCollider2D collider2D;
        int layerMask;
        bool isPlayerIn;

        private void Awake()
        {
            collider2D = GetComponent<CircleCollider2D>();
        }
        override protected void Start()
        {
            base.Start();
            layerMask = LayerMask.NameToLayer("Element");
            isPlayerIn = false;
            
        }
       

        public void StartSpawn()
        {
            StartCoroutine(SpawnElement());
        }
        private IEnumerator SpawnElement()
        {
            
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(GameConst.MIN_SPAWN_TIME, GameConst.MAX_SPAWN_TIME));
                if (GameManager.Instance.elementNum <= elementAmout&&!isPlayerIn)
                {
                    Pop();
                    //Instantiate(Elements[type], transform.position, Quaternion.Euler(0, 0, Random.value * 360.0f));
                    GameManager.Instance.elementNum++;
                }
            

            }
        }

        private void Update()
        {
            Collider2D collider = Physics2D.OverlapCircle(transform.position, collider2D.radius);
            if (collider.tag.Equals("Player"))
            {
                isPlayerIn = true;
                ChangePosition();
                print("change positon");
            }
            isPlayerIn = false;


        }
    
        private void ChangePosition()
        {
           transform.position =  Random.insideUnitCircle* worldSize;

        }
    }

    public class ElementObject : PoolObject<ElementSpawner, ElementObject>
    {
        Element element;
        protected WaitForSeconds removedUntilTime;
        public override void SetReferences()
        {
            Debug.Log(instance);
            element = instance.GetComponent<Element>();
            Debug.Log(element);
            removedUntilTime = new WaitForSeconds(pool.removeDelay);
            element.OnDie.AddListener(ReturnToPoolEvent);
        }

        public override void WakeUp()
        {
            
            instance.SetActive(true);
            instance.transform.position = pool.transform.position;
            element.ElementInitialize();

        }
        public override void Sleep()
        {
            instance.SetActive(false);
            GameManager.Instance.elementNum--;
        }

        protected void ReturnToPoolEvent()
        {
            pool.StartCoroutine(ReturnToPoolAfterDelay());
        }


       
        protected IEnumerator ReturnToPoolAfterDelay()
        {
            yield return removedUntilTime;
            ReturnToPool();
            
        }

    }
}

