using System.Numerics;
using System.Xml;
using ArcademiaEngine.Core;
using ArcademiaEngine.Core.Utils;
using Raylib_cs;

public class ExamplePlayerConnect : Scene
{
    PlayerConnectConfig Config;
    Viewport vp;

    // Scene constants
    const int SceneWidth = 640;
    const int SceneHeight = 360;

    const int UsableWidth = 480;
    const int UsableHeight = 220;
    const int Padding = 16;

    // Scene variables
    int rowOneCount = 0;
    int rowTwoCount = 0;

    // Input Textures
    Texture2D KeyboardMicro;
    Texture2D ControllerMicro;
    Texture2D ArcadeMicro;

    // Scene Theming
    public Color Background = new(34, 32, 52, 255);
    public Color Outline = new(63, 63, 116, 255);
    public Color TextColor = new(63, 63, 116, 255);

    public ExamplePlayerConnect() : this(new PlayerConnectConfig
    {
        MinPlayers = 1,
        MaxPlayers = 2,
        OnComplete = () => Inspector.Log("Player Connect Complete")
    })
    { }

    public ExamplePlayerConnect(PlayerConnectConfig config) : base("ExamplePlayerConnect")
    {
        Config = config;

        vp = new Viewport(SceneWidth, SceneHeight);

        if (Config.MaxPlayers > InputManager.MAX_PLAYERS)
        {
            Inspector.Error("Cannot initialise PlayerConnect with more plays than engine limit");
            SceneManager.SetScene(new RaylibLogo());
        }

        string microPath = $"{Resources.GetResourcePath()}/engine/control-icons/micro";
        KeyboardMicro = Raylib.LoadTexture($"{microPath}/keyboard_micro.png");
        ControllerMicro = Raylib.LoadTexture($"{microPath}/controller_micro.png");
        ArcadeMicro = Raylib.LoadTexture($"{microPath}/arcade_micro.png");

        InputManager.IsListening = true;
    }

    ~ExamplePlayerConnect()
    {
        Raylib.UnloadTexture(KeyboardMicro);
        Raylib.UnloadTexture(ControllerMicro);
        Raylib.UnloadTexture(ArcadeMicro);

        InputManager.IsListening = false;
    }

    public override void Draw()
    {
        InputGraphics.GlobalOptions globalOptions = new InputGraphics.GlobalOptions
        {
            Reactive = false,
            OutlineColor = Outline,
            FillColor = Raylib.ColorBrightness(Background, -0.2f),
            IconColor = TextColor,
        };

        // Declare Text Settings
        TextUtils.TextSettings textSettings = new()
        {
            FontSize = 30, // Default, overriden by most draws
            Color = TextColor,
            HorizontalAlignment = TextUtils.GuiTextAlignment.Middle,
            VerticalAlignment = TextUtils.GuiTextAlignment.Middle
        };

        TextUtils.WaveSettings waveSettings = new()
        {
            Amplitude = 5,
            Frequency = 0.5f,
            Speed = 2
        };

        vp.Begin();

        Raylib.ClearBackground(Background);

        // Top Text
        TextUtils.DrawTextAligned("Who's Playing...", new Vector2(320, 25), textSettings, waveSettings);

        // Bottom Text

        InputGraphics.KeyboardKeyOptions keyFormat = new InputGraphics.KeyboardKeyOptions
        {
            Base = globalOptions with { Height = 10 },
        };


        if (InputManager.GetPlayerCount() < Config.MinPlayers)
        {
            TextUtils.DrawTextAligned($"Minimum {Config.MinPlayers} Required", new Vector2(320, 330), textSettings, waveSettings);
        }
        else
        {
            if (Launcher.IsArcademia())
            {
                TextUtils.DrawTextAligned($"Press Start to Begin", new Vector2(320, 330), textSettings, waveSettings);
            }
            else
            {
                int keyWidth = InputGraphics.CalculateKeyWidth(KeyboardKey.Enter, keyFormat);

                InputGraphics.DrawKeyboardKey(new Vector2(320 - keyWidth, 305), KeyboardKey.Enter, out int width, keyFormat);

                InputGraphics.DrawMenuButton(new Vector2(320 + 30, 305), new InputGraphics.GamepadMenuOptions
                {
                    Base = globalOptions with { Height = 15 },
                    Button = GamepadButton.MiddleRight
                });

                TextUtils.DrawTextAligned("/", new Vector2(320, 305), textSettings with { FontSize = 20 });

                TextUtils.DrawTextAligned($"Press to Begin", new Vector2(320, 330), textSettings with { FontSize = 30 }, waveSettings);
            }
        }

        if (Config.MaxPlayers < 4)
        {
            rowOneCount = Config.MaxPlayers;
            rowTwoCount = 0;
        }
        else
        {
            rowOneCount = (int)float.Ceiling((float)Config.MaxPlayers / 2);
            rowTwoCount = (int)float.Floor((float)Config.MaxPlayers / 2);
        }

        bool isThereSecondRow = rowTwoCount > 0;

        for (int i = 0; i < Config.MaxPlayers; i++)
        {
            bool isInSecondRow = i >= rowOneCount;

            int rowIndex = i;
            if (isInSecondRow)
                rowIndex -= rowOneCount;

            int neighbours;
            if (!isInSecondRow)
                neighbours = rowOneCount;
            else
                neighbours = rowTwoCount;

            float width = ((float)UsableWidth - (Padding * (neighbours - 1))) / neighbours;
            float height = isThereSecondRow ? ((float)UsableHeight - Padding) / 2f : (float)UsableHeight;

            float x = rowIndex * (width + Padding);
            float y = isInSecondRow ? (height + Padding) : 0;

            // Center

            x += 80;
            y += 60;

            float centerX = x + width / 2;
            float centerY = y + height / 2;

            Raylib.DrawRectangleRec(new Rectangle(x, y, width, height), Background);

            TextUtils.DrawTextAligned($"{i + 1}", new Vector2(centerX, centerY - 30), textSettings);

            if (Launcher.IsArcademia())
            {
                InputGraphics.DrawArcademiaButton(new Vector2(centerX - 25, centerY + 10), new InputGraphics.ArcademiaButtonOptions
                {
                    Base = globalOptions with { Height = 15 },
                    Button = ArcademiaKeybind.P1_A
                });
                TextUtils.DrawTextAligned("Join", new Vector2(centerX - 10, centerY + 10), textSettings with { FontSize = 20 });
            }
            else
            {
                ButtonAction JoinGame = ActionMap.GetButtonAction("JoinGame");

                int nextEmptyKbSlot = Enumerable.Range(0, InputManager.MAX_KEYBOARD_PLAYERS)
                    .FirstOrDefault(kb => InputManager.Players.All(p =>
                        !p.IsActive || !p.Input.IsKeyboard || p.Input.InputIdx != kb));

                KeyboardKey nextJoinKey = JoinGame.KeyboardKeys[nextEmptyKbSlot];
                int keyWidth = InputGraphics.CalculateKeyWidth(nextJoinKey, keyFormat);

                TextUtils.DrawTextAligned("Join", new Vector2(centerX, centerY + 10), textSettings with { FontSize = 10 });

                InputGraphics.GamepadFaceOptions JoinButton = new InputGraphics.GamepadFaceOptions
                {
                    Base = globalOptions with { Height = 10 },
                    Directions = InputGraphics.AxisDirections.DOWN
                };

                if (InputManager.GetKeyboardPlayerCount() < InputManager.MAX_KEYBOARD_PLAYERS)
                {
                    InputGraphics.DrawKeyboardKey(new Vector2(centerX - keyWidth / 2 - 10, centerY + 30), nextJoinKey, out int Keywidth, keyFormat);
                    InputGraphics.DrawFaceButtons(new Vector2(centerX + 15, centerY + 30), JoinButton);
                }
                else
                {
                    InputGraphics.DrawFaceButtons(new Vector2(centerX, centerY + 30), JoinButton);
                }
            }

            // Draw Player Info if player exists
            PlayerSlot player = InputManager.Players[i];
            if (player.IsActive)
            {
                float animHeight = Easings.OutQuint(float.Clamp(player.Input.Lifetime / 2, 0, 1)) * height;
                float animWidth = Easings.OutQuint(float.Clamp(player.Input.Lifetime / 2, 0, 1)) * width;

                float startY = y + ((height / 2) - (animHeight / 2));
                float startX = x + ((width / 2) - (animWidth / 2));

                Raylib.BeginScissorMode((int)startX, (int)startY, (int)animWidth, (int)animHeight);

                Raylib.DrawRectangleRec(new Rectangle(x, y, width, height), Raylib.ColorBrightness(player.GetColour(), 0.5f));

                Texture2D targetTexture = player.Input.IsKeyboard ? KeyboardMicro : ControllerMicro;

                if (Launcher.IsArcademia())
                    targetTexture = ArcadeMicro;

                float scale = 1.5f - float.Clamp(player.Input.TimeSinceIdentifyingInput * 2.0f,
                                      0.0f, 0.5f);

                Vector2 position =
                    new(centerX - (32 * scale), centerY + 16 - (16 * scale));

                Raylib.DrawTextureEx(targetTexture, position, 0, scale, Raylib.ColorAlpha(Color.White, 0.4f));

                TextUtils.DrawTextAligned($"P{i + 1}", new Vector2(centerX, centerY), new TextUtils.TextSettings()
                {
                    FontSize = 40,
                    Color = Raylib.ColorBrightness(player.GetColour(), 0.8f),
                    HorizontalAlignment = TextUtils.GuiTextAlignment.Middle,
                    VerticalAlignment = TextUtils.GuiTextAlignment.End
                });

                Raylib.EndScissorMode();
            }

            // Draw Outline
            Raylib.DrawRectangleLinesEx(new Rectangle(x, y, width, height), 2, Outline);

            // DrawCircle(x, y, 10, RED);
            // DrawCircle(x + width, y + height, 10, GREEN);
        }

        vp.End();
        vp.Draw();
    }

    protected override void Update()
    {
#if DEBUG
        if (Raylib.IsKeyPressed(KeyboardKey.Right))
            Config.MaxPlayers++;
        if (Raylib.IsKeyPressed(KeyboardKey.Left))
            Config.MaxPlayers--;

        Config.MaxPlayers = int.Clamp(Config.MaxPlayers, 1, InputManager.MAX_PLAYERS);
#endif

        if (InputManager.GetPlayerCount() >= Config.MinPlayers && ActionMap.IsActionPressedGlobal("StartGame", false))
        {
            Config.OnComplete?.Invoke();
        }
    }
}