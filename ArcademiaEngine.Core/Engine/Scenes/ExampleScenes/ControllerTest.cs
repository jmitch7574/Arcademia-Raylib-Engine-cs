using System.Numerics;
using ArcademiaEngine.Core.Engine.Text;
using Raylib_cs;

public class ControllerTest : Scene
{
    Viewport vp;
    Texture2D controllerTex;
    int controllerId = 0;

    InputGraphics.GlobalOptions globalOptions;
    InputGraphics.GamepadFaceOptions gamepadFaceOptions;
    InputGraphics.GamepadStickOptions gamepadStickOptions;
    InputGraphics.GamepadShoulderOptions gamepadShoulderOptions;
    InputGraphics.GamepadTriggerOptions gamepadTriggerOptions;
    InputGraphics.GamepadMenuOptions gamepadMenuOptions;

    public ControllerTest() : base("ControllerTest")
    {
        vp = new Viewport(640, 360);
        controllerTex = Raylib.LoadTexture("Resources/engine/control-icons/controller.png");

        globalOptions = new InputGraphics.GlobalOptions
        {
            Height = 25,
            Reactive = true,
            OutlineColor = Color.Blue,
            FillColor = Color.DarkBlue,
            IconColor = Color.White,
            SpecialColor = Color.White
        };

        gamepadFaceOptions = new InputGraphics.GamepadFaceOptions
        {
            Base = globalOptions
        };

        gamepadStickOptions = new InputGraphics.GamepadStickOptions
        {
            Base = globalOptions,
            StickPress = false
        };

        gamepadShoulderOptions = new InputGraphics.GamepadShoulderOptions
        {
            Base = globalOptions with { Height = 20 },
            Button = GamepadButton.LeftTrigger1
        };

        gamepadTriggerOptions = new InputGraphics.GamepadTriggerOptions
        {
            Base = globalOptions with { Height = 20 },
            Axis = GamepadAxis.LeftTrigger
        };

        gamepadMenuOptions = new InputGraphics.GamepadMenuOptions
        {
            Base = globalOptions with { Height = 20 },
            Button = GamepadButton.MiddleLeft
        };
    }

    ~ControllerTest()
    {
        Raylib.UnloadTexture(controllerTex);
    }

    public override void Draw()
    {
        int cX = 320;
        int cY = 180;

        bool validController = Raylib.IsGamepadAvailable(controllerId);

        vp.Begin();
        Raylib.ClearBackground(Color.Black);

        TextSettings textSettings = new()
        {
            FontSize = 10,
            Color = Color.White,
            HorizontalAlignment = Alignment.Middle,
            VerticalAlignment = Alignment.Middle
        };

        Text.DrawText($"Gamepad {controllerId}", new Vector2(cX, 10), textSettings);

        if (!validController)
        {
            Text.DrawText("No Controller", new Vector2(cX, 25), textSettings);
            Raylib.DrawTexture(controllerTex, 0, 0, Raylib.ColorBrightness(Color.DarkBlue, -0.5f));
            vp.End();
            vp.Draw();
            return;
        }

        Text.DrawText(Raylib.GetGamepadName_(controllerId), new Vector2(cX, 25), textSettings);


        Raylib.DrawTexture(controllerTex, 0, 0, Raylib.ColorBrightness(Color.DarkBlue, -0.25f));

        // Draw Sticks
        gamepadStickOptions.StickLabel = 'L';
        gamepadStickOptions.HorizontalAxis = GamepadAxis.LeftX;
        gamepadStickOptions.VerticalAxis = GamepadAxis.LeftY;
        gamepadStickOptions.StickPressButton = GamepadButton.LeftThumb;
        InputGraphics.DrawStick(new Vector2(cX - 75, cY + 100), gamepadStickOptions);


        gamepadStickOptions.StickLabel = 'R';
        gamepadStickOptions.HorizontalAxis = GamepadAxis.RightX;
        gamepadStickOptions.VerticalAxis = GamepadAxis.RightY;
        gamepadStickOptions.StickPressButton = GamepadButton.RightThumb;
        InputGraphics.DrawStick(new Vector2(cX + 75, cY + 100), gamepadStickOptions);

        // Draw Dpad
        InputGraphics.DrawDirectionalPad(new Vector2(cX - 100, cY + 25), gamepadFaceOptions);

        // Draw Face buttons
        InputGraphics.DrawFaceButtons(new Vector2(cX + 100, cY + 25), gamepadFaceOptions);

        // Start and Select
        gamepadMenuOptions.Button = GamepadButton.MiddleLeft;
        InputGraphics.DrawMenuButton(new Vector2(cX - 35, cY - 25), gamepadMenuOptions);
        gamepadMenuOptions.Button = GamepadButton.MiddleRight;
        InputGraphics.DrawMenuButton(new Vector2(cX + 35, cY - 25), gamepadMenuOptions);

        // Bumpers
        gamepadShoulderOptions.Button = GamepadButton.LeftTrigger1;
        InputGraphics.DrawShoulderButton(new Vector2(cX - 115, cY - 70), gamepadShoulderOptions);
        gamepadShoulderOptions.Button = GamepadButton.RightTrigger1;
        InputGraphics.DrawShoulderButton(new Vector2(cX + 115, cY - 70), gamepadShoulderOptions);

        // Triggers
        gamepadTriggerOptions.Button = GamepadButton.LeftTrigger2;
        gamepadTriggerOptions.Axis = GamepadAxis.LeftTrigger;
        InputGraphics.DrawTriggerButton(new Vector2(cX - 110, cY - 111), gamepadTriggerOptions);
        gamepadTriggerOptions.Button = GamepadButton.RightTrigger2;
        gamepadTriggerOptions.Axis = GamepadAxis.RightTrigger;
        InputGraphics.DrawTriggerButton(new Vector2(cX + 110, cY - 111), gamepadTriggerOptions);

        vp.End();
        vp.Draw();
    }

    protected override void Update()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Right))
        {
            controllerId++;
        }
        if (Raylib.IsKeyPressed(KeyboardKey.Left))
        {
            controllerId--;
        }

        controllerId = (controllerId + InputManager.MAX_CONTROLLER_LISTENING) % InputManager.MAX_CONTROLLER_LISTENING;
    }
}