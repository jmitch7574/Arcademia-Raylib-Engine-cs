using Raylib_cs;

class RaylibLogo : Scene
{
    private Color BackgroundColor = Color.Black;
    private Color ForegroundColor = Color.White;

    private readonly Viewport Viewport = new(640, 360);

    private int logoPositionX;
    private readonly int logoPositionY;

    private float timer = 0;
    private int letterCount = 0;

    private int topSideRecWidth = 16;
    private int leftSideRecHeight = 16;

    private int bottomSideRecWidth = 16;
    private int rightSideRecHeight = 16;

    int state = 0;
    float alpha = 1.0f;



    public RaylibLogo() : base("Raylib Logo")
    {
        logoPositionX = Viewport.Width / 2 - 128;
        logoPositionY = Viewport.Height / 2 - 128;
    }


    protected override void Update()
    {
        timer += Raylib.GetFrameTime() * 2.0f;
        if (state == 0)
        {
            if (timer >= 2)
            {
                timer = 0;
                state = 1;
            }
        }
        else if (state == 1)
        {
            topSideRecWidth = Math.Max((int)(timer * 128), 16);
            leftSideRecHeight = Math.Max((int)(timer * 128), 16);

            if (timer >= 2)
            {
                timer = 0;
                state = 2;
                topSideRecWidth = 256;
                leftSideRecHeight = 256;
            }
        }
        else if (state == 2)
        {
            bottomSideRecWidth = (int)(timer * 128);
            rightSideRecHeight = (int)(timer * 128);

            if (timer >= 2)
            {
                timer = 0;
                state = 3;

                bottomSideRecWidth = 256;
                rightSideRecHeight = 256;
            }
        }
        else if (state == 3)
        {
            letterCount = (int)Math.Floor(timer * 5);

            if (letterCount >= 10)
            {
                state = 4;
                timer = 0;
            }
        }
        else if (state == 5)
        {
            float originalX = Viewport.Width / 2 - 128;

            logoPositionX = (int)(originalX - 140 * (float)Math.Sqrt(1 - Math.Pow(Math.Min(timer / 2, 1) - 1, 2)));
        }
        else if (state == 4)
        {
            alpha = 1.0f - ((timer - 5) / 2);
        }
    }

    public override void Draw()
    {
        Raylib.ClearBackground(BackgroundColor);
        Viewport.Begin();

        if (state == 0)
        {
            if ((int)(timer * 4) % 2 == 0)
            {
                Raylib.DrawRectangle(logoPositionX, logoPositionY, 16, 16, ForegroundColor);
            }
        }

        if (state >= 1)
        {
            Raylib.DrawRectangle(logoPositionX, logoPositionY, topSideRecWidth, 16, ForegroundColor);
            Raylib.DrawRectangle(logoPositionX, logoPositionY, 16, leftSideRecHeight, ForegroundColor);
        }

        if (state >= 2)
        {
            Raylib.DrawRectangle(logoPositionX, logoPositionY + 240, bottomSideRecWidth, 16, ForegroundColor);
            Raylib.DrawRectangle(logoPositionX + 240, logoPositionY, 16, rightSideRecHeight, ForegroundColor);
        }

        if (state >= 3)
        {
            Raylib.DrawText("raylib"[..Math.Min(letterCount, 6)], logoPositionX + 84, logoPositionY + 176, 50, ForegroundColor);
        }

        Viewport.End();
        Viewport.Draw(null, Raylib.Fade(Color.White, alpha));

    }

}