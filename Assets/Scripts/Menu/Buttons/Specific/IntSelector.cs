using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntSelector : ArrowButton {

    int _selector = 0;

    [SerializeField]
    private Setting _setting;

    [SerializeField]
    private int
        _min = 0,
        _max = 3;


    private Text _text;
    private Settings _settings;

    [SerializeField]
    private bool _minusOneIsInfinity = false;

    protected override void Start()
    {
        base.Start();
        _settings = ServiceLocator.Locate<Settings>();
        _text = GetComponentsInChildren<Text>()[1];

        _selector = Mathf.Clamp(_settings.GetInt(_setting), _min, _max);

		if (_selector == -1)
			_text.text = "∞";
		else
        	_text.text = _selector.ToString();
    }

    public override void ArrowKey(int side)
    {
        _selector += side;

        if (_selector < _min)
            _selector -= _min - _max;
        else if (_selector >= _max)
            _selector += _min - _max;

        if (_minusOneIsInfinity && _selector == -1)
            _text.text = "∞";
        else
            _text.text = _selector.ToString();

		_settings.SetInt (_setting, _selector);
    }
}
