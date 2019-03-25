using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;


public class ElementSpawner : MonoBehaviourPunCallbacks
{


    public GameObject[] Elements;
    public int elementAmout = 10;
    public int worldSize = 20;
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
    private void Start()
    {
       
        layerMask = LayerMask.NameToLayer("Element");
        isPlayerIn = false;
        /*
        for(int i=0;i<elements.Length;i++)
        {
            ElementPool pool = gameObject.AddComponent<ElementPool>();
            pool.initCount = 20;
            pool.element = elements[i];
            Pools[i] = pool;
        }*/
    }

    public void StartSpawn()
    {
        StartCoroutine(SpawnElement());
    }
    private IEnumerator SpawnElement()
    {
        string prep = "";
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(GameConst.MIN_SPAWN_TIME, GameConst.MAX_SPAWN_TIME));

            int type = Random.Range(0, Elements.Length);

            switch (type) {
                case 0:
                    prep = "Green";
                    break;
                case 1:
                    prep = "Red";
                    break;
                case 2:
                    prep = "Yellow";
                    break;
            }

            if (GameManager.Instance.elementNum <= elementAmout&&!isPlayerIn)
            {
                if (GameManager.Instance.isNet)
                    PhotonNetwork.InstantiateSceneObject(prep, transform.position, Quaternion.Euler(0, 0, Random.value * 360.0f), 0);
                else
                Instantiate(Elements[type], transform.position, Quaternion.Euler(0, 0, Random.value * 360.0f));
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

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(SpawnElement());
        }
    }
    /*
    private void OnTriggerStay2D(Collider2D collision)
    {
        print("On Trigger");

        if (!PhotonNetwork.IsMasterClient&&GameManager.Instance.isNet)
        {
            return;
        }
        if (collision.gameObject.layer == layerMask)
        {
            Rigidbody2D rigidbody = collision.GetComponent<Rigidbody2D>();
            if (rigidbody == null)
            {
                rigidbody = collision.GetComponentInParent<Rigidbody2D>();
            }
            Vector2 direction = rigidbody.position - (Vector2)gameObject.transform.position;
            direction.Normalize();
            float distance = direction.magnitude;
            Vector2 force = direction*(collider2D.radius - distance);
            print("force is " + (collider2D.radius - distance));
            print(rigidbody.velocity.magnitude);           
                rigidbody.AddForce(force);
            

        }
        
    }*/

    


}
