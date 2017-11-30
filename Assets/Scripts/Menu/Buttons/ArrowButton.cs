using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArrowButton : CustomButton
{
    [SerializeField]
    private GameObject
        _leftArrow,
        _rightArrow;

    [SerializeField]
    private float _fade = 0.1f;

    private EventSystem _system;

    protected override void Start()
    {
        _system = EventSystem.current;

        Selectable[] selectables = GetComponentsInChildren<Selectable>();
        _leftArrow = selectables[1].gameObject;
        _rightArrow = selectables[2].gameObject;
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        StartCoroutine(CheckArrows(eventData));
    }

    public virtual void ArrowKey(int side) { print("nay"); }

    IEnumerator CheckArrows(BaseEventData eventData)
    {
        yield return new WaitForEndOfFrame();

        float timer = 0;

        bool pressed = false;

        while (true)
        {
            GameObject selected = _system.currentSelectedGameObject;

            if (selected == gameObject || selected == null)
                yield break;

            if (selected != _leftArrow && selected != _rightArrow)
            {
                base.OnDeselect(eventData);
                yield break;
            }

            timer += Time.deltaTime;

            if (!pressed)
            {
                if (!Input.GetMouseButton(0))
                    ArrowKey(selected == _leftArrow ? 1 : -1);

                pressed = true;
            }

            if (timer > _fade)
            {
                _system.SetSelectedGameObject(gameObject);
                yield break;
            }
            yield return null;
        }
    }
}
