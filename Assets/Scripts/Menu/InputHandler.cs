using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Players;

namespace Players
{
    public enum Player
    {
        none,
        P1,
        P2,
        P3,
        P4,
        P5
    }

    public enum Axis
    {
        Hor,
        Ver
    }

    public enum Button
    {
        Fire,
        Parry,
		Submit,
		Cancel,
		Settings
    }

    public enum InputType
    {
        Move,
        Aim,
        Submit
    }
}

public class InputHandler : StandaloneInputModule
{
    public delegate void Ready(Player player);
    public static event Ready ready;

    static private InputHandler _instance = null;

    private Player[] _players = new Player[4];

    protected override void Awake()
    {
        base.Awake();

        if (_instance != null)
        {
            Destroy(this);
            return;
        }

        _instance = this;

        ServiceLocator.Provide(this);
    }

    protected override void Start()
    {
        base.Start();

        for (int i = 0; i < _players.Length; i++)
            _players[i] = Player.none;
    }

    private void Update()
    {
        RegisterPlayers();
        CheckReadyUp();
    }

    public PointerEventData GetLastPointerEventDataPublic()
    {
        return GetLastPointerEventData(-1);
    }

    private void RegisterPlayers()
    {
        for (int i = 0; i < _players.Length; i++)
        {
            if (_players[i] != Player.none)
                continue;

            _players[i] = CheckPlayer(Input.GetAxis("AllAxises") != 0);

            if (i == 0 && _players[i] != Player.none)
				SetPlayer(_players[0]);
        }
    }

    private void CheckReadyUp()
    {
		if (ready == null)
			return;

        for (int i = 0; i < _players.Length; i++)
        {
            if (_players[i] == Player.none)
				continue;

			if (Input.GetButtonDown (_players [i].ToString () + "Submit"))
				ready ((Player)(i + 1));
        }
    }
    private Player CheckPlayer(bool checkAxises)
    {
        for (int i = 1; i < 6; i++)
        {
            string player = ((Player)i).ToString();

			bool skip = false;

			for (int l = 0; l < _players.Length; l++) 
			{
				if ((int)_players[l] == i)
				{
					skip = true;
					break;
				}
			}

			if (skip)
				continue;

            if (Input.GetButton(player + "Submit"))
                return ((Player)i);

            if (!checkAxises)
                continue;

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
		Player controller = _instance._players[(int)(player - 1)];

        if (controller == Player.none)
        {
            Debug.LogWarning("Trying to get axis from unregistered player!");
            return 0;
        }
        return Input.GetAxis(controller.ToString() + axis.ToString() + inputType.ToString());
    }

    static public bool GetButton(Player player, Button button)
    {
		Player controller = _instance._players[(int)(player - 1)];

        if (controller == Player.none)
        {
            Debug.LogWarning("Trying to get button from unregistered player!");
            return false;
        }

        return Input.GetButton(controller.ToString() + button.ToString());
    }

    static public bool GetButtonDown(Player player, Button button)
    {
		Player controller = _instance._players[(int)(player - 1)];

        if (controller == Player.none)
        {
            Debug.LogWarning("Trying to get button from unregistered player!");
            return false;
        }

        return Input.GetButtonDown(controller.ToString() + button.ToString());
    }

    static public bool GetButtonUp(Player player, Button button)
    {
		Player controller = _instance._players[(int)(player - 1)];

        if (controller == Player.none)
        {
            Debug.LogWarning("Trying to get button from unregistered player!");
            return false;
        }

        return Input.GetButtonUp(controller.ToString() + button.ToString());
    }

    public void SetPlayer(Player player)
    {
        string playerName = player.ToString();

        horizontalAxis = playerName + "HorMove";
        verticalAxis = playerName + "VerMove";
		submitButton = playerName + "Submit";
		//cancelButton = playerName + "Cancel";
    }
}
