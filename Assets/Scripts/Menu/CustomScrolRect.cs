using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Players;

public class CustomScrolRect : ScrollRect
{
    private delegate void Check();
    private Check _Check;


    private enum lastInput
    {
        Mouse,
        Controller
    }

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
        _contentHeight,
        _desiredPosition;

    private lastInput _lastInput = lastInput.Controller;
    private InputHandler _inputHandler;

    protected override void Awake()
    {
        base.Awake();

        _desiredPosition = verticalNormalizedPosition;
        _viewHeight = GetComponent<RectTransform>().rect.height;
        _contentHeight = content.rect.height;
    }

    protected override void Start()
    {
        base.Start();
        _inputHandler = ServiceLocator.Locate<InputHandler>();
    }

    private void Update()
    {
        if (!Application.isPlaying || _inputHandler == null)
            return;

        AutoScroll();

        if (_Check != null)
            _Check();
    }

    private void AutoScroll()
    {
        if (InputHandler.GetAxis(Player.P1, InputType.Move, Axis.Ver) != 0)
            _lastInput = lastInput.Controller;
        else if (_inputHandler.GetLastPointerEventDataPublic().IsPointerMoving())
            _lastInput = lastInput.Mouse;

        if (_lastInput == lastInput.Mouse)
            return;

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

    public void SelectSlider(bool selected)
    {
        if (selected)
        {
            _Check -= CheckSlider;
            _Check += CheckSlider;
        }
        else
            _Check -= CheckSlider;
    }

    private void CheckSlider()
    {
        if (_lastInput == lastInput.Controller)
            ServiceLocator.Locate<Menu>().SelectObject(GetComponentInParent<SubMenu>().FirstSelected);
    }
}
