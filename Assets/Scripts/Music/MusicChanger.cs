using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
public class MusicChanger : MonoBehaviour {

    //Changing these floats from 0 to 1 will play their corresponding tracks in FMOD
    //Remember to deactivate other tracks
    //PlayMain needs to be activated to skip from intro/outro to MT,A1,A2 and A3 
    public float m_MT, m_A1, m_A2, m_A3, m_O; //MainTheme, ActionLevel1, ActionLevel2, ActionLevel3, Outro

    FMODUnity.StudioEventEmitter emitter;
    private void Start()
    {
        emitter = gameObject.GetComponent<FMODUnity.StudioEventEmitter>();
    }
    private void Update()
    {
        /* Activate to test
        if (Input.GetKeyDown(KeyCode.Q))
        {
            m_MT = 1;
            m_A1 = 0;
            m_A2 = 0;
            m_A3 = 0;
            m_O = 0;

        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            m_MT = 0;
            m_A1 = 1;
            m_A2 = 0;
            m_A3 = 0;
            m_O = 0;

        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            m_MT = 0;
            m_A1 = 0;
            m_A2 = 1;
            m_A3 = 0;
            m_O = 0;

        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            m_MT = 0;
            m_A1 = 0;
            m_A2 = 0;
            m_A3 = 1;
            m_O = 0;

        }*/


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
    }
}
