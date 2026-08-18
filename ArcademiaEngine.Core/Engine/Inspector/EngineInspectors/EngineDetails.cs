using System.Reflection;
using System.Runtime.InteropServices;
using ArcademiaEngine.Core;
using ImGuiNET;
using Raylib_cs;

public sealed class EngineDetails : InspectorTab
{
    public EngineDetails() : base("Engine")
    {
    }

    protected override void DrawInspectorItems()
    {
        ImGui.Text($"{DateTime.Now:t}");

        ImGui.SeparatorText("Arcademia Cross Platform Raylib Engine (ACRE)");
        ImGui.Text($"{RuntimeInformation.FrameworkDescription}");
        ImGui.Text($"Raylib v{Raylib.RAYLIB_VERSION}");
        ImGui.Text($"DearImGui v{ImGui.GetVersion()}");

        ImGui.SeparatorText("Engine Details");
        ImGui.Text($"Arcademia Mode: {Launcher.IsArcademia()}");
    }
}