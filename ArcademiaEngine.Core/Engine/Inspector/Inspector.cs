using System.Numerics;
using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;

public enum MessageLevel
{
    INFO,
    WARNING,
    ERROR
}

public struct InspectorMessage
{
    public string message;
    public MessageLevel level;
}

static class Inspector
{
    // State variables
    public static bool Initialised = false;
    public static bool Active { get; private set; } = false;
    public static bool ShouldPauseGame => ImGui.IsPopupOpen(null, ImGuiPopupFlags.AnyPopup);

    // Sizing
    public const int InspectorWidth = 400;
    public const int ConsoleHeight = 200;


    // Inspector
    private static readonly List<InspectorTab> tabs = [];

    // Console
    private static readonly List<InspectorMessage> messages = [];

    public static void Init()
    {
        Initialised = true;
        rlImGui.Setup(true);
        Inspector.Log("INSPECTOR: initialised");

        RegisterInspector(new EngineDetails());
    }

    public static void Shutdown()
    {
        rlImGui.Shutdown();
    }

    public static void RegisterInspector(InspectorTab tab)
    {
        tabs.Add(tab);
    }

    public static void RemoveInspector(InspectorTab tab)
    {
        tabs.Remove(tab);
    }

    private static void AddMessage(string message, MessageLevel level)
    {
        InspectorMessage im = new InspectorMessage { message = message, level = level };
        messages.Add(im);
        Console.WriteLine(StringFromMessage(im));
    }

    private static string StringFromMessage(InspectorMessage inspectorMessage) { return $"[{Enum.GetName(inspectorMessage.level.GetType(), inspectorMessage.level)}]: {inspectorMessage.message}"; }

    public static void Log(string message) => AddMessage(message, MessageLevel.INFO);

    public static void Warn(string message) => AddMessage(message, MessageLevel.WARNING);

    public static void Error(string message) => AddMessage(message, MessageLevel.ERROR);

    public static void Update()
    {
        if (!Initialised) return;

        if (Raylib.IsKeyPressed(KeyboardKey.Grave))
        {
            Active = !Active;
        }
    }

    public static void Draw(int screenWidth, int screenHeight)
    {
        if (!Initialised || !Active) return;

        rlImGui.Begin();

        // Inspector Tab

        ImGui.SetNextWindowPos(new Vector2(screenWidth - InspectorWidth, 0));
        ImGui.SetNextWindowSize(new Vector2(InspectorWidth, screenHeight));
        ImGui.Begin("Inspector", ImGuiWindowFlags.NoCollapse);
        ImGui.BeginTabBar("System");
        foreach (InspectorTab tab in tabs)
        {
            tab.DrawInspector();
        }
        ImGui.EndTabBar();
        ImGui.End();

        // Console Tab

        ImGui.SetNextWindowPos(new Vector2(0, screenHeight - ConsoleHeight));
        ImGui.SetNextWindowSize(new Vector2(screenWidth - InspectorWidth, ConsoleHeight));
        ImGui.Begin("Console", ImGuiWindowFlags.NoCollapse);

        ImGui.BeginChild("Scrolling Region", new Vector2((float)0, (float)0), ImGuiChildFlags.Borders,
                    ImGuiWindowFlags.HorizontalScrollbar);

        foreach (InspectorMessage msg in messages)
        {

            Vector4 textColor = msg.level switch
            {
                MessageLevel.INFO => new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                MessageLevel.WARNING => new Vector4(1.0f, 1.0f, 0, 1.0f),
                MessageLevel.ERROR => new Vector4(1.0f, 0, 0, 1.0f),
                _ => new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
            };

            ImGui.TextColored(textColor, StringFromMessage(msg));

        }

        // Auto scrolling logic
        bool autoScrolling = ImGui.GetScrollY() >= ImGui.GetScrollMaxY();

        if (autoScrolling) ImGui.SetScrollHereY(1.0f);

        ImGui.EndChild();

        ImGui.End();
        rlImGui.End();
    }
}