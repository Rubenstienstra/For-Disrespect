using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MainMenuSettings : MonoBehaviour
{
    public AudioMixer audiomixer;

    public void SetVolume(float volume)
    {
        audiomixer.SetFloat("Volume", volume);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;

    }
    public void kutGame()
    {
        Application.Quit();
        Debug.Log("i guit");
    }
}
