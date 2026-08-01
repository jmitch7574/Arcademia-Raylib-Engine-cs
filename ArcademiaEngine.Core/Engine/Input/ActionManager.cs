using System.Text.Json;
using System.Text.Json.Serialization;
using Raylib_cs;

public class ButtonAction
{
    public List<KeyboardKey> KeyboardKeys { get; set; } = new();
    public List<ArcademiaKeybind> ArcademiaButtons { get; set; } = new();
    public GamepadButton GamepadButton { get; set; } = new();
}

public class AxisAction
{
    public ButtonAction NegativeAction { get; set; } = new();
    public ButtonAction PositiveAction { get; set; } = new();
    public GamepadAxis ControllerAxis { get; set; } = new();
}

public class VectorAction
{
    public AxisAction AxisX { get; set; } = new();
    public AxisAction AxisY { get; set; } = new();
}

public class ActionMap
{
    public static ActionMap LoadedMap { get; private set; } = new();
    public static string path = "action_map.json";

    public Dictionary<string, ButtonAction> ButtonActions { get; set; } = [];
    public Dictionary<string, AxisAction> AxisActions { get; set; } = [];
    public Dictionary<string, VectorAction> VectorActions { get; set; } = [];

    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() } // Automatically handles lists/arrays of enums
    };

    public static void Save()
    {
        string json = JsonSerializer.Serialize(LoadedMap, Options);
        File.WriteAllText(Resources.GetResourcePath(path), json);
    }

    public static void Load()
    {
        if (!File.Exists(Resources.GetResourcePath(path)))
        {
            LoadedMap = new();
            return;
        }

        using StreamReader r = new(Resources.GetResourcePath(path));

        string json = r.ReadToEnd();
        LoadedMap = JsonSerializer.Deserialize<ActionMap>(json, Options);
    }

    public static ButtonAction GetButtonAction(string actionName)
    {
        return LoadedMap.ButtonActions[actionName];
    }

    public static AxisAction GetAxisAction(string actionName)
    {
        return LoadedMap.AxisActions[actionName];
    }

    public static VectorAction GetVectorAction(string actionName)
    {
        return LoadedMap.VectorActions[actionName];
    }
}