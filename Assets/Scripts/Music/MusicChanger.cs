using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
public class MusicChanger : MonoBehaviour {

    //Changing these floats from 0 to 1 will play their corresponding tracks in FMOD
    public float m_MT, m_A1, m_A2, m_A3, m_O, m_Menu, m_Pause; //MainTheme, ActionLevel1, ActionLevel2, ActionLevel3, Outro, Menu Music, Pause;

    FMODUnity.StudioEventEmitter emitter;
    private void Start()
    {
        emitter = gameObject.GetComponent<FMODUnity.StudioEventEmitter>();
    }
    private void Update()
    {

        if (m_MT == 1f)
        {
            emitter.SetParameter("MainTheme", 1);
        }
        else
        {
            emitter.SetParameter("MainTheme", 0);
        }
        if (m_A1 == 1f)
        {
            emitter.SetParameter("ActionLevel1", 1);
        }
        else
        {
            emitter.SetParameter("ActionLevel1", 0);
        }
        if (m_A2 == 1f)
        {
            emitter.SetParameter("ActionLevel2", 1);
        }
        else
        {
            emitter.SetParameter("ActionLevel2", 0);
        }
        if (m_A3 == 1f)
        {
            emitter.SetParameter("ActionLevel3", 1);
        }
        else
        {
            emitter.SetParameter("ActionLevel3", 0);
        }
        if (m_O == 1f)
        {
            emitter.SetParameter("Outro", 1);
        }
        else
        {
            emitter.SetParameter("Outro", 0);
        }
        if (m_Menu == 1f)
        {
            emitter.SetParameter("MenuMusic", 1);
        }
        else
        {
            emitter.SetParameter("MenuMusic", 0);
        }
        if (m_Pause == 1f)
        {
            emitter.SetParameter("Pause", 1);
        }
        else
        {
            emitter.SetParameter("Pause", 0);
        }
    }
}
