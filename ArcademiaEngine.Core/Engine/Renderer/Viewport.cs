using System.Numerics;
using Raylib_cs;

class Viewport
{
    protected RenderTexture2D RenderTexture;

    public int Width => RenderTexture.Texture.Width;
    public int Height => RenderTexture.Texture.Height;

    protected static Stack<Viewport> activeViewports = [];

    public Viewport(int width, int height)
    {
        RenderTexture = Raylib.LoadRenderTexture(width, height);
    }

    ~Viewport()
    {
        Raylib.UnloadRenderTexture(RenderTexture);
    }

    public void Begin()
    {
        activeViewports.Push(this);
        Raylib.BeginTextureMode(RenderTexture);
        Raylib.ClearBackground(Color.Blank);

        Raylib.SetMouseScale(Raylib.GetScreenWidth() / Width, Raylib.GetScreenHeight() / Height);
    }

    public void End()
    {
        Raylib.EndTextureMode();

        if (activeViewports.Count > 0) activeViewports.Pop();

        Restore();
    }

    public void Draw(Rectangle? rec = null, Color? col = null)
    {
        rec ??= new Rectangle(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
        col ??= Color.White;

        Raylib.DrawTexturePro(RenderTexture.Texture, new Rectangle(0, 0, Width, -Height), (Rectangle)rec, new Vector2(0, 0), 0, (Color)col);
    }

    public static void Restore()
    {
        if (activeViewports.Count > 0)
            Raylib.BeginTextureMode(activeViewports.Last().RenderTexture);
    }
}