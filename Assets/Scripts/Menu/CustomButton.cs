using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : Button {

    public override void OnSelect(BaseEventData eventData)
    {
        print(true);
        base.OnSelect(eventData);
    }

    public override Selectable FindSelectableOnRight()
    {
        print(false);
        return base.FindSelectableOnRight();
    }


    // Update is called once per frame
    void Update () {

        foreach (string joyName in Input.GetJoystickNames())
        {
            print(joyName);
        } 

	}
}
