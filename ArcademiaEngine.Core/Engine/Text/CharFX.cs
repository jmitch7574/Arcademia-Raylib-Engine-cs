using System.Numerics;
using Raylib_cs;

public struct CharFX
{
    public char Char;
    public Color Color = Color.White;
    public int Alpha = 255;
    public bool Visible = true;
    public float Time = 0;
    public Font Font = Raylib.GetFontDefault();
    public int GlyphIndex;
    public int GlyphCount;
    public int GlyphIndexLine;
    public Vector2 Offset;

    public CharFX()
    {
    }
}