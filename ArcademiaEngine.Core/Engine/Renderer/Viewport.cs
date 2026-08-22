using System.Numerics;
using Raylib_cs;

public class Viewport
{
    private RenderTexture2D RenderTexture;

    public int Width => RenderTexture.Texture.Width;
    public int Height => RenderTexture.Texture.Height;

    protected static Stack<Viewport> activeViewports = [];

    public bool Drawing
    {
        get
        {
            return Rlgl.GetActiveFramebuffer() == (uint)RenderTexture.Id;
        }
    }

    public Viewport(int width, int height)
    {
        RenderTexture = Raylib.LoadRenderTexture(width, height);
    }

    ~Viewport()
    {
        Raylib.UnloadRenderTexture(RenderTexture);
    }

    public void Begin(bool clear = true)
    {
        activeViewports.Push(this);
        Raylib.BeginTextureMode(RenderTexture);
        if (clear) Raylib.ClearBackground(Color.Blank);

        Raylib.SetMouseScale(Raylib.GetScreenWidth() / Width, Raylib.GetScreenHeight() / Height);
    }


    public void Clear()
    {
        Raylib.BeginTextureMode(RenderTexture);
        Raylib.ClearBackground(Color.Blank);
        Raylib.EndTextureMode();
        Restore();
    }

    public void End()
    {
        Raylib.EndTextureMode();

        if (activeViewports.Count > 0)
            activeViewports.Pop();

        Restore();
    }

    public void Draw(Rectangle? rec = null, Color? col = null)
    {
        Vector2 size = GetCurrentViewportSize();
        rec ??= new Rectangle(0, 0, size.X, size.Y);
        col ??= Color.White;

        Raylib.DrawTexturePro(
            RenderTexture.Texture,
            new Rectangle(0, 0, Width, -Height),
            (Rectangle)rec,
            new Vector2(0, 0),
            0,
            (Color)col
        );
    }

    public static void Restore()
    {
        if (activeViewports.Count > 0)
            Raylib.BeginTextureMode(activeViewports.Peek().RenderTexture);
    }

    public static Viewport GetCurrentViewport()
    {
        return activeViewports.Peek();
    }

    public static Vector2 GetCurrentViewportSize()
    {
        if (activeViewports.Count == 0) return new Vector2(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
        return new Vector2(activeViewports.Peek().Width, activeViewports.Peek().Height);
    }

    public List<Rectangle> Divide(int screens)
    {
        int amountX = (int)Math.Ceiling(Math.Sqrt(screens));
        int amountY = (int)Math.Ceiling((float)screens / (float)amountX);

        float sizeX = Width / amountX;
        float sizeY = Height / amountY;

        List<Rectangle> rectangles = [];

        for (int i = 0; i < screens; i++)
        {
            int rowIdx = i % amountX;
            int colIdx = (int)Math.Floor((float)i / (float)amountX);

            rectangles.Add(new Rectangle(rowIdx * sizeX, colIdx * sizeY, sizeX, sizeY));
        }

        return rectangles;
    }
}