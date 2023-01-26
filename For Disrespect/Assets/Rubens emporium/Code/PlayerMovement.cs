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

    public bool isAttacking;
    public bool waitedBeforeAttacking;
    public float waitTimeBeforeAttacking = 1f;
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
            //stream.SendNext(transform.position);
            stream.SendNext(allowMoving);
            stream.SendNext(isAttacking);
            stream.SendNext(playerID);
            stream.SendNext(isBlocking);

            stream.SendNext(playerManager.crPlayerName);
            stream.SendNext(playerManager.isHost);
            stream.SendNext(playerManager.isGuest);
            stream.SendNext(playerManager.isReadyLobby);
            stream.SendNext(playerManager.isReadyToFight);
            stream.SendNext(playerManager.stamina);
            stream.SendNext(playerManager.syncedHP);
            //stream.SendNext(playerManager.hp);
        }
        else if(stream.IsReading)
        {
            //this.transform.position = (Vector3)stream.ReceiveNext();
            this.allowMoving = (bool)stream.ReceiveNext();
            this.isAttacking = (bool)stream.ReceiveNext();
            this.playerID = (int)stream.ReceiveNext();
            this.isBlocking = (bool)stream.ReceiveNext();

            playerManager.crPlayerName = (string)stream.ReceiveNext();
            playerManager.isHost = (bool)stream.ReceiveNext();
            playerManager.isGuest = (bool)stream.ReceiveNext();
            playerManager.isReadyLobby = (bool)stream.ReceiveNext();
            playerManager.isReadyToFight = (bool)stream.ReceiveNext();
            playerManager.stamina = (float)stream.ReceiveNext();
            playerManager.syncedHP = (float)stream.ReceiveNext();
            //playerManager.hp = (float)stream.ReceiveNext();
        }
    }
    #region InputActions
    public void OnEsc(InputValue value)
    {
        if (photonID.IsMine && playerManager.hasStartedGame && !playerManager.theGameEnded)
        {
            if(!hasOpenedESC)
            {
                hasOpenedESC = true; allowMoving = false;
                playerUI.playerESCMenu.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                return;
            }
            DisableESCMenu();
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

            if (allowMoving && !playerManager.theGameEnded)
            {
                playerManager.playerAnimations.SetFloat("Vertical", -movementWASD[2] + movementWASD[0]);
            }
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
            if (allowMoving && !playerManager.theGameEnded)
            {
                playerManager.playerAnimations.SetFloat("Horizontal", -movementWASD[1] + movementWASD[3]);
            }
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
            if (allowMoving && !playerManager.theGameEnded)
            {
                playerManager.playerAnimations.SetFloat("Vertical", -movementWASD[2] + movementWASD[0]);
            }
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
            if (allowMoving && !playerManager.theGameEnded)
            {
                playerManager.playerAnimations.SetFloat("Horizontal", -movementWASD[1] + movementWASD[3]);
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
            if (allowMoving && !playerManager.theGameEnded)
            {
                playerManager.playerAnimations.SetBool("Running", holdingShift);
            }
        }
    }
    public void OnMouseXY(InputValue value)
    {
        if (photonID.IsMine && allowMoving && !hasOpenedESC && !playerManager.theGameEnded)
        {
            mouseXYInput = value.Get<Vector2>();

            transform.Rotate(0, mouseXYInput.x * rotationXYSpeed.x * Time.deltaTime, 0);
        }
    }
    public void OnAttack(InputValue value)
    {
        if (photonID.IsMine && playerManager.stamina >= playerManager.staminaCostAttack && allowMoving && waitedBeforeAttacking && !playerManager.theGameEnded)
        {
            if (value.Get<float>() == 1)
            {
                isAttacking = true; waitedBeforeAttacking = false;
                allowMoving = false;
                playerManager.playerAttackCollider.enabled = enabled;

                playerManager.stamina -= playerManager.staminaCostAttack;
                StartCoroutine(Attack());
                playerManager.playerAnimations.SetTrigger("Attack");
            }
        }
    }
    public void OnBlock(InputValue value)
    {
        if (photonID.IsMine && !isAttacking && !hasOpenedESC && allowMoving && !playerManager.theGameEnded)
        {
            if (value.Get<float>() == 1)
            {
                isBlocking = true;
            }
            else
            {
                isBlocking = false;
            }
        }
    }
    #endregion

    public IEnumerator Attack()
    {
        if (photonID.IsMine &&!playerManager.theGameEnded)
        {
            yield return new WaitForSeconds(0.5f);//Wait time for collider to trigger form gameobjects around him.
            if (playerManager.playerInAttackRange)
            {
                if (playerManager.playerInAttackRange.GetComponent<PlayerMovement>().isBlocking)
                {
                    playerManager.DealtBlockedDamage();
                }
                else
                {
                    playerManager.SuccesfullyDealtDamage();
                }
            }
            else
            {
                print("playerCollider coudn't find GameObjects");
            }
            allowMoving = true;
            isAttacking = false;
            playerManager.playerAttackCollider.enabled = !enabled;
            playerManager.playerInAttackRange = null;

            yield return new WaitForSeconds(waitTimeBeforeAttacking);
            waitedBeforeAttacking = true;
        }
        yield return new WaitForSeconds(0);
    }
    public void DisableESCMenu()
    {
        hasOpenedESC = false; allowMoving = true;
        playerUI.playerESCMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void Start()
    {
        
    }

    void FixedUpdate()
    {
        if (playerManager.isReadyToFight && !playerManager.theGameEnded)
        {
            if (photonID.IsMine)
            {
                #region MOVEMENT
                if (!isAttacking && allowMoving)
                {
                    Physics.Raycast(rayCastPos + transform.position, Vector3.down, out hitSlope, rayCastDistance); // maakt een rayccast aan die naar beneden toe gaat om te checken of gravity aan moet.
                    distanceBetweenGround = hitSlope.distance;

                    if (distanceBetweenGround <= 0.001f)
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

                    }
                }
                #endregion

                if (playerUI.enemyWorldSpaceUI)
                {
                    playerUI.enemyWorldSpaceUI.transform.LookAt(playerManager.playerMovableCamera.transform);
                }

                if (playerManager.stamina < 100)
                {
                    playerManager.stamina += Time.deltaTime * playerManager.staminaRegenRate;
                    if (playerManager.stamina > 100)
                    {
                        playerManager.stamina = 100;
                    }
                }
            }
            if (playerManager.hp < playerManager.syncedHP)// Als jij damage doet in de lobby zal je HP altijd lager zijn dan de syncedHP dus moet de syncedHP Updaten.
            {
                playerManager.syncedHP = playerManager.hp;
            }
            else if (playerManager.syncedHP < playerManager.hp)// Als jouw HP hoger is dan wat er gesynced is betekent dat je damage hebt gekregen en dat moet je updaten met je eigen HP.
            {
                playerManager.hp = playerManager.syncedHP;
            }
        }
    }
}
