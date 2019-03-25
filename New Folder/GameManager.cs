using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
using Cinemachine;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using ExitGames.Client.Photon;

using PlayerController = CJS.Player;

public class GameManager : MonoBehaviourPunCallbacks
{

    public static GameManager Instance= null;
    public int WorldSize = 10;
    public bool isNet = false;
    public GameObject Player;
    public Text InfoText;
    public Transform[] SpawnerPosition;
    public int RoomNum = 2;
    
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

    public override void OnEnable()
    {
        base.OnEnable();
        CountdownTimer.OnCountdownTimerHasExpired += OnCountdownTimerIsExpired;

    }

    private void Start()
    {
        InfoText.text = "Waiting For other Player";
        elementNum = 0;
        if (!isNet)
        {
            playerNum = 0;
            StartGame();
        }
        else
        {
            Hashtable props = new Hashtable
            {
                { GameConst.PLAYER_LOADED_LEVEL,true}
            };
            print("Game Manager Start()" + PhotonNetwork.LocalPlayer);
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();

        CountdownTimer.OnCountdownTimerHasExpired -= OnCountdownTimerIsExpired;
    }

    public void RegistPlayer(int playerId,PlayerController player)
    {
        playList.Add(playerId, player);
    }
    public void UnRegistPlayer(int playerId,PlayerController player)
    {
        playList.Remove(playerId);
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
            InfoText.text= string.Format("Player {0} won with {1} points.\n\n\nReturning to login screen in {2} seconds.", winner, score, timer.ToString("n2"));
            yield return new WaitForEndOfFrame();
            timer -= Time.deltaTime;
        }
        PhotonNetwork.LeaveRoom();
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("DemoAsteroids-LobbyScene");
    }

    

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        
    }

    public override void OnPlayerPropertiesUpdate(Player target, Hashtable changedProps)
    {
        /*
        if (changedProps.ContainsKey(AsteroidsGame.PLAYER_LIVES))
        {
            CheckEndOfGame();
            return;
        }*/

        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (changedProps.ContainsKey(GameConst.PLAYER_LOADED_LEVEL))
        {
            if (CheckAllPlayerLoadedLevel())
            {
                Hashtable props = new Hashtable
                    {
                        {CountdownTimer.CountdownStartTime, (float) PhotonNetwork.Time}
                    };
                PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            }
        }
    }

    private void StartGame()
    {
        //Vector2 position = Random.insideUnitCircle * WorldSize;

        int playerID;

        Vector2 position;
        


        if (isNet)
        {
            playerID = PhotonNetwork.LocalPlayer.ActorNumber;
            position = SpawnerPosition[playerID - 1].position;
            PhotonNetwork.Instantiate("Player", position, Quaternion.identity, 0);
        }
            
        else
        {
            
            for (int i = 0; i < RoomNum; i++)
            {
                position = SpawnerPosition[i].position;
                Instantiate(Player, position, Quaternion.identity);
                
            }
            
        }
            
        foreach(ElementSpawner spawner in spawners){
            spawner.StartSpawn();
        }


    }

    private bool CheckAllPlayerLoadedLevel()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object playerLoadedLevel;

            if (p.CustomProperties.TryGetValue(GameConst.PLAYER_LOADED_LEVEL, out playerLoadedLevel))
            {
                if ((bool)playerLoadedLevel)
                {
                    continue;
                }
            }

            return false;
        }

        return true;
    }
    private void OnCountdownTimerIsExpired()
    {
        StartGame();
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
