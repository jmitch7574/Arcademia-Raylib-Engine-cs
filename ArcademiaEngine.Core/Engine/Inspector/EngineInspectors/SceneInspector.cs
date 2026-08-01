using System.Numerics;
using System.Reflection;
using ImGuiNET;

public class SceneInspector : InspectorTab
{

    Dictionary<string, Type> sceneMap = new Dictionary<string, Type>();

    private string searchQuery = "";
    private string selectedSceneKey = null;

    public SceneInspector() : base("Scene")
    {
        foreach (Type type in Assembly.GetAssembly(typeof(Scene)).GetTypes().Where(m => m.IsClass && !m.IsAbstract && m.IsSubclassOf(typeof(Scene))))
        {
            sceneMap.Add(type.ToString(), type);
        }
    }

    protected override void DrawInspectorItems()
    {
        Scene scene = SceneManager.GetScene();

        ImGui.SeparatorText("Scene Control");
        if (ImGui.Button("Load Scene"))
            ImGui.OpenPopup("Load Scene");

        if (ImGui.BeginPopupModal("Load Scene", ImGuiWindowFlags.AlwaysAutoResize))
        {
            // Search Bar
            ImGui.SetNextItemWidth(300);
            ImGui.InputText("Search", ref searchQuery, 100);

            ImGui.Spacing();

            // Scrollable selection box (Width: 300px, Height: 200px)
            if (ImGui.BeginChild("SceneList", new Vector2(300, 200)))
            {
                foreach (KeyValuePair<string, Type> kvp in sceneMap)
                {
                    string sceneName = kvp.Key;
                    Type sceneType = kvp.Value;

                    // Filter check (case-insensitive)
                    if (!string.IsNullOrWhiteSpace(searchQuery) &&
                        !sceneName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    // Selection state check using dictionary key
                    bool isSelected = (selectedSceneKey == sceneName);
                    if (ImGui.Selectable(sceneName, isSelected))
                    {
                        selectedSceneKey = sceneName;
                    }

                    // Double-click shortcut to load immediately
                    if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                    {
                        SceneManager.SetScene((Scene)Activator.CreateInstance(sceneType));
                        ImGui.CloseCurrentPopup();
                    }
                }
                ImGui.EndChild();
            }

            if (ImGui.Button("Cancel", new Vector2(120, 0)))
            {
                ImGui.CloseCurrentPopup();
            }

            ImGui.EndPopup();
        }

        ImGui.SeparatorText(scene.Name);
        ImGui.Text($@"Scene Lifetime: {TimeSpan.FromSeconds(scene.Lifetime):hh\:mm\:ss}");

        if (scene is ISceneInspector sceneInspector)
        {
            sceneInspector.DrawInspector();
        }
    }
}