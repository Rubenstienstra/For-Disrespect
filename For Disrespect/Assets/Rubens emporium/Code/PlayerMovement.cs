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
        transform.Translate(new Vector3(-movementWASD[1] + movementWASD[3], 0, -movementWASD[2] + movementWASD[0])* movementSpeedBuff * crShiftBuff * Time.deltaTime);
    }
}
