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
    private static Viewport mainViewport;

    public static void Init(LauncherConfig config)
    {
        Launcher.config = config;
        mainViewport = new Viewport(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
        SceneManager.SetScene(new RaylibLogo());
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
        SceneManager.SwapScene();
        SceneManager.Update();
        if (Raylib.IsWindowResized())
        {
            mainViewport = new Viewport(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
        }
    }
    private static void Draw()
    {
        Raylib.BeginDrawing();

        mainViewport.Begin();
        Raylib.ClearBackground(Color.RayWhite);

        SceneManager.Draw();

        mainViewport.End();
        mainViewport.Draw();
        Raylib.EndDrawing();
    }
}
