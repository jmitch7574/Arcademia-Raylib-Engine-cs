using System.Globalization;
using System.Numerics;
using System.Text.Json;
using System.Timers;
using ArcademiaEngine.Core.Engine.Text;
using ImGuiNET;
using Raylib_cs;

public class ExampleTextEffects : Scene, ISceneInspector
{
    Viewport vp;

    Font Jersey;

    TextSettings LeftTextSettings;
    TextSettings RightTextSettings;

    private const float TransitionDurations = 10.0f;

    private readonly Dictionary<string, string> Items = new()
    {
        {"Color:", """<color value="#ff0000">red</color> <color value="#00ff00">green</color> <color value="#0000ff">blue</color>"""},
        {"Fade:", $"""<fade InTime="0" OutTime="{TransitionDurations}">Let there be fading</fade>"""},
        {"Glow:", $"""<glow>Shine!!! <color value="#ff0000">all the colours!!!</color></glow>"""},
        {"Item:", """Bring me <item>five billion monies</item>"""},
        {"Rainbow:", """<rainbow>rainbows!!!</rainbow> and <rainbow Speed="0.5" Sat="0.2" Val="0.8">pastel rainbows!!!!</rainbow>"""},
        {"Sparkle:", """<sparkle Freq="10" Colors="#d50000, #a04800, #570000">Villains</sparkle> and <sparkle Freq="10" Colors="#000f84, #00aeff, #00e1ff">Heroes</sparkle>"""},
        {"Wave:", "<wave>im waving wave back</wave>"},
        {"Everything:", """<rainbow Speed="0.5" Sat="0.2" Val="0.8"><wave><glow>The Everything Text</glow></wave></rainbow>"""},
    };

    readonly List<RichText> TextObjects = [];

    public ExampleTextEffects() : base("ExampleTextEffects")
    {
        vp = new Viewport(1280, 720);

        Jersey = Raylib.LoadFontEx("Resources/fonts/jersey_10.ttf", 80, null, 0);

        LeftTextSettings = new TextSettings()
        {
            FontSize = 20,
            Font = Jersey,
            Color = Color.Gray,
            HorizontalAlignment = Alignment.End,
            VerticalAlignment = Alignment.Middle,
            GlowStrength = 2.0f
        };
        RightTextSettings = LeftTextSettings with { Color = Color.White, HorizontalAlignment = Alignment.Start };

        int currentY = 30;

        int alignLeft = 150;
        int alignRight = 160;

        void Increment() => currentY += 45;
        Vector2 GetPosLeft() => new(alignLeft, currentY);
        Vector2 GetPosRight() => new(alignRight, currentY);

        foreach (KeyValuePair<string, string> effect in Items)
        {
            TextObjects.Add(new(effect.Key, GetPosLeft(), LeftTextSettings));
            TextObjects.Add(new(effect.Value, GetPosRight(), RightTextSettings, vp));
            Increment();
        }


    }

    public override void Draw()
    {
        vp.Begin();
        Raylib.ClearBackground(Color.Black);

        foreach (RichText textObject in TextObjects)
        {
            textObject.Draw();
            textObject.Lifetime %= TransitionDurations;
        }

        vp.End();
        vp.Draw();
    }

    public void DrawInspector()
    {

    }

    protected override void Update()
    {

    }
}