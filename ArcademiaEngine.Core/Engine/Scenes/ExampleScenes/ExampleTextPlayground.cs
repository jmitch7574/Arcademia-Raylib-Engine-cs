using System.Numerics;
using System.Timers;
using ImGuiNET;
using Raylib_cs;

public class ExampleTextPlayground : Scene, ISceneInspector
{
    Viewport vp;

    bool WaveyText;

    TextUtils.TextSettings textSettings = new TextUtils.TextSettings()
    {
        FontSize = 20,
        Color = Color.White,
        HorizontalAlignment = TextUtils.GuiTextAlignment.Start,
        VerticalAlignment = TextUtils.GuiTextAlignment.Middle
    };

    TextUtils.WaveSettings waveSettings = new TextUtils.WaveSettings()
    {
        Amplitude = 5,
        Frequency = 1,
        Speed = 5,
    };

    public ExampleTextPlayground() : base("ExampleTextPlayground")
    {
        vp = new Viewport(640, 360);
    }

    public override void Draw()
    {
        vp.Begin();
        Raylib.ClearBackground(Color.Black);
        string message = "This is\nmultiline\ntext";



        // Draw Alignment Grid
        Raylib.DrawLine(320, 0, 320, 360, Color.Gray);
        Raylib.DrawLine(0, 180, 640, 180, Color.Gray);

        // Draw Text 
        var combinations = Enum.GetValues<TextUtils.GuiTextAlignment>()
            .SelectMany(c => Enum.GetValues<TextUtils.GuiTextAlignment>(), (vertical, horizontal) => (Horizontal: horizontal, Vertical: vertical));

        int currentX = 0;
        int currentY = 0;
        foreach (var (horizontal, vertical) in combinations)
        {
            textSettings.HorizontalAlignment = horizontal;
            textSettings.VerticalAlignment = vertical;

            if (WaveyText) TextUtils.DrawTextAligned(message, new Vector2(currentX, currentY), textSettings, waveSettings);
            else TextUtils.DrawTextAligned(message, new Vector2(currentX, currentY), textSettings);

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
        ImGui.Checkbox("Wavey Text enabled", ref WaveyText);
        ImGui.DragFloat("Amplitude", ref waveSettings.Amplitude, 0.1f);
        ImGui.DragFloat("Frequency", ref waveSettings.Frequency, 0.1f);
        ImGui.DragFloat("Speed", ref waveSettings.Speed, 0.1f);
    }

    protected override void Update()
    {

    }
}