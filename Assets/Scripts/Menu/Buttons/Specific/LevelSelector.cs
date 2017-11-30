using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : ArrowButton
{
    private Levels _levels;

    private int _selector = 0;
    private Text _text;

    protected override void Start()
    {
        base.Start();

        _levels = ServiceLocator.Locate<Levels>();
        _text = GetComponentInChildren<Text>();

        _text.text = _levels.ChangeLevel(0);
    }


    public override void ArrowKey(int side)
    {
        _selector += side;

        if (_selector < 0)
            _selector += _levels.Count;

        _selector %= _levels.Count;

        _text.text = _levels.ChangeLevel(_selector);
    }
}
