using System.Runtime.InteropServices.JavaScript;
using Raylib_cs;
using SharedGame;

namespace WebApp
{
    public partial class Program
    {
        private static Game game = null!;

        public static void Main()
        {
            Raylib.InitWindow(1280, 720, "Raylib Game - Web WASM App");
            Raylib.SetTargetFPS(60);

            game = new Game();
            game.Init();
        }

        [JSExport]
        public static void UpdateFrame()
        {
            game.Update();
            Raylib.BeginDrawing();
            game.Draw();
            Raylib.EndDrawing();
        }
    }
}