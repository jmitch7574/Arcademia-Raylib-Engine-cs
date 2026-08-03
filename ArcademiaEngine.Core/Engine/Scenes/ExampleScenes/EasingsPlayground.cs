using System.Numerics;
using ArcademiaEngine.Core.Utils;
using Raylib_cs;

public class EasingsPlayground : Scene
{
    Viewport vp;

    private float RealTime = 0;
    private float OscilTime = 0;

    Dictionary<string, Func<float, float>> EasingsDict = new()
    {
        { "Linear", Easings.Linear },
        { "InSine", Easings.InSine },
        { "OutSine", Easings.OutSine },
        { "InOutSine", Easings.InOutSine },
        { "InExpo", Easings.InExpo },
        { "OutExpo", Easings.OutExpo },
        { "InOutExpo", Easings.InOutExpo },
        { "InQuad", Easings.InQuad },
        { "OutQuad", Easings.OutQuad },
        { "InOutQuad", Easings.InOutQuad },
        { "InCubic", Easings.InCubic },
        { "OutCubic", Easings.OutCubic },
        { "InOutCubic", Easings.InOutCubic },
        { "InQuart", Easings.InQuart },
        { "OutQuart", Easings.OutQuart },
        { "InOutQuart", Easings.InOutQuart },
        { "InQuint", Easings.InQuint },
        { "OutQuint", Easings.OutQuint },
        { "InOutQuint", Easings.InOutQuint },
        { "InCirc", Easings.InCirc },
        { "OutCirc", Easings.OutCirc },
        { "InOutCirc", Easings.InOutCirc },
        { "InElastic", Easings.InElastic },
        { "OutElastic", Easings.OutElastic },
        { "InOutElastic", Easings.InOutElastic },
        { "InBack", Easings.InBack },
        { "OutBack", Easings.OutBack },
        { "InOutBack", Easings.InOutBack },
        { "InBounce", Easings.InBounce },
        { "OutBounce", Easings.OutBounce },
        { "InOutBounce", Easings.InOutBounce },
    };

    public EasingsPlayground()
        : base("EasingsPlayground")
    {
        vp = new Viewport(640, 360);
    }

    protected override void Update()
    {
        RealTime += Raylib.GetFrameTime();

        if ((int)RealTime == 0)
        {
            OscilTime = (RealTime % 1);
        }
        else if ((int)RealTime == 1)
        {
            OscilTime = 1 - (RealTime % 1);
        }
        else if ((int)RealTime == 2)
        {
            OscilTime = 0;
        }
        else
        {
            RealTime = 0;
        }
    }

    public override void Draw()
    {
        // There are 31 functions in Easings.cs, 30 excluding Linear
        // Linear shall be at the top, with a 5x6 grid underneath

        vp.Begin();

        Raylib.ClearBackground(Color.Black);

        Raylib.DrawText("Easings", 5, 5, 10, Color.White);

        DrawEasing("Linear", EasingsDict["Linear"], new Vector2(5, 20));

        for (int i = 1; i < EasingsDict.Count; i++)
        {
            int realIndex = i - 1;

            KeyValuePair<string, Func<float, float>> easingFunc = EasingsDict.ElementAt(i);

            int posX = 5 + (100 * (realIndex % 6));
            int posY = 50 * ((int)(realIndex / 6) + 1);

            DrawEasing(easingFunc.Key, easingFunc.Value, new Vector2(posX, posY));
        }

        vp.End();
        vp.Draw();
    }

    void DrawEasing(string easingName, Func<float, float> func, Vector2 position)
    {
        Raylib.DrawText(easingName, (int)position.X, (int)position.Y, 10, Color.White);
        Raylib.DrawLine(
            (int)position.X,
            (int)(position.Y + 25),
            (int)(position.X + 60),
            (int)(position.Y + 25),
            Color.White
        );
        Raylib.DrawCircle(
            (int)(position.X + (60 * func(OscilTime))),
            (int)(position.Y + 25),
            5,
            Color.Red
        );
    }
}
