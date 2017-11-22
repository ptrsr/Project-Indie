using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuItem : MonoBehaviour
{
    #region Items
    [System.Serializable]
    struct NearbyItems
    {
        public MenuItem
            Up,
            down,
            Left,
            Right;
    }
    [SerializeField]
    private NearbyItems _items;
    #endregion

    void Start ()
    {
		
	}
}
