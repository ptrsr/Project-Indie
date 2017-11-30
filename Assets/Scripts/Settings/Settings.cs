using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Setting
{
    multiplyingBulletsOnBlock,
	graduallySpeedingUp,
    test3
}

public class Settings : MonoBehaviour
{
    [System.Serializable]
    struct DefaultBoolSetting
    {
        public Setting setting;
        public bool value;
    }

    [SerializeField]
    private List<DefaultBoolSetting> _defaultBools = new List<DefaultBoolSetting>();

	[SerializeField]
    private bool[] bools;

    private void Awake()
    {
        ServiceLocator.Provide(this);
        ResetToDefault();
    }

    public void ResetToDefault()
    {
        bools = new bool[_defaultBools.Count];

        foreach (DefaultBoolSetting setting in _defaultBools)
            bools[(int)setting.setting] = setting.value;
    }

    public bool GetBool(Setting setting)
    {
        return bools[(int)setting];
    }

    public void SetBool(Setting setting, bool value)
    {
        bools[(int)setting] = value;
    }
}
