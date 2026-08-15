using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using ArcademiaEngine.Core.Utils;
using ImGuiNET;
using Raylib_cs;

public class InputGraphicsPlayground : Scene, ISceneInspector
{
    Viewport vp;

    InputGraphics.KeyboardKeyOptions keyboardOptions;
    InputGraphics.GamepadFaceOptions gamepadFaceOptions;
    InputGraphics.GamepadStickOptions gamepadStickOptions;
    InputGraphics.GamepadShoulderOptions gamepadShoulderOptions;
    InputGraphics.GamepadTriggerOptions gamepadTriggerOptions;
    InputGraphics.GamepadMenuOptions gamepadMenuOptions;

    int pageOption = 0;

    public InputGraphicsPlayground()
        : base("InputGraphicsPlayground")
    {
        vp = new Viewport(640, 360);

        keyboardOptions = new()
        {
            Height = 10,
            Reactive = true,
            TextColor = Color.White,
            KeyColor = Color.Blue
        };

        gamepadFaceOptions = new()
        {
            Height = 12,
            OutlineColor = Color.Blue,
            InactiveColor = Color.DarkBlue,
            ActiveColor = Color.White,
            Reactive = true,
            PressedColor = Color.Orange,
        };

        gamepadStickOptions = new()
        {
            Height = 12,
            OutlineColor = Color.Blue,
            FillColor = Color.DarkBlue,
            TextColor = Color.White,
            StickLabel = 'L',
            HorizontalAxis = GamepadAxis.LeftX,
            VerticalAxis = GamepadAxis.LeftY,
            StickPressButton = GamepadButton.LeftThumb,
            StickPress = false,
            Reactive = true
        };

        gamepadShoulderOptions = new()
        {
            Height = 15,
            OutlineColor = Color.Blue,
            FillColor = Color.DarkBlue,
            TextColor = Color.White,
            Reactive = true
        };

        gamepadTriggerOptions = new()
        {
            Height = 15,
            OutlineColor = Color.Blue,
            FillColor = Color.DarkBlue,
            TextColor = Color.White,
            Reactive = true
        };

        gamepadMenuOptions = new()
        {
            Height = 15,
            OutlineColor = Color.Blue,
            FillColor = Color.DarkBlue,
            IconColor = Color.White,
            Reactive = true
        };
    }

    protected override void Update()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Right)) pageOption++;
        if (Raylib.IsKeyPressed(KeyboardKey.Left)) pageOption--;

        pageOption = Math.Clamp(pageOption, 0, 2);
    }

    public override void Draw()
    {
        vp.Begin();

        Raylib.ClearBackground(Color.Black);

        if (pageOption == 0) DrawKeyboardPage();
        if (pageOption == 1) DrawControllerButtons();
        if (pageOption == 2) DrawArcademiaButtons();

        vp.End();
        vp.Draw();
    }

    public void DrawKeyboardPage()
    {
        Raylib.DrawText("Keyboard Graphics", 5, 5, 10, Color.White);

        KeyboardKey[] values = (KeyboardKey[])Enum.GetValues(typeof(KeyboardKey));

        int currentX = 15;
        int currentY = 30;

        foreach (KeyboardKey key in values)
        {
            InputGraphics.DrawKeyboardKey(new Vector2(currentX, currentY), key, out int usedWidth, keyboardOptions);

            currentX += usedWidth + 5;
            if (currentX > 550)
            {
                currentX = 15; currentY += 30;
            }
        }
    }

    public void DrawControllerButtons()
    {
        Raylib.DrawText("Controller Graphics", 5, 5, 10, Color.White);

        int maxBitmask = Enum.GetValues(typeof(InputGraphics.AxisDirections))
                             .Cast<int>()
                             .Aggregate(0, (acc, val) => acc | val);

        InputGraphics.AxisDirections[] allCombinations = Enumerable.Range(0, maxBitmask + 1)
                                                     .Select(i => (InputGraphics.AxisDirections)i)
                                                     .ToArray();

        int currentX = 25;
        int currentY = 40;

        foreach (var flag in allCombinations)
        {
            gamepadFaceOptions.Directions = flag;
            InputGraphics.DrawFaceButtons(new Vector2(currentX, currentY), gamepadFaceOptions);

            currentX += 45;
            if (currentX > 550)
            {
                currentX = 25; currentY += 45;
            }
        }


        currentX = 25;
        currentY += 45;

        foreach (var flag in allCombinations)
        {
            gamepadFaceOptions.Directions = flag;
            InputGraphics.DrawDirectionalPad(new Vector2(currentX, currentY), gamepadFaceOptions);

            currentX += 45;
            if (currentX > 550)
            {
                currentX = 25; currentY += 45;
            }
        }


        currentX = 25;
        currentY += 45;

        gamepadStickOptions.StickPress = false;
        foreach (var flag in allCombinations)
        {
            gamepadStickOptions.Directions = flag;
            InputGraphics.DrawStick(new Vector2(currentX, currentY), gamepadStickOptions);

            currentX += 45;
            if (currentX > 550)
            {
                currentX = 25; currentY += 45;
            }
        }

        gamepadStickOptions.StickPress = true;
        InputGraphics.DrawStick(new Vector2(currentX, currentY), gamepadStickOptions);


        currentX = 25; currentY += 45;

        gamepadShoulderOptions.Button = InputGraphics.ShoulderButton.BUMPER_LEFT;
        InputGraphics.DrawShoulderButton(new Vector2(currentX, currentY), gamepadShoulderOptions);
        currentX += 45;

        gamepadShoulderOptions.Button = InputGraphics.ShoulderButton.BUMPER_RIGHT;
        InputGraphics.DrawShoulderButton(new Vector2(currentX, currentY), gamepadShoulderOptions);
        currentX += 45;

        gamepadTriggerOptions.Button = InputGraphics.TriggerButton.TRIGGER_LEFT;
        gamepadTriggerOptions.Axis = InputGraphics.TriggerAxis.TRIGGER_LEFT;
        InputGraphics.DrawTriggerButton(new Vector2(currentX, currentY), gamepadTriggerOptions);
        currentX += 45;

        gamepadTriggerOptions.Button = InputGraphics.TriggerButton.TRIGGER_RIGHT;
        gamepadTriggerOptions.Axis = InputGraphics.TriggerAxis.TRIGGER_RIGHT;
        InputGraphics.DrawTriggerButton(new Vector2(currentX, currentY), gamepadTriggerOptions);
        currentX += 45;

        gamepadMenuOptions.Button = InputGraphics.MenuButton.LEFT;
        InputGraphics.DrawMenuButton(new Vector2(currentX, currentY), gamepadMenuOptions);
        currentX += 45;

        gamepadMenuOptions.Button = InputGraphics.MenuButton.RIGHT;
        InputGraphics.DrawMenuButton(new Vector2(currentX, currentY), gamepadMenuOptions);
        currentX += 45;
    }

    public void DrawArcademiaButtons()
    {
        Raylib.DrawText("Arcademia Graphics", 5, 5, 10, Color.White);
    }

    public void DrawInspector()
    {
        ImGui.SeparatorText("Keyboard");
        ImGuiEx.RaylibColorEdit("Key Colour", ref keyboardOptions.KeyColor);
        ImGuiEx.RaylibColorEdit("Text Colour", ref keyboardOptions.TextColor);

        ImGui.SeparatorText("Gamepad Face Buttons");
        ImGuiEx.RaylibColorEdit("Active Button Color", ref gamepadFaceOptions.ActiveColor);
        ImGuiEx.RaylibColorEdit("Inactive Button Color", ref gamepadFaceOptions.InactiveColor);
        ImGuiEx.RaylibColorEdit("Outline Color", ref gamepadFaceOptions.OutlineColor);

        ImGui.SeparatorText("Gamepad Stick Options");
        ImGuiEx.RaylibColorEdit("Stick Fill Color", ref gamepadStickOptions.FillColor);
        ImGuiEx.RaylibColorEdit("Stick Outline Color", ref gamepadStickOptions.OutlineColor);
        ImGuiEx.RaylibColorEdit("Stick Text Color", ref gamepadStickOptions.TextColor);

        string a = gamepadStickOptions.StickLabel.ToString();
        ImGui.InputText("Stick Text Color", ref a, 1);

        if (a.Length > 0)
            gamepadStickOptions.StickLabel = a.ToCharArray()[0];
        else
            gamepadStickOptions.StickLabel = ' ';

        ImGui.Text($"{float.Clamp((InputManager.GetGlobalAxis(GamepadAxis.LeftTrigger) + 1), 0.0f, 1.0f)}");
    }
}