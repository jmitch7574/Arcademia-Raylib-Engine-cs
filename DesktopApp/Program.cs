using System;
using Raylib_cs;
using SharedGame;

namespace DesktopApp
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            const int width = 1280;
            const int height = 720;

            // Initialize desktop window
            Raylib.InitWindow(width, height, "Raylib Game - Desktop App");
            Raylib.SetTargetFPS(60);

            // Initialize our shared game core
            Game game = new Game();
            game.Init();

            // Native blocking loop: tightly runs execution cycles
            while (!Raylib.WindowShouldClose())
            {
                game.Update();

                Raylib.BeginDrawing();
                game.Draw();
                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }
    }
}