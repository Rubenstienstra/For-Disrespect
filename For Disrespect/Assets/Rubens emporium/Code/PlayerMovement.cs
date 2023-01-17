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

    public Vector2 mouseXYInput;
    public Vector2 oldMouseXYInput;
    public bool[] isGoingLeftOrRight;
    public float minXRotation;
    public float maxXRotation;
    public Vector2 rotationXYSpeed;

    public float movementShiftBuff;
    private float crShiftBuff = 1;
    public float movementSpeedBuff;

    public Vector3 rayCastPos;
    public float rayCastDistance;
    public RaycastHit hitSlope;
    public float distanceBetweenGround;

    public GameObject UIPrefab;
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

            stream.SendNext(playerManager.crPlayerName);
            stream.SendNext(playerManager.isHost);
            stream.SendNext(playerManager.isGuest);
            stream.SendNext(playerManager.isReadyLobby);
            stream.SendNext(playerManager.isReadyToFight);
            stream.SendNext(playerManager.hp);
        }
        else if(stream.IsReading)
        {
            this.transform.position = (Vector3)stream.ReceiveNext();
            this.allowMoving = (bool)stream.ReceiveNext();
            this.isAttacking = (bool)stream.ReceiveNext();
            this.playerID = (int)stream.ReceiveNext();

            playerManager.crPlayerName = (string)stream.ReceiveNext();
            playerManager.isHost = (bool)stream.ReceiveNext();
            playerManager.isGuest = (bool)stream.ReceiveNext();
            playerManager.isReadyLobby = (bool)stream.ReceiveNext();
            playerManager.isReadyToFight = (bool)stream.ReceiveNext();
            this.playerManager.hp = (int)stream.ReceiveNext();
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
    public void OnMouseXY(InputValue value)
    {
        if (allowMoving && photonID.IsMine)
        {
            mouseXYInput = value.Get<Vector2>();

            if (transform.rotation.x > minXRotation && transform.rotation.x < maxXRotation)
            {
                transform.Rotate(0, mouseXYInput.x * rotationXYSpeed.x * Time.deltaTime, 0);
            }
        }
    }
    public void OnAttack(InputValue value)
    {
        if (photonID.IsMine)
        {
            inputAttack = value.Get<float>();
            if (value.Get<float>() > 0)
            {
                if (!isAttacking)
                {
                    isAttacking = true;
                    Attack();
                }
            }
        }
    }

    public void Attack()
    {
        if (photonID.IsMine)
        {
            Physics.Raycast(transform.position, Vector3.forward, out rayCastAttackHit);
            if (rayCastAttackHit.transform != null)
            {
                print("It has Found: " + rayCastAttackHit.collider.gameObject.name);
                if (rayCastAttackHit.transform.tag == "Player")
                {
                    playerManager.SuccesfullyDealtDamage(rayCastAttackHit);
                }
            }
            isAttacking = false;
        }
    }
    public void Start()
    {
        minXRotation += playerManager.playerMovableCamera.rotation.x;
        maxXRotation += playerManager.playerMovableCamera.rotation.x;
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

                isTotalWalkingWASD = 0; //resets the number

                #endregion

                #region MOUSE ROTATION

                #endregion
            }
        }
        //lookAtAngle = Mathf.Atan2(addMovement.x, addMovement.z)* Mathf.Rad2Deg + playerCam.transform.eulerAngles.y; // berekent de angle waar je naar kijkt
        //endAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, lookAtAngle, ref velocity, timeToTurn); // hiermee berekent je de angle van de speler naar links of rechts toe via de camera
    }
}
