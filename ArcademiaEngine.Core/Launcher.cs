namespace ArcademiaEngine.Core;

using Raylib_cs;

public struct LauncherConfig
{
    public bool EnableImGui;
    public string Title;
}

public static class Launcher
{
    private static LauncherConfig config;

    public static void Init(LauncherConfig config)
    {
        Launcher.config = config;
    }
    public static void Shutdown()
    {
    }

    public static void Tick()
    {
        Update();
        Draw();
    }

    private static void Update()
    {
    }
    private static void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.RayWhite);
        Raylib.DrawText("Hello, Raylib", 10, 10, 10, Color.Black);
        Raylib.EndDrawing();
    }
}
