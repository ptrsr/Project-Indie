using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomScrolRect : ScrollRect
{
    [SerializeField]
    private float
        _minDist = 10f,
        _scrollMulti = 5,
        _scrollInertia = 5;

    [SerializeField] [Range(0, 1)]
    private float
        _scrollSpeed = 0.99f;

    private float
        _viewHeight,
        _contentHeight;

    private float
        _desiredPosition;

    protected override void Awake()
    {
        base.Awake();

        _desiredPosition = verticalNormalizedPosition;
        _viewHeight = GetComponent<RectTransform>().rect.height;
        _contentHeight = content.rect.height;
    }

    private void Update()
    {
        float speed = 1 - Mathf.Pow(Mathf.Abs(_desiredPosition - verticalNormalizedPosition), _scrollInertia);

        verticalNormalizedPosition = Mathf.Lerp(verticalNormalizedPosition, _desiredPosition, Mathf.Max(speed, _scrollSpeed) * Time.deltaTime * _scrollMulti);
    }

    public void SelectButton(RectTransform transform)
    {
        float buttonHeight = _contentHeight + transform.localPosition.y;

        float bottom = _desiredPosition * (_contentHeight - _viewHeight);
        float top = bottom + _viewHeight;

        float move = 0;

        move += Mathf.Min(buttonHeight - bottom - transform.rect.height / 2 - _minDist, 0);
        move += Mathf.Max(buttonHeight - top + transform.rect.height / 2 + _minDist, 0);

        _desiredPosition = Mathf.Clamp01(_desiredPosition + move / _viewHeight);
    }
}
