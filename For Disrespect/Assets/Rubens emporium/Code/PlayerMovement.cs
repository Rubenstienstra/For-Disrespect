using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public int[] movementWASD;
    public bool holdingShift;

    public float movementShiftBuff;
    private float crShiftBuff = 1;
    public float movementSpeedBuff;

    public Vector3 rayCastPos;
    public float rayCastDistance;
    public RaycastHit hitSlope;
    public float distanceBetweenGround;

    public CharacterController characterControl;

    public void OnForward(InputValue value)
    {
        if(value.Get<float>() == 1)
        {
            movementWASD[0] = 1;
        }
        else
        {
            movementWASD[0] = 0;
        }
    }
    public void OnLeft(InputValue value)
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
    public void OnDown(InputValue value)
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
    public void OnRight(InputValue value)
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
    public void OnShift(InputValue value)
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

    void FixedUpdate()
    {
        Physics.Raycast(rayCastPos + transform.position, Vector3.down, out hitSlope, rayCastDistance); // maakt een rayccast aan die naar beneden toe gaat
        distanceBetweenGround = hitSlope.distance;
        if (hitSlope.distance >= 0.001f)
        {
            characterControl.Move(new Vector3(-movementWASD[1] + movementWASD[3], 0, -movementWASD[2] + movementWASD[0]) * movementSpeedBuff * crShiftBuff * Time.deltaTime);
        }
        else
        {
            characterControl.Move(new Vector3(-movementWASD[1] + movementWASD[3], -1, -movementWASD[2] + movementWASD[0]) * movementSpeedBuff * crShiftBuff * Time.deltaTime);
        }
    }
    //lookAtAngle = Mathf.Atan2(addMovement.x, addMovement.z)* Mathf.Rad2Deg + playerCam.transform.eulerAngles.y; // berekent de angle waar je naar kijkt
    //endAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, lookAtAngle, ref velocity, timeToTurn); // hiermee berekent je de angle van de speler naar links of rechts toe via de camera
}
