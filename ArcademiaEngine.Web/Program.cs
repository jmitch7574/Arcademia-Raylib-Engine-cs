using System.Runtime.InteropServices.JavaScript;
using ArcademiaEngine.Core;
using Raylib_cs;

namespace WebApp
{
    public partial class Program
    {

        public static void Main()
        {
            Raylib.InitWindow(1280, 720, "Raylib Game - Web WASM App");
            Raylib.SetTargetFPS(60);

            Launcher.Init(new LauncherConfig { EnableImGui = false, IsWeb = true });
        }

        [JSExport]
        public static void UpdateFrame()
        {
            Launcher.Tick();
        }
    }
}