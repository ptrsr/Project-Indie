using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levels : MonoBehaviour
{
    private GameObject[] _levels;

    private int selector = 0;

    private void Awake()
    {
        _levels = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
            _levels[i] = transform.GetChild(i).gameObject;

        ServiceLocator.Provide(this);
    }
    public string SetLevel(int number)
    {
        if (number < 0 || number > _levels.Length - 1)
        {
            Debug.LogWarning("Level doesn't exist!");
            return _levels[selector].name;
        }

        _levels[selector].SetActive(false);

        selector = number;
        _levels[selector].SetActive(true);

        return _levels[selector].name;
    }

    public int Count
    {
        get { return _levels.Length; }
    }
    
}
