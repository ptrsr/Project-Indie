using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Players;

public class CustomInputHandler : StandaloneInputModule
{
    protected override void Awake()
    {
        base.Awake();

        ServiceLocator.Provide(this);
    }

    public PointerEventData GetLastPointerEventDataPublic()
    {
        return GetLastPointerEventData(-1);
    }

    public void SetPlayer(Player player)
    {
        string playerName = player.ToString();

        horizontalAxis = playerName + "HorMove";
        verticalAxis = playerName + "VerMove";
    }
}
