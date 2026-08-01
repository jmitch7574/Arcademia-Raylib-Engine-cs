using ImGuiNET;

public abstract class InspectorTab
{
    public string Name { get; protected set; } = "Unnamed Tab";

    public InspectorTab(string name)
    {
        Name = name;
    }

    public void DrawInspector()
    {
        if (ImGui.BeginTabItem(Name))
        {
            DrawInspectorItems();
            ImGui.EndTabItem();
        }
    }

    protected abstract void DrawInspectorItems();
}