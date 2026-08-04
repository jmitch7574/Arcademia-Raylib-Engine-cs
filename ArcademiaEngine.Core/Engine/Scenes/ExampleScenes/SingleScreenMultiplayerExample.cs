using System.Numerics;
using Raylib_cs;

/// Split Screen Example - Arcademia Engine
/// 
/// This scene shows how dynamic and automatic splitscreening can be accomplished using RenderTextures and the custom input system.
/// The scene automatically recalculates screen divisions and placements anytime a player joins or leaves.


public class SingleScreenMultiplayerExample : Scene
{

    /// <summary>
    /// A class to hold player data
    /// </summary>
    private class SplitscreenExamplePlayer
    {
        public SplitscreenExamplePlayer()
        {
            position = new(200, 200);
        }
        public Vector2 position;
    }

    Viewport viewport = new Viewport(640, 360);

    const int LEVEL_WIDTH = 100;
    const int LEVEL_HEIGHT = 100;
    const int TILE_SIZE = 16;
    Camera2D camera;

    List<SplitscreenExamplePlayer> players;
    int previousPlayerCount = -1;

    public SingleScreenMultiplayerExample() : base("SingleScreenMultiplayerExample")
    {
        // Initialise our players
        players = [];
        for (int i = 0; i < InputManager.MAX_PLAYERS; i++)
        {
            players.Add(new SplitscreenExamplePlayer());
        }

        // Set input to listening so players can join
        InputManager.IsListening = true;

        InputManager.PlayerJoined += OnPlayerJoin;

        camera = new Camera2D(new Vector2(320, 180), new Vector2(200, 200), 0, 1);
    }

    ~SingleScreenMultiplayerExample()
    {
        InputManager.PlayerJoined -= OnPlayerJoin;
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
                players[i].position += input.Input.GetActionVector2("Movement") * 256.0f * Raylib.GetFrameTime();  // Move player
                activePlayerCount++;
            }
        }

        previousPlayerCount = activePlayerCount;

        // Fit Camera to Player bounds
        if (InputManager.GetPlayerCount() == 0) return;

        float minX = float.PositiveInfinity;
        float maxX = float.NegativeInfinity;
        float minY = float.PositiveInfinity;
        float maxY = float.NegativeInfinity;

        for (int i = 0; i < InputManager.MAX_PLAYERS; i++)
        {
            if (!InputManager.Players[i].IsActive) continue;

            minX = Math.Min(minX, players[i].position.X);
            maxX = Math.Max(maxX, players[i].position.X);
            minY = Math.Min(minY, players[i].position.Y);
            maxY = Math.Max(maxY, players[i].position.Y);
        }

        float dx = maxX - minX;
        float dy = maxY - minY;

        // Screen Padding
        float padding = 64f;
        dx += padding * 2;
        dy += padding * 2;

        // Center of the player bounding box in world space
        camera.Target = new Vector2(minX + (maxX - minX) / 2f, minY + (maxY - minY) / 2f);

        camera.Offset = new Vector2(viewport.Width / 2f, viewport.Height / 2f);

        // Calculate required zoom to fit screen bounds
        float zoomX = viewport.Width / dx;
        float zoomY = viewport.Height / dy;
        float targetZoom = Math.Min(zoomX, zoomY);

        // Clamp Zoom
        float minZoom = 0.5f;
        float maxZoom = 2.0f;
        camera.Zoom = Math.Clamp(targetZoom, minZoom, maxZoom);

        // Keep players on screen if the camera is fully zoomed out
        float zoomEpsilon = 0.001f;
        if (camera.Zoom <= minZoom + zoomEpsilon)
        {
            float halfScreenWidth = (viewport.Width / 2f) / camera.Zoom;
            float halfScreenHeight = (viewport.Height / 2f) / camera.Zoom;

            float visibleMinX = camera.Target.X - halfScreenWidth;
            float visibleMaxX = camera.Target.X + halfScreenWidth - 32f;
            float visibleMinY = camera.Target.Y - halfScreenHeight;
            float visibleMaxY = camera.Target.Y + halfScreenHeight - 32f;

            // 3. Clamp all active players within the visible bounds
            for (int i = 0; i < InputManager.MAX_PLAYERS; i++)
            {
                if (!InputManager.Players[i].IsActive) continue;

                var player = players[i];

                player.position.X = Math.Clamp(player.position.X, visibleMinX, visibleMaxX);
                player.position.Y = Math.Clamp(player.position.Y, visibleMinY, visibleMaxY);
            }
        }

    }

    public override void Draw()
    {
        // Don't draw if there are no players
        if (previousPlayerCount == 0) return;

        viewport.Begin();
        Raylib.ClearBackground(Color.Black);
        Raylib.BeginMode2D(camera);


        // Draw Grid
        for (int x = 0; x <= LEVEL_WIDTH; x++)
        {
            Raylib.DrawLineEx(new Vector2(x * TILE_SIZE, 0),
                       new Vector2(x * TILE_SIZE, LEVEL_HEIGHT * TILE_SIZE), 2, Color.Gray);
        }
        for (int y = 0; y <= LEVEL_HEIGHT; y++)
        {
            Raylib.DrawLineEx(new Vector2(0, y * TILE_SIZE),
                       new Vector2(LEVEL_WIDTH * TILE_SIZE, y * TILE_SIZE), 2, Color.Gray);
        }


        // Draw other players faded
        for (int i = 0; i < InputManager.MAX_PLAYERS; i++)
        {
            // Don't draw anything for inactive players
            if (!InputManager.Players[i].IsActive)
            {
                continue;
            }

            Raylib.DrawRectangle((int)players[i].position.X, (int)players[i].position.Y, TILE_SIZE, TILE_SIZE, InputManager.Players[i].GetColour());
            Raylib.DrawText($"P{i + 1}", 10, 10, 30, InputManager.Players[i].GetColour());
        }


        Raylib.EndMode2D();

        // Draw Player Indicators
        for (int i = 0; i < InputManager.MAX_PLAYERS; i++)
        {
            // Don't draw anything for inactive players
            if (!InputManager.Players[i].IsActive)
            {
                continue;
            }

            Vector2 playerPos = Raylib.GetWorldToScreen2D(players[i].position, camera);

            Raylib.DrawText($"P{i + 1}", (int)playerPos.X - 5, (int)playerPos.Y - 10, 10, InputManager.Players[i].GetColour());
        }


        viewport.End();
        viewport.Draw();
    }

    private void OnPlayerJoin(int playerId)
    {
        // If this is the first player to join, return
        if (InputManager.GetPlayerCount() <= 1) return;

        // Pick random player position
        Vector2 spawnPosition = players.Where((player, index) => InputManager.Players[index].IsActive).First().position;

        // Move player to new position
        players[playerId].position = spawnPosition;
    }
}