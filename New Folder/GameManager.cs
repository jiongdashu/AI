using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Cinemachine;
using CJS;

using PlayerController = CJS.Player;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance= null;
    public int WorldSize = 10;
    public bool isNet = false;
    public GameObject Player;
    public GameObject AI;
    public Transform[] SpawnerPosition;
    public int RoomNum = 1;
    
    [HideInInspector]
    public int playerNum;
    [HideInInspector]
    public int elementNum;
    public Dictionary<int,PlayerController> playList = new Dictionary<int,PlayerController>();

    
   
    ElementSpawner[] spawners;
    CinemachineVirtualCamera cinemachineVirtualCamera;

    private void Awake()
    {
        Instance = this;
        spawners = FindObjectsOfType<ElementSpawner>();
        cinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();

    }

    private void Start()
    {
       
        elementNum = 0;   
        playerNum = 0;
        StartGame();
       
    }

    public void RegistPlayer(int playerId,PlayerController player)
    {
        playList.Add(playerId, player);
        playerNum++;
    }
    public void UnRegistPlayer(int playerId,PlayerController player)
    {
        playList.Remove(playerId);
        playerNum--;
    }

    public void SetCameraObject(Transform target)
    {
        
        cinemachineVirtualCamera.Follow = target;
    }

    private IEnumerator EndOfGame(string winner,int score)
    {
        float timer = 5.0f;
        while (timer >= 0)
        {
            //InfoText.text= string.Format("Player {0} won with {1} points.\n\n\nReturning to login screen in {2} seconds.", winner, score, timer.ToString("n2"));
            yield return new WaitForEndOfFrame();
            timer -= Time.deltaTime;
        }
        
    }

    private void StartGame()
    {
        //Vector2 position = Random.insideUnitCircle * WorldSize;

        int playerID;
        Vector2 position;
        for (int i = 0; i < RoomNum; i++)
        {
            position = SpawnerPosition[i].position;
            //Instantiate(Player, position, Quaternion.identity);
            
            if (i > 0)
            {
                 Instantiate(AI, position, Quaternion.identity);                           
            }
            else
            {
                Instantiate(Player, position, Quaternion.identity);
            }
        }



        foreach (ElementSpawner spawner in spawners){
            spawner.StartSpawn();
        }


    }

    
}

public class GameConst
{
    public const float MIN_SPAWN_TIME = 5.0f;
    public const float MAX_SPAWN_TIME = 10.0f;
    public const int MIN_HEALTH = 50;
    public const int MAX_HEALTH = 100;
    public const float INVINCIBLE_TIME = 2;
    public const float PLAYER_RESPAWN_TIME = 4.0f;
    public const float ELEMENT_MAX_SPEED = 5.0f;
    public const bool PLAYER_IS_ALIVE = true;

    public const string PLAYER_LIVES = "PlayerLives";
    public const string PLAYER_READY = "IsPlayerReady";
    public const string PLAYER_LOADED_LEVEL = "PlayerLoadedLevel";


}
