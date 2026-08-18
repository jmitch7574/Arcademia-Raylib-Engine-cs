using System.Numerics;
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

        vp.Begin();

        Raylib.ClearBackground(Background);

        // Top Text
        TextUtils.DrawTextAligned("Who's Playing...", 30, new Vector2(320, 25),
            TextUtils.GuiTextAlignment.Center,
            TextUtils.GuiTextAlignmentVertical.Middle,
            TextColor);

        // Bottom Text

        if (InputManager.GetPlayerCount() < Config.MinPlayers)
        {
            TextUtils.DrawTextAligned($"Minimum {Config.MinPlayers} Required", 30, new Vector2(320, 330),
                TextUtils.GuiTextAlignment.Center,
                TextUtils.GuiTextAlignmentVertical.Middle,
                TextColor);
        }
        else
        {
#if ARCADEMIA
            TextUtils.DrawTextAligned($"Press Start to Begin", 30, new Vector2(320, 330),
                TextUtils.GuiTextAlignment.Center,
                TextUtils.GuiTextAlignmentVertical.Middle,
                TextColor);
#else
            InputGraphics.KeyboardKeyOptions key = new InputGraphics.KeyboardKeyOptions
            {
                Base = globalOptions with { Height = 10 },
            };

            int keyWidth = InputGraphics.CalculateKeyWidth(KeyboardKey.Enter, key);

            InputGraphics.DrawKeyboardKey(new Vector2(320 - keyWidth, 305), KeyboardKey.Enter, out int width, key);

            InputGraphics.DrawMenuButton(new Vector2(320 + 30, 305), new InputGraphics.GamepadMenuOptions
            {
                Base = globalOptions with { Height = 15 },
                Button = GamepadButton.MiddleRight
            });

            TextUtils.DrawTextAligned("/", 20, new Vector2(320, 305), TextUtils.GuiTextAlignment.Center, TextUtils.GuiTextAlignmentVertical.Middle, TextColor);

            TextUtils.DrawTextAligned($"Press to Begin", 30, new Vector2(320, 330),
                TextUtils.GuiTextAlignment.Center,
                TextUtils.GuiTextAlignmentVertical.Middle,
                TextColor);
#endif

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

            TextUtils.DrawTextAligned($"{i + 1}", 40, new Vector2(centerX, centerY - 20),
                            TextUtils.GuiTextAlignment.Center, TextUtils.GuiTextAlignmentVertical.Middle,
                            Outline);

            // Draw Player Info if player exists
            PlayerSlot player = InputManager.Players[i];
            if (player.IsActive)
            {
                float animHeight = Easings.OutSine(float.Clamp(player.Input.Lifetime, 0, 1)) * height;

                Raylib.BeginScissorMode((int)x, (int)y, (int)width, (int)animHeight);

                Raylib.DrawRectangleRec(new Rectangle(x, y, width, height), Raylib.ColorBrightness(player.GetColour(), 0.5f));

                TextUtils.DrawTextAligned($"P{i + 1}", 20, new Vector2(centerX, y + 30), TextUtils.GuiTextAlignment.Center, TextUtils.GuiTextAlignmentVertical.Middle, Raylib.ColorBrightness(player.GetColour(), 0.8f));

                Texture2D targetTexture = player.Input.IsKeyboard ? KeyboardMicro : ControllerMicro;

#if ARCADEMIA
                targetTexture = ArcadeMicro;
#endif

                float scale = 1.5f - float.Clamp(player.Input.TimeSinceIdentifyingInput * 2.0f,
                                      0.0f, 0.5f);

                Vector2 position =
                    new(centerX - (32 * scale), centerY - (16 * scale) + 16);

                Raylib.DrawTextureEx(targetTexture, position, 0, scale, Raylib.ColorAlpha(Color.White, 0.4f));

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