using System;
using ArcademiaEngine.Core;
using Raylib_cs;

namespace DesktopApp
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            bool useImGui = false;

#if DEBUG
            useImGui = true;
#endif

            const int width = 1920;
            const int height = 1080;

            // Initialize desktop window
#if RELEASE
            Raylib.InitWindow(Raylib.GetMonitorWidth(Raylib.GetCurrentMonitor()), Raylib.GetMonitorHeight(Raylib.GetCurrentMonitor()), CONTSANTS.GAME_NAME);
#else
            Raylib.InitWindow(width, height, CONTSANTS.GAME_NAME);
#endif
            Raylib.SetTargetFPS(60);

            // Initialize our shared game core
            Launcher.Init(new LauncherConfig { EnableImGui = useImGui });

            // Native blocking loop: tightly runs execution cycles
            while (!Raylib.WindowShouldClose())
            {
                Launcher.Tick();
            }

            Raylib.CloseWindow();
            Launcher.Shutdown();
        }
    }
}