using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Setting
{
    multiplyingBulletsOnBlock,
	graduallySpeedingUp,
    Laser,
    maxBounces,
	roundsToWin,
	clipSize
}

public class Settings : MonoBehaviour
{

    [System.Serializable]
    class DefaultSetting
    {
        public Setting setting;
    }

    [System.Serializable]
    class BoolSetting : DefaultSetting
    {
        public bool value;
    }

    [System.Serializable]
    class IntSetting : DefaultSetting
    {
        public int value;
    }

    [SerializeField]
    private List<BoolSetting> _defaultBools = new List<BoolSetting>();

    [SerializeField]
    private List<IntSetting> _defaultInts = new List<IntSetting>();

    private bool[] bools;
    private int[] ints;

    private void Awake()
    {
        bools = new bool[10];
        ints = new int[10];

        ServiceLocator.Provide(this);
        ResetToDefault();
    }

    public void ResetToDefault()
    {
        bools = new bool[_defaultBools.Count];

        foreach (BoolSetting setting in _defaultBools)
            bools[(int)setting.setting] = setting.value;

		foreach (IntSetting setting in _defaultInts)
			ints [(int)setting.setting] = setting.value;
    }

    public bool GetBool(Setting setting)
    {
        return bools[(int)setting];
    }

    public void SetBool(Setting setting, bool value)
    {
        bools[(int)setting] = value;
    }

    public int GetInt(Setting setting)
    {
        return ints[(int)setting];
    }

    public void SetInt(Setting setting, int value)
    {
        ints[(int)setting] = value;
    }
}
