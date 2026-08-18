namespace ArcademiaEngine.Core;

using Raylib_cs;

public struct LauncherConfig
{
    public bool EnableImGui;
    public bool IsWeb;
    public bool IsArcademia;
    public string Title;
}

public static class Launcher
{
    public static LauncherConfig config { get; private set; }
    private static Viewport mainViewport;

    public static void Init(LauncherConfig config)
    {
        Raylib.SetExitKey(KeyboardKey.Null);
        InputGraphics.Init();

        Launcher.config = config;

        if (config.EnableImGui)
        {
            Inspector.Init();
        }

        mainViewport = new Viewport(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());

        SceneManager.SetScene(new RaylibLogo());
        ActionMap.Load();
        ActionMap.Save();
    }

    public static void Shutdown()
    {
        if (config.EnableImGui)
            Inspector.Shutdown();
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
        InputManager.Update();

        if (Raylib.IsWindowResized())
        {
            mainViewport = new Viewport(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
        }

        Inspector.Update();
    }

    private static void Draw()
    {
        Raylib.BeginDrawing();

        mainViewport.Begin();
        Raylib.ClearBackground(Color.RayWhite);

        SceneManager.Draw();

        mainViewport.End();

        if (Inspector.Active)
            mainViewport.Draw(new Rectangle(0, 0, mainViewport.Width - Inspector.InspectorWidth, mainViewport.Height - Inspector.ConsoleHeight));
        else
            mainViewport.Draw();


        if (config.EnableImGui)
        {
            Raylib.SetMouseScale(1, 1);
            Inspector.Draw(mainViewport.Width, mainViewport.Height);
        }

        Raylib.EndDrawing();
    }

    public static bool IsArcademia() => config.IsArcademia;
}
