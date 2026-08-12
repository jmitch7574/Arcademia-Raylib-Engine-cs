using System.Numerics;
using ArcademiaEngine.Core.Utils;
using Raylib_cs;

public class InputGraphicsPlayground : Scene
{
    Viewport vp;


    public InputGraphicsPlayground()
        : base("InputGraphicsPlayground")
    {
        vp = new Viewport(640, 360);
    }

    protected override void Update()
    {
    }

    public override void Draw()
    {
        // There are 31 functions in Easings.cs, 30 excluding Linear
        // Linear shall be at the top, with a 5x6 grid underneath

        vp.Begin();

        Raylib.ClearBackground(Color.Black);

        Raylib.DrawText("Input Graphics", 5, 5, 10, Color.White);

        KeyboardKey[] values = (KeyboardKey[])Enum.GetValues(typeof(KeyboardKey));

        int currentX = 15;
        int currentY = 30;

        foreach (KeyboardKey key in values)
        {
            InputGraphics.DrawKeyboardKey(new Vector2(currentX, currentY), key, 20, out int usedWidth, true);

            currentX += usedWidth + 5;
            if (currentX > 600)
            {
                currentX = 15; currentY += 30;
            }
        }

        vp.End();
        vp.Draw();
    }

}