using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleButton : CustomButton
{
    private Toggle _toggle;

    protected override void Awake()
    {
        _toggle = GetComponentInChildren<Toggle>();
        base.Awake();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        _toggle.isOn ^= true;
        base.OnPointerClick(eventData);
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        _toggle.isOn ^= true;
        base.OnSubmit(eventData);
    }
}
