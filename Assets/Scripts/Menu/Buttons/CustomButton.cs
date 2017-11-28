using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : Button
{
    private CustomScrolRect _scroller;
    private RectTransform _transform;

    protected override void Awake()
    {
        base.Awake();

        _transform = GetComponentInParent<RectTransform>();
        _scroller = GetComponentInParent<CustomScrolRect>();
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);

        if (_scroller == null)
            return;

        _scroller.SelectButton(_transform);
    }
}
