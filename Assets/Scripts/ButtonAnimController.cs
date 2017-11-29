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
        if (gameObject.tag == "LargeButton")
        {
            gameObject.GetComponent<Animation>().Play("ButtonSelected");
        }
        else if (gameObject.tag == "SmallButton")
        {
            gameObject.GetComponent<Animation>().Play("SmallButtonSelected");
        }
    }
    public void ButtonReturn()
    {
        if (gameObject.tag == "LargeButton")
        {
            gameObject.GetComponent<Animation>().Play("ButtonReturn");
        }
        else if (gameObject.tag == "SmallButton")
        {
            gameObject.GetComponent<Animation>().Play("SmallButtonReturn");
        }
    }
}
