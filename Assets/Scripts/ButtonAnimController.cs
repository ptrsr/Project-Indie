using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ButtonAnimController : MonoBehaviour {

    private void Update()
    {
        //For Testing ---
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ButtonSelected();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            ButtonReturn();
        }
        // ---
    }
    public void ButtonSelected()
    {
        gameObject.GetComponent<Animation>().Play("ButtonSelected");
    }
    public void ButtonReturn()
    {
        gameObject.GetComponent<Animation>().Play("ButtonReturn");
    }


}
