using System.Numerics;
using Raylib_cs;

/// Split Screen Example - Arcademia Engine
/// 
/// This scene shows how dynamic and automatic splitscreening can be accomplished using RenderTextures and the custom input system.
/// The scene automatically recalculates screen divisions and placements anytime a player joins or leaves.


public class SplitscreenExample : Scene
{

    /// <summary>
    /// A class to hold player data
    /// </summary>
    private class SplitscreenExamplePlayer
    {
        public SplitscreenExamplePlayer()
        {
            position = new(200, 200);
            camera = new Camera2D();
            camera.Zoom = 1;
        }
        public Vector2 position;
        public RenderTexture2D rt;
        public Rectangle divide;
        public Camera2D camera;
    }

    Viewport viewport = new Viewport(640, 360);

    const int LEVEL_WIDTH = 100;
    const int LEVEL_HEIGHT = 100;
    const int TILE_SIZE = 16;

    List<SplitscreenExamplePlayer> players;
    int previousPlayerCount = -1;

    public SplitscreenExample() : base("SplitscreenExample")
    {
        // Initialise our players
        players = [];
        for (int i = 0; i < InputManager.MAX_PLAYERS; i++)
        {
            players.Add(new SplitscreenExamplePlayer());
        }

        // Set input to listening so players can join
        InputManager.IsListening = true;
    }

    protected override void Update()
    {
        // Begin tracking player count
        int activePlayerCount = 0;

        // Iterate over all possible player slots
        for (int i = 0; i < InputManager.MAX_PLAYERS; i++)
        {
            PlayerSlot input = InputManager.Players[i];

            // Skip non-active players
            if (input.IsActive)
            {
                activePlayerCount++;

                players[i].position += input.Input.GetActionVector2("Movement") * 32f * Raylib.GetFrameTime();                  // Move player
                players[i].camera.Target = new Vector2((int)players[i].position.X, (int)players[i].position.Y);                 // Move camera
                players[i].camera.Offset = new Vector2((int)players[i].divide.Width / 2, (int)players[i].divide.Height / 2);    // Center camera
            }
        }

        if (activePlayerCount == 0) return; // If there are no players then don't bother splitting screen

        // Reupdate screen splits if player count changes
        if (previousPlayerCount != activePlayerCount)
        {

            // Get viewport divides
            List<Rectangle> divides = viewport.Divide(activePlayerCount);

            // Track playerslots and divides separately
            int index = 0;
            int assignments = 0;

            // While there are still divides to be assigned
            while (assignments < divides.Count)
            {
                // Reset everyone's render texture
                Raylib.UnloadRenderTexture(players[index].rt);
                players[index].rt = new RenderTexture2D();

                // Don't assign divides to inactive players
                if (!InputManager.Players[index].IsActive)
                {
                    index++;
                    continue;
                }

                // Assign divide and create new render texture with divide's dimensions
                players[index].rt = Raylib.LoadRenderTexture((int)divides[0].Width, (int)divides[0].Height);
                players[index].divide = divides[assignments];

                index++;
                assignments++;
            }
        }

        previousPlayerCount = activePlayerCount;
    }

    public override void Draw()
    {
        // Don't draw if there are no players
        if (previousPlayerCount == 0) return;

        // Iterate over all player slots
        for (int i = 0; i < InputManager.MAX_PLAYERS; i++)
        {
            // Don't draw anything for inactive players
            if (!InputManager.Players[i].IsActive)
            {
                continue;
            }

            // Begin drawing in active player's render texture
            Raylib.BeginTextureMode(players[i].rt);
            Raylib.ClearBackground(Color.White);
            Raylib.BeginMode2D(players[i].camera);

            // Draw Grid
            for (int x = 0; x < LEVEL_WIDTH; x++)
            {
                Raylib.DrawLineEx(new Vector2(x * TILE_SIZE, 0),
                           new Vector2(x * TILE_SIZE, LEVEL_HEIGHT * TILE_SIZE), 2, Color.Gray);
            }
            for (int y = 0; y < LEVEL_WIDTH; y++)
            {
                Raylib.DrawLineEx(new Vector2(0, y * TILE_SIZE),
                           new Vector2(LEVEL_WIDTH * TILE_SIZE, y * TILE_SIZE), 2, Color.Gray);
            }

            // Draw Player
            Raylib.DrawRectangle((int)players[i].position.X, (int)players[i].position.Y, TILE_SIZE, TILE_SIZE, InputManager.Players[i].GetColour());
            Raylib.EndMode2D();

            // Draw outline and player number along divide border
            Raylib.DrawRectangleLinesEx(new Rectangle(0, 0, (int)players[i].divide.Width, (int)players[i].divide.Height), 5, InputManager.Players[i].GetColour());
            Raylib.DrawText($"P{i + 1}", 10, 10, 30, InputManager.Players[i].GetColour());

            Raylib.EndTextureMode();
        }

        // Draw divides to scene's viewport
        viewport.Begin();
        Raylib.ClearBackground(Color.Black);
        for (int i = 0; i < InputManager.MAX_PLAYERS; i++)
        {
            if (!InputManager.Players[i].IsActive)
            {
                continue;
            }

            Raylib.DrawTexturePro(players[i].rt.Texture, new Rectangle(0, 0, players[i].divide.Width, -players[i].divide.Height), players[i].divide, Vector2.Zero, 0, Color.White);
        }
        viewport.End();
        viewport.Draw();
    }
}