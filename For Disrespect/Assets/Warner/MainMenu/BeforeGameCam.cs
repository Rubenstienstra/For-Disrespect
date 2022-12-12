using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeforeGameCam : MonoBehaviour
{

    public Animator beforeGameCam;

    public void camMove()
    {
        beforeGameCam.SetBool("BeforeComabt", true);
    }

    public void camBack()
    {
        beforeGameCam.SetBool("BeforeComabt", false);
    }

}
