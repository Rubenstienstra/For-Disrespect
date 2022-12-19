using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerMovement : MonoBehaviourPunCallbacks , IPunObservable
{
    public static PlayerMovement playerMovement;

    public int[] movementWASD;
    public int isTotalWalkingWASD;
    public bool holdingShift;
    public bool allowMoving;

    public RaycastHit rayCastAttackHit;
    public float rayCastDistanceAttack;
    public bool isAttacking;
    public float inputAttack;
    public float hp = 10;

    public float movementShiftBuff;
    private float crShiftBuff = 1;
    public float movementSpeedBuff;

    public Vector3 rayCastPos;
    public float rayCastDistance;
    public RaycastHit hitSlope;
    public float distanceBetweenGround;

    public Animator playerAnimations;
    public bool isRobot;

    public bool isHost;
    public bool isGuest;

    public string crPlayerName;
    public static GameObject thisPlayerPrefab;
    public GameObject UIPrefab; // Missing
    public GameObject worldSpaceCanvasPlayerName;
    public GameObject cameraPlayer; // Missing

    public GameObject multiplayerDeletable;
    public Vector3 playerToGoPos;

    public int playerID;
    public PhotonView photonID;
    public UIPlayer playerUI; // missing
    public GameLobbyManager crGameLobbyManager; //missing
    public CharacterController characterControl;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)// ?
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position); //playerToGoPos = Vector3.Lerp(transform.position, playerOldPos, 0.1f);
            stream.SendNext(isAttacking);
            stream.SendNext(hp);
            stream.SendNext(playerID);
            stream.SendNext(crPlayerName);
            //print("sended: ");
        }
        else if(stream.IsReading)
        {
            this.playerToGoPos = (Vector3)stream.ReceiveNext();
            this.isAttacking = (bool)stream.ReceiveNext();
            this.hp = (float)stream.ReceiveNext();
            this.playerID = (int)stream.ReceiveNext();
            this.crPlayerName = (string)stream.ReceiveNext();
            print("recieved: ");
        }
    }
    public void OnForward(InputValue value)
    {
        if (photonID.IsMine)
        {
            if (value.Get<float>() == 1)
            {
                movementWASD[0] = 1;
            }
            else
            {
                movementWASD[0] = 0;
            }
        }
    }
    public void OnLeft(InputValue value)
    {
        if (photonID.IsMine)
        {
            if (value.Get<float>() == 1)
            {
                movementWASD[1] = 1;
            }
            else
            {
                movementWASD[1] = 0;
            }
        }
    }
    public void OnDown(InputValue value)
    {
        if(photonID.IsMine)
        {
            if (value.Get<float>() == 1)
            {
                movementWASD[2] = 1;
            }
            else
            {
                movementWASD[2] = 0;
            }
        }
    }
    public void OnRight(InputValue value)
    {
        if (photonID.IsMine)
        {
            if (value.Get<float>() == 1)
            {
                movementWASD[3] = 1;
            }
            else
            {
                movementWASD[3] = 0;
            }
        }
    }
    public void OnShift(InputValue value)
    {
        if (photonID.IsMine)
        {
            if (value.Get<float>() == 1)
            {
                holdingShift = true;
                crShiftBuff = movementShiftBuff;
            }
            else
            {
                holdingShift = false;
                crShiftBuff = 1;
            }
        }
    }

    public void Awake()
    {
        if (photonID.IsMine)
        {
            thisPlayerPrefab = gameObject;
            crPlayerName = PhotonNetwork.NickName;

            worldSpaceCanvasPlayerName.SetActive(false);
        }
        else
        {
            for (int i = 0; i < multiplayerDeletable.transform.childCount; i++)
            {
                print("Destroyed: " + multiplayerDeletable.transform.GetChild(i).gameObject + "current for loop: " + i.ToString());
                multiplayerDeletable.transform.GetChild(i).gameObject.SetActive(false);
            }
            //if(crPlayerName != "")
            //{
            //      photonID.Owner.NickName;
            //}
        }
        //DontDestroyOnLoad(gameObject);
       
    }
    public void Start()
    {
        playerMovement = this;
        print("ViewID: "+ photonID.ViewID);

        if (photonID.IsMine)
        {
            if (crGameLobbyManager == null)
            {
                crGameLobbyManager = GameObject.Find("GameManager").GetComponent<GameLobbyManager>();
            }

            if (PhotonNetwork.IsMasterClient)
            {
                isHost = true;
                crGameLobbyManager.hostUI.SetActive(true);
                crGameLobbyManager.uiAnimation = crGameLobbyManager.hostUI.GetComponent<Animator>();
            }
            else
            {
                isGuest = true;
                crGameLobbyManager.guestUI.SetActive(true);
                crGameLobbyManager.uiAnimation = crGameLobbyManager.guestUI.GetComponent<Animator>();
            }
            crGameLobbyManager.uiAnimation.SetBool("BeforeCombat", true);

            crGameLobbyManager.camAnimation.SetBool("BeforeCombat", true);

        }
        crGameLobbyManager.RecalculatePlacementReadyUpRoom();
    }

    void FixedUpdate()
    {
        if (photonID.IsMine && allowMoving)
        {
            if (hp <= 0)
            {
                GameLobbyManager.gameLobbyInfo.LeaveRoom();
            }

            if (isRobot)
            {
                //ROBOT MOVEMENT
                if (movementWASD[2] > 0 || movementWASD[0] > 0 || playerAnimations.GetFloat("Speed") > 0.001f)
                {
                    playerAnimations.SetFloat("Speed", -movementWASD[2] + movementWASD[0], 0.25f, Time.deltaTime);
                }
                else if (playerAnimations.GetFloat("Speed") < 0.002f && movementWASD[2] == 0 && movementWASD[0] == 0)
                {
                    playerAnimations.SetFloat("Speed", 0);
                }

                if (movementWASD[1] > 0 || movementWASD[3] > 0 || playerAnimations.GetFloat("Direction") > 0.001)
                {
                    playerAnimations.SetFloat("Direction", -movementWASD[1] + movementWASD[3], 0.25f, Time.deltaTime);
                }
                else if (playerAnimations.GetFloat("Direction") < 0.002f && movementWASD[1] == 0 && movementWASD[3] == 0)
                {
                    playerAnimations.SetFloat("Direction", 0);
                }

                return;
            }

            //NOMRAL MOVEMENT
            Physics.Raycast(rayCastPos + transform.position, Vector3.down, out hitSlope, rayCastDistance); // maakt een rayccast aan die naar beneden toe gaat
            distanceBetweenGround = hitSlope.distance;
            if(hitSlope.distance < 0)
            {
                characterControl.Move(new Vector3(0,-1,0) * movementSpeedBuff * crShiftBuff * Time.deltaTime);
            }
            if (hitSlope.distance >= 0.001f)
            {
                characterControl.Move(new Vector3(-movementWASD[1] + movementWASD[3], 0, -movementWASD[2] + movementWASD[0]) * movementSpeedBuff * crShiftBuff * Time.deltaTime);
            }
            else
            {
                characterControl.Move(new Vector3(-movementWASD[1] + movementWASD[3], -1, -movementWASD[2] + movementWASD[0]) * movementSpeedBuff * crShiftBuff * Time.deltaTime);
            }

            for (int i = 0; i < movementWASD.Length; i++)//checking if player is moving
            {
                if (movementWASD[i] > 0)
                {
                    isTotalWalkingWASD++;
                }
            }
            isTotalWalkingWASD = 0; //resets the number
        } 
    }
    //lookAtAngle = Mathf.Atan2(addMovement.x, addMovement.z)* Mathf.Rad2Deg + playerCam.transform.eulerAngles.y; // berekent de angle waar je naar kijkt
    //endAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, lookAtAngle, ref velocity, timeToTurn); // hiermee berekent je de angle van de speler naar links of rechts toe via de camera

    public void OnAttack(InputValue value)
    {
        inputAttack = value.Get<float>();
        if (value.Get<float>() > 0)
        {
            if (!isAttacking && photonID.IsMine)
            {
                isAttacking = true;
                Attack();
            }
        }
    }

    public void Attack()
    {
        if (photonID.IsMine)
        {
            Physics.Raycast(transform.position, Vector3.down, out rayCastAttackHit, distanceBetweenGround);
            if(rayCastAttackHit.transform != null)
            {
                print("It has Found: " + rayCastAttackHit);
                if (rayCastAttackHit.transform.tag == "Player")
                {
                    rayCastAttackHit.transform.gameObject.GetComponent<PlayerMovement>().hp--;
                    rayCastAttackHit.transform.gameObject.GetComponent<UIPlayer>().OnHealthChange(hp);
                }
            } 
            isAttacking = false;
        }
    }

    public void LeaveRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LeaveRoom();
            return;
        }    
        SceneManager.LoadScene("Launcher");
    }
    void OnLevelWasLoaded(int level)
    {
        print(level);
    }
}
