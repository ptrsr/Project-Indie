using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomInputHandler : StandaloneInputModule
{
    public PointerEventData GetLastPointerEventDataPublic()
    {
        return GetLastPointerEventData(-1);
    }
}
