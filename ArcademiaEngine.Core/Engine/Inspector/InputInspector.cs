using ImGuiNET;

public class InputInspector : InspectorTab
{
    public InputInspector() : base("Input")
    {
    }

    protected override void DrawInspectorItems()
    {
        ImGui.Checkbox("Listening", ref InputManager.IsListening);

        for (int i = 0; i < InputManager.MAX_PLAYERS; i++)
        {
            if (InputManager.Players[i].IsActive)
            {
                ImGui.SeparatorText($"Player {i}");
                ImGui.Text($"Is Connected: {InputManager.Players[i].Input.IsConnected}");
                ImGui.Text($"Is Keyboard: {InputManager.Players[i].Input.IsKeyboard}");
                ImGui.Text($"Input Device ID: {InputManager.Players[i].Input.InputIdx}");


                ImGui.Separator();
                ImGui.Text("Actions:");

                foreach (string key in ActionMap.LoadedMap.ButtonActions.Keys)
                {
                    ImGui.Text($"{key}: {InputManager.Players[i].Input.IsActionDown(key)}");
                }
                foreach (string key in ActionMap.LoadedMap.AxisActions.Keys)
                {
                    ImGui.Text($"{key}: {InputManager.Players[i].Input.GetActionAxis(key)}");
                }
                foreach (string key in ActionMap.LoadedMap.VectorActions.Keys)
                {
                    ImGui.Text($"{key}: {InputManager.Players[i].Input.GetActionVector2(key)}");
                }
            }
        }
    }
}