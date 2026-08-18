using Raylib_cs;

public class InputManager
{

    public const int MAX_KEYBOARD_PLAYERS = 2;
    public const int MAX_PLAYERS = 8;
    public const int MAX_CONTROLLER_LISTENING = 128;


    public static readonly Color[] PlayerColours = [
        new(245, 46, 46, 255), new(84, 99, 255, 255), new(255, 199, 23, 255), new(31, 158, 64, 255),
        new(255, 102, 25, 255), new(36, 212, 196, 255), new(212, 28, 229, 255), new(74, 69, 89, 255)
    ];

    public static readonly PlayerSlot[] Players = new PlayerSlot[MAX_PLAYERS]  {
        new(0),
        new(1),
        new(2),
        new(3),
        new(4),
        new(5),
        new(6),
        new(7),
    };

    public static bool IsListening;

    // Input Events
    public static event Action<int>? PlayerJoined;
    public static event Action<int>? PlayerDropped;
    public static event Action<int>? PlayerDisconnected;
    public static event Action<int>? PlayerReconnected;

    public static void Update()
    {
        foreach (PlayerSlot player in Players)
        {
            if (player.IsActive)
            {
                player.Input.Lifetime += Raylib.GetFrameTime();
                player.Input.TimeSinceIdentifyingInput += Raylib.GetFrameTime();
            }
        }

        CheckPlayerJoins();
        CheckControllerDisconnects();

        if (IsListening)
        {
            CheckPlayerDrops();
            ClearDisconnectedPlayers();
        }

    }

    private static void CheckPlayerJoins()
    {
#if ARCADEMIA
                if (Raylib.IsKeyPressed((KeyboardKey)ArcademiaKeybind.P1_A))
                {
                    if (Players[0].IsActive)
                    {
                        Players[0].Input.TimeSinceIdentifyingInput = 0;
                    }
                    else if (IsListening)
                    {
                        Players[0].IsActive = true;
                        Players[0].Input = new(true, 0);
                        PlayerJoined?.Invoke(0);
                    }
                }

                if (Raylib.IsKeyPressed((KeyboardKey)ArcademiaKeybind.P2_A))
                {
                    if (Players[1].IsActive)
                    {
                        Players[1].Input.TimeSinceIdentifyingInput = 0;
                    }
                    else if (IsListening)
                    {
                        Players[1].IsActive = true;
                        Players[1].Input = new(true, 1);
                        PlayerJoined?.Invoke(1);
                    }
                }

#else
        ButtonAction JoinGame = ActionMap.GetButtonAction("JoinGame");

        // New Keyboard Player
        for (int i = 0; i < MAX_KEYBOARD_PLAYERS; i++)
        {
            bool keyboardTaken = false;
            for (int j = 0; j < MAX_PLAYERS; j++)
            {
                if (Players[j].IsActive && Players[j].Input.IsKeyboard && Players[j].Input.InputIdx == i)
                {
                    keyboardTaken = true;
                    if (Players[j].Input.IsActionDown("JoinGame"))
                    {
                        Players[j].Input.TimeSinceIdentifyingInput = 0;
                    }
                }
            }

            if (keyboardTaken)
                continue;


            if (Raylib.IsKeyPressed(JoinGame.KeyboardKeys[i]))
            {
                if (GetNextDisconnectedPlayerSlot() != -1)
                {
                    int slot = GetNextDisconnectedPlayerSlot();
                    Players[slot].Input = new PlayerInput(true, i, Players[slot].Input.Lifetime);
                    PlayerReconnected?.Invoke(slot);
                }
                else if (IsListening && IsThereAvailablePlayerSlot())
                {
                    int slot = GetNextInactivePlayerSlot();
                    Players[slot].Input = new PlayerInput(true, i);
                    Players[slot].IsActive = true;
                    PlayerJoined?.Invoke(slot);
                }

            }
        }

        for (int i = 0; i < MAX_PLAYERS; i++)
        {
            if (Players[i].IsActive && Players[i].Input.IsKeyboard &&
                Raylib.IsKeyPressed(JoinGame.KeyboardKeys[Players[i].Input.InputIdx]))
            {
                Players[i].Input.TimeSinceIdentifyingInput = 0;
            }
        }


        // New Controller Player
        for (int gamepadIdx = 0; gamepadIdx < MAX_CONTROLLER_LISTENING; gamepadIdx++)
        {
            // Is Gamepad Valid and Available
            if (!Raylib.IsGamepadAvailable(gamepadIdx))
                continue;

            // If this controller is already registered, skip it
            bool controllerTaken = false;
            for (int j = 0; j < MAX_PLAYERS; j++)
            {
                if (Players[j].IsActive && !Players[j].Input.IsKeyboard && Players[j].Input.IsConnected &&
                    Players[j].Input.InputIdx == gamepadIdx)
                {
                    controllerTaken = true;
                    if (Raylib.IsGamepadButtonPressed(gamepadIdx, JoinGame.GamepadButton))
                    {
                        Players[j].Input.TimeSinceIdentifyingInput = 0;
                    }
                }
            }

            if (controllerTaken)
                continue;

            // Check if the start button of this controller is pressed
            if (Raylib.IsGamepadButtonPressed(gamepadIdx, JoinGame.GamepadButton))
            {
                if (GetNextDisconnectedPlayerSlot() != -1)
                {
                    int slot = GetNextDisconnectedPlayerSlot();
                    Players[slot].Input = new PlayerInput(false, gamepadIdx, Players[slot].Input.Lifetime);
                    PlayerReconnected?.Invoke(slot);
                }

                else if (IsListening && IsThereAvailablePlayerSlot())
                {
                    int slot = GetNextInactivePlayerSlot();
                    Players[slot].Input = new PlayerInput(false, gamepadIdx);
                    Players[slot].IsActive = true;
                    PlayerJoined?.Invoke(slot);
                }
            }
        }

#endif
    }

    private static void CheckPlayerDrops()
    {
        for (int i = 0; i < MAX_PLAYERS; i++)
        {
            if (Players[i].IsActive && Players[i].Input.IsConnected && Players[i].Input.IsActionPressed("DropOut"))
            {
                Players[i].IsActive = false;
                PlayerDropped?.Invoke(i);
            }
        }
    }

    private static void CheckControllerDisconnects()
    {
        for (int i = 0; i < MAX_PLAYERS; i++)
        {
            if (Players[i].IsActive && Players[i].Input.IsConnected && !Players[i].Input.IsKeyboard)
            {
                if (!Raylib.IsGamepadAvailable(Players[i].Input.InputIdx))
                {
                    Players[i].Input.IsConnected = false;
                    PlayerDisconnected?.Invoke(i);
                }
            }
        }
    }

    private static void ClearDisconnectedPlayers()
    {
        for (int i = 0; i < Players.Length; i++)
        {
            PlayerSlot ps = Players[i];

            if (ps.IsActive && !ps.Input.IsConnected && !ps.Input.IsKeyboard)
            {
                ps.IsActive = false;
                PlayerDropped?.Invoke(i);
            }
        }
    }

    public static bool GetGlobalGamepadButtonDown(GamepadButton button) => Enumerable.Range(0, MAX_CONTROLLER_LISTENING)
                                                                            .Where(id => Raylib.IsGamepadAvailable(id))
                                                                            .Any(id => Raylib.IsGamepadButtonDown(id, button));

    public static bool GetGlobalGamepadButtonPressed(GamepadButton button) => Enumerable.Range(0, MAX_CONTROLLER_LISTENING)
                                                                            .Where(id => Raylib.IsGamepadAvailable(id))
                                                                            .Any(id => Raylib.IsGamepadButtonPressed(id, button));

    public static bool GetGlobalGamepadButtonReleased(GamepadButton button) => Enumerable.Range(0, MAX_CONTROLLER_LISTENING)
                                                                            .Where(id => Raylib.IsGamepadAvailable(id))
                                                                            .Any(id => Raylib.IsGamepadButtonReleased(id, button));

    public static float GetGlobalAxis(GamepadAxis axis) => float.Clamp(Enumerable.Range(0, MAX_CONTROLLER_LISTENING)
                                                                            .Where(id => Raylib.IsGamepadAvailable(id))
                                                                            .Sum(id => Raylib.GetGamepadAxisMovement(id, axis)), -1.0f, 1.0f);

    private static void BeginListening() => IsListening = true;

    private static void EndListening() => IsListening = false;

    public static int GetPlayerCount() => Players.Count(ps => ps.IsActive);

    public static int GetKeyboardPlayerCount() => Players.Count(ps => ps.IsActive && ps.Input.IsKeyboard);

    public static int GetControllerPlayerCount() => Players.Count(ps => ps.IsActive && !ps.Input.IsKeyboard);

    public static int GetConnectedControllerPlayerCount() => Players.Count(ps => ps.IsActive && !ps.Input.IsKeyboard && ps.Input.IsConnected);

    public static bool IsThereAvailablePlayerSlot() => Players.Count(ps => !ps.IsActive) > 0;

    public static int GetNextInactivePlayerSlot() => Players.First(ps => !ps.IsActive).PlayerIndex;

    public static int GetNextDisconnectedPlayerSlot()
    {
        if (Players.Count(ps => ps.IsActive && !ps.Input.IsConnected) == 0) return -1;

        return Players.First(ps => ps.IsActive && !ps.Input.IsConnected).PlayerIndex;
    }
}