using Raylib_cs;

public struct PlayerSlot
{
    public int PlayerIndex { get; }
    public bool IsActive { get; set; }

    public PlayerInput Input;

    public PlayerSlot(int playerIndex)
    {
        PlayerIndex = playerIndex;
        IsActive = false;
        Input = new();
    }

    public readonly string GetFriendlyName()
    {
        return "not implemented";
    }

    public readonly Color GetColour()
    {
        return InputManager.PlayerColours[PlayerIndex];
    }
}