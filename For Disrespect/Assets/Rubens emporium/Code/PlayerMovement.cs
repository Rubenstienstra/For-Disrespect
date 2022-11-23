using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviourPunCallbacks , IPunObservable
{
    public int[] movementWASD;
    public int isTotalWalkingWASD;
    public bool holdingShift;

    public RaycastHit rayCastAttackHit;
    public float rayCastDistanceAttack;
    public bool isAttacking;
    public int hp = 10;

    public float movementShiftBuff;
    private float crShiftBuff = 1;
    public float movementSpeedBuff;

    public Vector3 rayCastPos;
    public float rayCastDistance;
    public RaycastHit hitSlope;
    public float distanceBetweenGround;

    public Animator playerAnimations;

    public PhotonView photonID;
    public CharacterController characterControl;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //stream.SendNext();
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

    public void Start()
    {
        print("ViewID: "+ photonID.ViewID);
    }
    void FixedUpdate()
    {
        if (photonID.IsMine)// && !PhotonNetwork.IsConnected
        {
            
            if (hp <= 0)
            {
                GameLobbyManager.gameLobbyInfo.LeaveRoom();
            }

            //ROBOT MOVEMENT
            if(movementWASD[2] > 0 || movementWASD[0] > 0 || playerAnimations.GetFloat("Speed") > 0.001f)
            {
                playerAnimations.SetFloat("Speed", -movementWASD[2] + movementWASD[0], 0.25f, Time.deltaTime);
            }
            else if(playerAnimations.GetFloat("Speed") < 0.002f && movementWASD[2] == 0 && movementWASD[0] == 0)
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

            //NOMRAL MOVEMENT
            Physics.Raycast(rayCastPos + transform.position, Vector3.down, out hitSlope, rayCastDistance); // maakt een rayccast aan die naar beneden toe gaat
            distanceBetweenGround = hitSlope.distance;
            if(hitSlope.distance < 0)
            {
                characterControl.Move(new Vector3(0,-1,0) * movementSpeedBuff * crShiftBuff * Time.deltaTime);
            }
            //if (hitSlope.distance >= 0.001f)
            //{
            //    characterControl.Move(new Vector3(-movementWASD[1] + movementWASD[3], 0, -movementWASD[2] + movementWASD[0]) * movementSpeedBuff * crShiftBuff * Time.deltaTime);
            //}
            //else
            //{
            //    characterControl.Move(new Vector3(-movementWASD[1] + movementWASD[3], -1, -movementWASD[2] + movementWASD[0]) * movementSpeedBuff * crShiftBuff * Time.deltaTime);
            //}

            //for (int i = 0; i < movementWASD.Length; i++)//checking if player is moving
            //{
            //    if(movementWASD[i] > 0)
            //    {
            //        isTotalWalkingWASD++;
            //    }
            //}
            //if(isTotalWalkingWASD > 0)
            //{
            //    playerAnimations.SetFloat("Speed", 1);
            //}
            //else
            //{
            //    playerAnimations.SetFloat("Speed", 0);
            //}
            //isTotalWalkingWASD = 0; //resets the number
        }
    }
    //lookAtAngle = Mathf.Atan2(addMovement.x, addMovement.z)* Mathf.Rad2Deg + playerCam.transform.eulerAngles.y; // berekent de angle waar je naar kijkt
    //endAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, lookAtAngle, ref velocity, timeToTurn); // hiermee berekent je de angle van de speler naar links of rechts toe via de camera

    public void OnAttack(InputValue value)
    {
        if (value.Get<float>() == 1)
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
            //if (rayCastAttackHit.collider.gameObject.tag == "Player") WERKT NIET???
            //{
            //    rayCastAttackHit.transform.gameObject.GetComponent<PlayerMovement>().hp--;
            //}
            isAttacking = false;
        }
    }
}
