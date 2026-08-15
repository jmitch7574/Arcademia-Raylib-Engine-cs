using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using ArcademiaEngine.Core.Utils;
using ImGuiNET;
using Raylib_cs;

public class InputGraphicsPlayground : Scene, ISceneInspector
{
    Viewport vp;

    InputGraphics.GlobalOptions globalOptions;
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
        ResetOptions();
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

    private void ResetOptions()
    {

        globalOptions = new InputGraphics.GlobalOptions
        {
            Height = 15,
            Reactive = true,
            OutlineColor = Color.Blue,
            FillColor = Color.DarkBlue,
            IconColor = Color.White,
            SpecialColor = Color.Orange
        };

        keyboardOptions = new InputGraphics.KeyboardKeyOptions
        {
            Base = globalOptions with { Height = 10 }
        };

        gamepadFaceOptions = new InputGraphics.GamepadFaceOptions
        {
            Base = globalOptions with { Height = 12 }
        };

        gamepadStickOptions = new InputGraphics.GamepadStickOptions
        {
            Base = globalOptions with { Height = 12 },
            StickLabel = 'L',
            HorizontalAxis = GamepadAxis.LeftX,
            VerticalAxis = GamepadAxis.LeftY,
            StickPressButton = GamepadButton.LeftThumb,
            StickPress = false
        };

        gamepadShoulderOptions = new InputGraphics.GamepadShoulderOptions
        {
            Base = globalOptions,
            Button = GamepadButton.LeftTrigger1
        };

        gamepadTriggerOptions = new InputGraphics.GamepadTriggerOptions
        {
            Base = globalOptions,
            Axis = GamepadAxis.LeftTrigger
        };

        gamepadMenuOptions = new InputGraphics.GamepadMenuOptions
        {
            Base = globalOptions,
            Button = GamepadButton.MiddleLeft
        };
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

        gamepadShoulderOptions.Button = GamepadButton.LeftTrigger1;
        InputGraphics.DrawShoulderButton(new Vector2(currentX, currentY), gamepadShoulderOptions);
        currentX += 45;

        gamepadShoulderOptions.Button = GamepadButton.RightTrigger1;
        InputGraphics.DrawShoulderButton(new Vector2(currentX, currentY), gamepadShoulderOptions);
        currentX += 45;

        gamepadTriggerOptions.Button = GamepadButton.LeftTrigger2;
        gamepadTriggerOptions.Axis = GamepadAxis.LeftTrigger;
        InputGraphics.DrawTriggerButton(new Vector2(currentX, currentY), gamepadTriggerOptions);
        currentX += 45;

        gamepadTriggerOptions.Button = GamepadButton.RightTrigger2;
        gamepadTriggerOptions.Axis = GamepadAxis.RightTrigger;
        InputGraphics.DrawTriggerButton(new Vector2(currentX, currentY), gamepadTriggerOptions);
        currentX += 45;

        gamepadMenuOptions.Button = GamepadButton.MiddleLeft;
        InputGraphics.DrawMenuButton(new Vector2(currentX, currentY), gamepadMenuOptions);
        currentX += 45;

        gamepadMenuOptions.Button = GamepadButton.MiddleRight;
        InputGraphics.DrawMenuButton(new Vector2(currentX, currentY), gamepadMenuOptions);
        currentX += 45;
    }

    public void DrawArcademiaButtons()
    {
        Raylib.DrawText("Arcademia Graphics", 5, 5, 10, Color.White);
    }

    private void DrawColorOptions(ref InputGraphics.GlobalOptions options)
    {
        ImGuiEx.RaylibColorEdit("Outline Color", ref options.OutlineColor);
        ImGuiEx.RaylibColorEdit("Fill Color", ref options.FillColor);
        ImGuiEx.RaylibColorEdit("Icon Color", ref options.IconColor);
        ImGuiEx.RaylibColorEdit("Special Color", ref options.SpecialColor);
    }

    public void DrawInspector()
    {
        if (ImGui.Button("Reset"))
            ResetOptions();

        ImGui.PushID("Keyboard");
        ImGui.SeparatorText("Keyboard Options");
        DrawColorOptions(ref keyboardOptions.Base);
        ImGui.PopID();

        ImGui.PushID("Face");
        ImGui.SeparatorText("Gamepad Face Options");
        DrawColorOptions(ref gamepadFaceOptions.Base);
        ImGui.PopID();

        ImGui.PushID("Stick");
        ImGui.SeparatorText("Gamepad Stick Options");
        DrawColorOptions(ref gamepadStickOptions.Base);
        string a = gamepadStickOptions.StickLabel.ToString();
        ImGui.InputText("Stick Text: ", ref a, 1);

        if (a.Length > 0)
            gamepadStickOptions.StickLabel = a.ToCharArray()[0];
        else
            gamepadStickOptions.StickLabel = ' ';
        ImGui.PopID();


        ImGui.PushID("Shoulder");
        ImGui.SeparatorText("Gamepad Shoulder Options");
        DrawColorOptions(ref gamepadShoulderOptions.Base);
        ImGui.PopID();

        ImGui.PushID("Trigger");
        ImGui.SeparatorText("Gamepad Trigger Options");
        DrawColorOptions(ref gamepadTriggerOptions.Base);
        ImGui.PopID();

        ImGui.PushID("Menu");
        ImGui.SeparatorText("Gamepad Menu Options");
        DrawColorOptions(ref gamepadMenuOptions.Base);
        ImGui.PopID();

        ImGui.Text($"{float.Clamp((InputManager.GetGlobalAxis(GamepadAxis.LeftTrigger) + 1), 0.0f, 1.0f)}");
    }
}