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

    public GameObject UIPrefab; // Missing
    public GameObject worldSpaceCanvasPlayerNam;
    public GameObject cameraPlayer; // Missing

    public GameObject multiplayerDeletable;
    public Vector3 playerToGoPos;

    public int playerID;
    public PhotonView photonID;
    public UIPlayer playerUI; // missing
    public PlayerManager playerManager;
    public CharacterController characterControl;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)// Can only have 1 OnPhotonSerializeView!
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position); //playerToGoPos = Vector3.Lerp(transform.position, playerOldPos, 0.1f);
            stream.SendNext(isAttacking);
            stream.SendNext(hp);
            stream.SendNext(playerID);

            stream.SendNext(playerManager.crPlayerName);
            stream.SendNext(playerManager.isReadyLobby);
            stream.SendNext(playerManager.isReadyToFight);
        }
        else if(stream.IsReading)
        {
            this.playerToGoPos = (Vector3)stream.ReceiveNext();
            this.isAttacking = (bool)stream.ReceiveNext();
            this.hp = (float)stream.ReceiveNext();
            this.playerID = (int)stream.ReceiveNext();

            playerManager.crPlayerName = (string)stream.ReceiveNext();
            playerManager.isReadyLobby = (bool)stream.ReceiveNext();
            playerManager.isReadyToFight = (bool)stream.ReceiveNext();
            print("recieved stream");
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
            if (rayCastAttackHit.transform != null)
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

    public void Awake()
    {

    }
    public void Start()
    {
        
    }

    void FixedUpdate()
    {
        if (photonID.IsMine && allowMoving)
        {
            if (hp <= 0)
            {
                GameLobbyManager.gameLobbyInfo.LeaveRoom();
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

}
