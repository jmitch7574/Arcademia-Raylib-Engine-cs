using System;
using System.Numerics;
using Raylib_cs;

public static class TextUtils
{
    public enum GuiTextAlignment
    {
        Left,
        Center,
        Right
    }

    public enum GuiTextAlignmentVertical
    {
        Top,
        Middle,
        Bottom
    }

    public static void DrawTextAligned(
        string text,
        int fontSize,
        Vector2 position,
        GuiTextAlignment horizontalAlignment,
        GuiTextAlignmentVertical verticalAlignment,
        Color color)
    {
        float spacing = MathF.Floor(fontSize / 10f);

        Vector2 textSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), text, fontSize, spacing);

        float targetX = horizontalAlignment switch
        {
            GuiTextAlignment.Left => position.X,
            GuiTextAlignment.Center => position.X - (textSize.X / 2f),
            GuiTextAlignment.Right => position.X - textSize.X,
            _ => position.X
        };

        float targetY = verticalAlignment switch
        {
            GuiTextAlignmentVertical.Top => position.Y,
            GuiTextAlignmentVertical.Middle => position.Y - (textSize.Y / 2f),
            GuiTextAlignmentVertical.Bottom => position.Y - textSize.Y,
            _ => position.Y
        };

        Vector2 targetPosition = new Vector2(targetX, targetY);

        Raylib.DrawTextEx(Raylib.GetFontDefault(), text, targetPosition, fontSize, spacing, color);
    }
}