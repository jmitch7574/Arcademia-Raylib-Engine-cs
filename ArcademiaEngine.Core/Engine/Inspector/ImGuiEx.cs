using System.Numerics;
using ImGuiNET;
using Raylib_cs;

public static class ImGuiEx
{
    public static void RaylibColorEdit(string label, ref Color col)
    {
        Vector4 vCol = Raylib.ColorNormalize(col);

        ImGui.ColorEdit4(label, ref vCol);

        col = Raylib.ColorFromNormalized(vCol);
    }
}