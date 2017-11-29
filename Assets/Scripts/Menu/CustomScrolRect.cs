using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomScrolRect : ScrollRect
{
    [SerializeField]
    private float 
        _minDist = 10f,
        _scrollSpeed = 1f;

    private float
        _viewHeight,
        _contentHeight;

    protected override void Awake()
    {
        base.Awake();

        _viewHeight = GetComponent<RectTransform>().rect.height;
        _contentHeight = content.rect.height;
    }

    public void SelectButton(RectTransform transform)
    {
        float buttonHeight = _contentHeight + transform.localPosition.y;

        float bottom = verticalNormalizedPosition * (_contentHeight - _viewHeight);
        float top = bottom + _viewHeight;

        float move = 0;

        move += Mathf.Min(buttonHeight - bottom - transform.rect.height / 2 - _minDist, 0);
        move += Mathf.Max(buttonHeight - top + transform.rect.height / 2 + _minDist, 0);

        verticalNormalizedPosition = Mathf.Clamp01(verticalNormalizedPosition + move / _viewHeight);
    }
}
