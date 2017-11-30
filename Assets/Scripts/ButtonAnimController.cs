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
            //gameObject.GetComponent<Animation>().Play("ButtonSelected");
            GetComponent<Animator>().SetInteger("Selected", 1);
            Debug.Log("Check");

        }
        else if (gameObject.tag == "SmallButton")
        {
            //gameObject.GetComponent<Animation>().Play("SmallButtonSelected");
            GetComponent<Animator>().SetInteger("Selected", 1);
            
        }
    }
    public void ButtonReturn()
    {
        if (gameObject.tag == "LargeButton")
        {
            //gameObject.GetComponent<Animation>().Play("ButtonReturn");
            GetComponent<Animator>().SetInteger("Selected", 0);
            Debug.Log("Check1");
        }
        else if (gameObject.tag == "SmallButton")
        {
            //gameObject.GetComponent<Animation>().Play("SmallButtonReturn");
            GetComponent<Animator>().SetInteger("Selected", 0);
        }
    }
}
