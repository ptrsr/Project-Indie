using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ButtonAnimController : MonoBehaviour {

    private void Update()
    {
		#if UNITY_EDITOR
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
		#endif
    }
    public void ButtonSelected()
    {
        if (gameObject.tag == "LargeButton")
        {
            //gameObject.GetComponent<Animation>().Play("ButtonSelected");
            GetComponent<Animator>().SetInteger("Selected", 1);

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
        }
        else if (gameObject.tag == "SmallButton")
        {
            //gameObject.GetComponent<Animation>().Play("SmallButtonReturn");
            GetComponent<Animator>().SetInteger("Selected", 0);
        }
    }
}
