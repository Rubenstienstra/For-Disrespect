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
    public float[] movementWASD;
    public int isTotalWalkingWASD;
    public bool holdingShift;
    public bool allowMoving;

    public RaycastHit rayCastAttackHit;
    public float rayCastDistanceAttack;
    public bool isAttacking;
    public bool isBlocking; //Blocking is niet een bool. het is een trigger van wanneer je wordt geraakt.

    public bool hasOpenedESC;

    public Vector2 mouseXYInput;
    public Vector2 rotationXYSpeed;

    public float movementShiftBuff;
    private float crShiftBuff = 1;
    public float movementSpeedBuff;

    public Vector3 rayCastPos;
    public float rayCastDistance;
    public RaycastHit hitSlope;
    public float distanceBetweenGround;

    public GameObject worldSpaceCanvasPlayerNam;

    public GameObject multiplayerDeletable;

    public int playerID;
    public PhotonView photonID;
    public UIPlayer playerUI;
    public PlayerManager playerManager;
    public CharacterController characterControl;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)// Can only have 1 OnPhotonSerializeView!
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(allowMoving);
            stream.SendNext(isAttacking);
            stream.SendNext(playerID);
            stream.SendNext(isBlocking);

            stream.SendNext(playerManager.crPlayerName);
            stream.SendNext(playerManager.isHost);
            stream.SendNext(playerManager.isGuest);
            stream.SendNext(playerManager.isReadyLobby);
            stream.SendNext(playerManager.isReadyToFight);
            stream.SendNext(playerManager.hp);
            stream.SendNext(playerManager.stamina);
        }
        else if(stream.IsReading)
        {
            this.transform.position = (Vector3)stream.ReceiveNext();
            this.allowMoving = (bool)stream.ReceiveNext();
            this.isAttacking = (bool)stream.ReceiveNext();
            this.playerID = (int)stream.ReceiveNext();
            this.isBlocking = (bool)stream.ReceiveNext();

            playerManager.crPlayerName = (string)stream.ReceiveNext();
            playerManager.isHost = (bool)stream.ReceiveNext();
            playerManager.isGuest = (bool)stream.ReceiveNext();
            playerManager.isReadyLobby = (bool)stream.ReceiveNext();
            playerManager.isReadyToFight = (bool)stream.ReceiveNext();
            playerManager.hp = (int)stream.ReceiveNext();
            playerManager.stamina = (float)stream.ReceiveNext();
            print("recieved stream");
        }
    }
    #region InputActions
    public void OnEsc(InputValue value)
    {
        if (photonID.IsMine)
        {
            if(!hasOpenedESC)
            {
                hasOpenedESC = true;
                playerManager.playerESCMenu.SetActive(true);
                return;
            }
            hasOpenedESC = false;
            playerManager.playerESCMenu.SetActive(false);
            return;
        }
    }
    public void OnForward(InputValue value)
    {
        if (photonID.IsMine)
        {
            if (value.Get<float>() == 1)
            {
                movementWASD[0] = value.Get<float>();
            }
            else
            {
                movementWASD[0] = 0;
            }
            playerManager.playerAnimations.SetFloat("Vertical", -movementWASD[2] + movementWASD[0]);
        }
    }
    public void OnLeft(InputValue value)
    {
        if (photonID.IsMine)
        {
            if (value.Get<float>() == 1)
            {
                movementWASD[1] = value.Get<float>();
            }
            else
            {
                movementWASD[1] = 0;
            }
            playerManager.playerAnimations.SetFloat("Horizontal", -movementWASD[1] + movementWASD[3]);
        }
    }
    public void OnDown(InputValue value)
    {
        if(photonID.IsMine)
        {
            if (value.Get<float>() == 1)
            {
                movementWASD[2] = value.Get<float>();
            }
            else
            {
                movementWASD[2] = 0;
            }
            playerManager.playerAnimations.SetFloat("Vertical", -movementWASD[2] + movementWASD[0]);
        }
    }
    public void OnRight(InputValue value)
    {
        if (photonID.IsMine)
        {
            if (value.Get<float>() == 1)
            {
                movementWASD[3] = value.Get<float>();
            }
            else
            {
                movementWASD[3] = 0;
            }
            playerManager.playerAnimations.SetFloat("Horizontal", -movementWASD[1] + movementWASD[3]);
        }
    }
    public void OnShift(InputValue value)
    {
        if (photonID.IsMine)
        {
            if (value.Get<float>() == 1)
            {
                holdingShift = true;
                playerManager.playerAnimations.SetBool("Running", true);

                crShiftBuff = movementShiftBuff;
            }
            else
            {
                holdingShift = false;
                playerManager.playerAnimations.SetBool("Running", true);

                crShiftBuff = 1;
            }
        }
    }
    public void OnMouseXY(InputValue value)
    {
        if (allowMoving && photonID.IsMine && !hasOpenedESC)
        {
            mouseXYInput = value.Get<Vector2>();

            transform.Rotate(0, mouseXYInput.x * rotationXYSpeed.x * Time.deltaTime, 0);
        }
    }
    public void OnAttack(InputValue value)
    {
        if (photonID.IsMine)
        {
            if (value.Get<float>() == 1)
            {
                if (!isAttacking && !isBlocking && !hasOpenedESC)
                {
                    isAttacking = true;
                    playerManager.stamina -= playerManager.staminaCostAttack;
                    Attack();
                }
            }
        }
    }
    public void OnBlock(InputValue value)
    {
        if (photonID.IsMine)
        {
            if (value.Get<float>() == 1)
            {
                if (!isBlocking && !isAttacking && !hasOpenedESC)
                {
                    isBlocking = true;
                }
            }
            else
            {
                isBlocking = false;
            }
        }
    }
    #endregion

    public void Attack()
    {
        if (photonID.IsMine)
        {
            for (int i = 0; i < playerManager.playersInAttackRange.Count; i++)
            {
                if(playerManager.playersInAttackRange[i].name == "Player (Clone)")
                {
                    if (playerManager.playersInAttackRange[i].gameObject.GetComponent<PlayerMovement>().isBlocking)
                    {
                        playerManager.DealtBlockedDamage(playerManager.playersInAttackRange[i]);
                    }
                    else
                    {
                        playerManager.SuccesfullyDealtDamage(playerManager.playersInAttackRange[i]);
                    }
                }
            }
            isAttacking = false;
        }
    }
    public void Start()
    {

    }

    void FixedUpdate()
    {
        if (photonID.IsMine && allowMoving)
        {
            #region MOVEMENT

            Physics.Raycast(rayCastPos + transform.position, Vector3.down, out hitSlope, rayCastDistance); // maakt een rayccast aan die naar beneden toe gaat
            distanceBetweenGround = hitSlope.distance;

            if (hitSlope.distance < 0)
            {
                characterControl.Move(new Vector3(0, -1, 0) * movementSpeedBuff * crShiftBuff * Time.deltaTime);
            }
            if (hitSlope.distance >= 0.001f)
            {
                transform.Translate(new Vector3(-movementWASD[1] + movementWASD[3], 0, -movementWASD[2] + movementWASD[0]) * movementSpeedBuff * crShiftBuff * Time.deltaTime);
            }
            else
            {
                transform.Translate(new Vector3(-movementWASD[1] + movementWASD[3], -1, -movementWASD[2] + movementWASD[0]) * movementSpeedBuff * crShiftBuff * Time.deltaTime);
            }

            //characterControl.Move(new Vector3(-movementWASD[1] + movementWASD[3], 0, -movementWASD[2] + movementWASD[0]) * movementSpeedBuff * crShiftBuff * Time.deltaTime);


            for (int i = 0; i < movementWASD.Length; i++)//checking if player is moving
            {
                if (movementWASD[i] > 0)
                {
                    isTotalWalkingWASD++;
                }

                isTotalWalkingWASD = 0; //resets the number

                #endregion

                if (playerManager.worldSpaceEnemyUIBar)
                {
                   playerManager.worldSpaceEnemyUIBar.transform.LookAt(playerManager.playerMovableCamera.transform);
                }
            }
            if(playerManager.stamina <= 100)
            {
                playerManager.stamina += Time.deltaTime;
            }
        }
        //lookAtAngle = Mathf.Atan2(addMovement.x, addMovement.z)* Mathf.Rad2Deg + playerCam.transform.eulerAngles.y; // berekent de angle waar je naar kijkt
        //endAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, lookAtAngle, ref velocity, timeToTurn); // hiermee berekent je de angle van de speler naar links of rechts toe via de camera
    }
}
