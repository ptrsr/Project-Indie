using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleButton : CustomButton
{
    private Toggle _toggle;

    [SerializeField]
    private Setting _setting;

    [SerializeField]
    private bool _value;

    private Settings _settings;

    protected override void Awake()
    {
        _toggle = GetComponentInChildren<Toggle>();
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        _settings = ServiceLocator.Locate<Settings>();
        _value = _settings.GetBool(_setting);
        _toggle.isOn = _value;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        _toggle.isOn ^= true;
        _settings.SetBool(_setting, _toggle.isOn);
        base.OnPointerClick(eventData);
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        _toggle.isOn ^= true;
        _settings.SetBool(_setting, _toggle.isOn);
        base.OnSubmit(eventData);
    }
}
