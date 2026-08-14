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

    int pageOption = 0;

    public InputGraphicsPlayground()
        : base("InputGraphicsPlayground")
    {
        vp = new Viewport(640, 360);

        keyboardOptions = new()
        {
            Height = 10,
            LightUp = true,
            TextColor = Color.White,
            KeyColor = Color.Blue
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


    public void DrawArcademiaButtons()
    {
        Raylib.DrawText("Arcademia Graphics", 5, 5, 10, Color.White);
    }

    public void DrawInspector()
    {
        ImGui.SeparatorText("Keyboard");
        ImGuiEx.RaylibColorEdit("Key Colour", ref keyboardOptions.KeyColor);
        ImGuiEx.RaylibColorEdit("Text Colour", ref keyboardOptions.TextColor);

    }
}