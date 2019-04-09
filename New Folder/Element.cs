using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using CJS;


public class Element : MonoBehaviour
{
    [System.Serializable]
    public class DamageEvent : UnityEvent { };
    [System.Serializable]
    public class ConnectEvent : UnityEvent<Element> { };

    //0,1,2
    public int type;
    public bool isFirst;
    public int initSpeed = 100;
    public float TotalHealth=100f;
    public int health;
    public int damage = 10;
    public int PlayerID = 0;
    public bool is_Connected;
    public bool is_Invincible;
    public GameObject ConnectVFX;
    public GameObject DamagedVFX;
    public Sprite[] ElementSprites;
    public ElementObject elementObject;
    public LayerMask layerMask;

    public DamageEvent OnDie;
    public ConnectEvent OnConnect;
    [HideInInspector]
    public List<Element> children;
    
    

    protected Rigidbody2D rigidbody2d;
    protected SpriteRenderer spriteRenderer;
    protected FixedJoint2D fixedJoint2D;
    
    //private ParticleSystem particleSystem;

    
   
    int m_scale;
    float timer;


    // Start is called before the first frame update

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        rigidbody2d = GetComponent<Rigidbody2D>();    
        if (!isFirst)
        {
            fixedJoint2D = GetComponent<FixedJoint2D>();
            //particleSystem = ConnectVFX.GetComponent<ParticleSystem>();
            fixedJoint2D.enabled = false;
            ElementInitialize();
        }      
    }

    public void ElementInitialize()
    {

        int randomType = Random.Range(0, ElementSprites.Length);
        int initialHealth = Random.Range(GameConst.MIN_HEALTH, GameConst.MAX_HEALTH);       
        health = initialHealth;
        is_Invincible = true;
        is_Connected = false;
        type = randomType;
        spriteRenderer.sprite = ElementSprites[type];
        PlayerID = -1;
        timer = 0;


        
        float speedX = Random.Range(-1f, 1f) * Random.Range(initSpeed / 2f, initSpeed);
        float speedY = Random.Range(-1f, 1f) * Random.Range(initSpeed / 2f, initSpeed);
        Vector2 velocity = new Vector2(speedX, speedY);
        rigidbody2d.AddForce(velocity);
    }

    public void initialnize(int health,int playID)
    {
        this.health = health;
        this.PlayerID = playID;
        
    }
    
    

    public void AddChildren(Element element)
    {
        children.Add(element);
    }
    


    private void Update()
    {
        if (is_Invincible)
        {
            timer += Time.deltaTime;
            if (timer >= GameConst.INVINCIBLE_TIME)
            {
                is_Invincible = false;
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {       
        if (is_Invincible)
        {
            return;
        }
        
        if ((1<<collision.gameObject.layer&layerMask.value)==0)
        {
            return;
        }
        
        Element other = collision.gameObject.GetComponent<Element>();             
        if (is_Connected && !other.is_Connected)
        {
            return;
        }
        //我没连它连，则连到父对象

        if (!is_Connected && other.is_Connected)
        {
            //不能photonView.RPC("ConnectToPlayer", RpcTarget.AllViaServer, other.PlayerID);          
            int playId = other.PlayerID;                                
            ConnectToPlayer(other, playId);                 
        }
        else if (is_Connected && other.is_Connected)
        {
            if (PlayerID != other.PlayerID)
            {

            }
                //TakeDamage(other);
        }



    }


    public void ConnectToPlayer(int playerID,int index)
    {
        print("RPC CAll"+playerID);
        
        Player player = GameManager.Instance.playList[playerID];
        Element element = player.GetElement(index);
       // player.AddElement(element,this);
        fixedJoint2D.connectedBody = element.rigidbody2d;
        fixedJoint2D.enabled = true;
        gameObject.tag = player.gameObject.tag;
        //rigidbody2d.Sleep();
        is_Connected = true;
        PlayerID = playerID;
        if (type == 0)
        {
            // Player player = other.parent.GetComponent<Player>();
            // player.AddCircle(element);
        }
    }

    public void ConnectToPlayer(Element element,int playerID)
    {
        if (OnConnect != null)
        {
            OnConnect.Invoke(this);
        }
       
        Player player = GameManager.Instance.playList[playerID];
        player.AddElement(this);
        fixedJoint2D.connectedBody = element.rigidbody2d;
        fixedJoint2D.enabled = true;
        gameObject.tag = element.gameObject.tag;
        //rigidbody2d.Sleep();
        is_Connected = true;
        PlayerID = playerID;
        Vector2 position = (element.transform.position + transform.position) / 2;
        //ConnectVFX.transform.position = position;
        //particleSystem.Play();


        if (type == 0)
        {
            // Player player = other.parent.GetComponent<Player>();
            // player.AddCircle(element);
        }
    }




    public void TakeDamage(bool isAttack,int amount)
    {
        Color color = spriteRenderer.color;
        if (!isAttack)
        {
            health -= amount;
            if (health <= 0)
            {
                if (!isFirst)
                {
                    if (OnDie != null)
                        OnDie.Invoke();
                }
                else
                {
                    EndGame();
                }
                 
            }
            color.a = health / TotalHealth;
            spriteRenderer.color = color;
        }
    }

    private void EndGame()
    {
        
    }

   
}
