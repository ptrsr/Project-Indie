using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Players;

namespace Players
{
    public enum Player
    {
        none,
        P1,
        P2
    }
}

public class InputManager : MonoBehaviour
{

    private enum Aim
    {
        Hor,
        Ver
    }

    private enum InputType
    {
        Move,
        Aim
    }

    private Player _player1 = Player.none;

    private void Update()
    {
        if (_player1 == Player.none && Input.GetAxis("AllAxises") != 0)
        {
            _player1 = CheckPlayer();
            ServiceLocator.Locate<CustomInputHandler>().SetPlayer(_player1);
        }
    }

    private Player CheckPlayer()
    {
        List<string> _axises = new List<string>();

        for (int i = 1; i < 3; i++)
        {
            string player = ((Player)i).ToString();

            for (int j = 0; j < 2; j++)
            {
                string aim = ((Aim)j).ToString();

                for (int k = 0; k < 2; k++)
                {
                    string inputType = ((InputType)k).ToString();
                    if (Input.GetAxis(player + aim + inputType) != 0)
                        return ((Player)i);
                }
            }
        }
        return Player.none;
    }
}
