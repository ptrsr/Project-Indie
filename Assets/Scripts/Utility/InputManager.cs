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

    public enum Axis
    {
        Hor,
        Ver
    }

    public enum Button
    {
        Fire,
        Parry
    }

    public enum InputType
    {
        Move,
        Aim
    }
}

public class InputManager : MonoBehaviour
{
    public delegate void Ready(Player player);
    public static event Ready ready;

    static private InputManager _instance = null;

    private Player[] _players = new Player[4];

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
            return;
        }

        _instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < _players.Length; i++)
            _players[i] = Player.none;
    }

    private void Update()
    {
        RegisterPlayers();
        CheckReadyUp();
    }

    private void RegisterPlayers()
    {
        if (Input.GetAxis("AllAxises") == 0)
            return;

        for (int i = 0; i < _players.Length; i++)
        {
            if (_players[i] != Player.none)
                continue;

            _players[i] = CheckPlayer();

            if (i == 0)
                ServiceLocator.Locate<CustomInputHandler>().SetPlayer(Player.P1);
        }
    }

    private void CheckReadyUp()
    {
        for (int i = 0; i < _players.Length; i++)
        {
            if (_players[i] == Player.none)
                return;

            if (Input.GetButton(_players[i].ToString() + "Fire"))
                ready(_players[i]);
        }
    }
    private Player CheckPlayer()
    {
        for (int i = 1; i < 3; i++)
        {
            string player = ((Player)i).ToString();

            for (int j = 0; j < 2; j++)
            {
                string aim = ((Axis)j).ToString();

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

    static public float GetAxis(Player player, InputType inputType, Axis axis)
    {
        Player controller = _instance._players[(int)player];

        if (controller == Player.none)
        {
            Debug.LogWarning("Trying to get axis from unregistered player!");
            return 0;
        }
        return Input.GetAxis(controller.ToString() + axis.ToString() + inputType.ToString());
    }

    static public bool GetButton(Player player, Button button)
    {
        Player controller = _instance._players[(int)player];

        if (controller == Player.none)
        {
            Debug.LogWarning("Trying to get button from unregistered player!");
            return false;
        }

        return Input.GetButton(controller.ToString() + button.ToString());
    }

    static public bool GetButtonDown(Player player, Button button)
    {
        Player controller = _instance._players[(int)player];

        if (controller == Player.none)
        {
            Debug.LogWarning("Trying to get button from unregistered player!");
            return false;
        }

        return Input.GetButtonDown(controller.ToString() + button.ToString());
    }

    static public bool GetButtonUp(Player player, Button button)
    {
        Player controller = _instance._players[(int)player];

        if (controller == Player.none)
        {
            Debug.LogWarning("Trying to get button from unregistered player!");
            return false;
        }

        return Input.GetButtonUp(controller.ToString() + button.ToString());
    }
}
