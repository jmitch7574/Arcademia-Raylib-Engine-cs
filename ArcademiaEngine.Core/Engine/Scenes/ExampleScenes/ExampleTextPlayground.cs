using System.Numerics;
using System.Text.Json;
using System.Timers;
using ArcademiaEngine.Core.Engine.Text;
using ImGuiNET;
using Raylib_cs;

public class ExampleTextPlayground : Scene, ISceneInspector
{
    Viewport vp;

    bool WaveyText;

    Font jersey;

    TextSettings textSettings;

    public ExampleTextPlayground() : base("ExampleTextPlayground")
    {
        vp = new Viewport(640, 360);

        jersey = Raylib.LoadFontEx("Resources/fonts/jersey_10.ttf", 80, null, 0);

        textSettings = new TextSettings()
        {
            FontSize = 20,
            Font = jersey,
            Color = Color.White,
            HorizontalAlignment = Alignment.Start,
            VerticalAlignment = Alignment.Middle
        };
    }

    public override void Draw()
    {
        vp.Begin();
        Raylib.ClearBackground(Color.Black);
        string message = "This\nis\ntext";



        // Draw Alignment Grid
        Raylib.DrawLine(320, 0, 320, 360, Color.Gray);
        Raylib.DrawLine(0, 180, 640, 180, Color.Gray);

        // Draw Text 
        var combinations = Enum.GetValues<Alignment>()
            .SelectMany(c => Enum.GetValues<Alignment>(), (vertical, horizontal) => (Horizontal: horizontal, Vertical: vertical));

        int currentX = 0;
        int currentY = 0;
        foreach (var (horizontal, vertical) in combinations)
        {
            textSettings.HorizontalAlignment = horizontal;
            textSettings.VerticalAlignment = vertical;

            Text.DrawText(message, new Vector2(currentX, currentY), textSettings);

            currentX += 320;

            if (currentX > 640)
            {
                currentX = 0;
                currentY += 180;
            }
        }

        vp.End();
        vp.Draw();
    }

    public void DrawInspector()
    {

        ImGui.SeparatorText("Text Settings");
        ImGui.InputInt("Font Size", ref textSettings.FontSize);
    }

    protected override void Update()
    {

    }
}