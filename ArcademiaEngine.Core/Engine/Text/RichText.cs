using System;
using System.Numerics;
using System.Timers;
using ArcademiaEngine.Core.Engine.Text.Modifiers;
using Raylib_cs;

namespace ArcademiaEngine.Core.Engine.Text;

public class RichText
{
    public string Text
    {
        get;
        set
        {
            field = value;
            RichCharacters = RichTextParser.Parse(value);
            Lifetime = 0;
        }
    }

    public Vector2 Position;

    public TextSettings Settings;

    private List<RichChar> RichCharacters = [];

    public double Lifetime = 0.0f;

    public Shader GlowShader;

    Viewport? GlowPass;

    public RichText(string text, Vector2 position, TextSettings settings, Viewport? parentVp = null)
    {
        Text = text;
        Position = position;
        Settings = settings;

        GlowShader = Raylib.LoadShader("", $"{Resources.GetResourcePath()}/shaders/text_outline.fs");

        int glowRadiusLoc = Raylib.GetShaderLocation(GlowShader, "glowRadius");
        Raylib.SetShaderValue(GlowShader, glowRadiusLoc, settings.GlowRadius, ShaderUniformDataType.Float);
        int glowStrengthLoc = Raylib.GetShaderLocation(GlowShader, "glowStrength");
        Raylib.SetShaderValue(GlowShader, glowStrengthLoc, settings.GlowStrength, ShaderUniformDataType.Float);

        if (parentVp != null)
        {
            GlowPass = new Viewport(parentVp.Width, parentVp.Height);
            int renderSizeLoc = Raylib.GetShaderLocation(GlowShader, "renderSize");
            Vector2 size = new(GlowPass.Width, GlowPass.Height);
            Raylib.SetShaderValue(GlowShader, renderSizeLoc, size, ShaderUniformDataType.Vec2);
        }
    }

    ~RichText()
    {
        Raylib.UnloadShader(GlowShader);
    }

    public void Draw()
    {
        Raylib.SetTextureFilter(Settings.Font.Texture, TextureFilter.Point);
        float spacing = MathF.Floor(Settings.FontSize / 10f);

        // Parse Rich Text
        List<RichChar> characters = RichTextParser.Parse(Text);

        // Split Rich Characters into lines
        List<List<RichChar>> lines = new();
        List<RichChar> currentLine = new();

        // Calculate Lifetime
        Lifetime += Raylib.GetFrameTime();

        foreach (var rc in characters)
        {
            if (rc.Character == '\n')
            {
                lines.Add(currentLine);
                currentLine = new();
            }
            else
            {
                currentLine.Add(rc);
            }
        }
        if (currentLine.Count > 0) lines.Add(currentLine);

        // Calculate the top Y position of the text block
        float totalHeight = lines.Count * Settings.FontSize + (lines.Count - 1) * spacing;
        float blockStartY = Settings.VerticalAlignment switch
        {
            Alignment.Start => Position.Y,
            Alignment.Middle => Position.Y - (totalHeight / 2f),
            Alignment.End => Position.Y - totalHeight,
            _ => Position.Y
        };

        // Index Tracking
        int globalCharIndex = 0;
        int totalChars = characters.Count;

        if (GlowPass != null) GlowPass.Clear();

        // Iterate through each line
        for (int lineIndex = 0; lineIndex < lines.Count; lineIndex++)
        {
            List<RichChar> line = lines[lineIndex];

            // Measure line width
            string lineText = new string(line.Select(c => c.Character).ToArray());
            Vector2 lineSize = Raylib.MeasureTextEx(Settings.Font, lineText, Settings.FontSize, spacing);

            // Calculate Line Position
            float currentX = Settings.HorizontalAlignment switch
            {
                Alignment.Start => Position.X,
                Alignment.Middle => Position.X - (lineSize.X / 2f),
                Alignment.End => Position.X - lineSize.X,
                _ => Position.X
            };

            // Offset Y by line index
            float currentY = blockStartY + (lineIndex * (Settings.FontSize + spacing));

            // Iterate and draw characters
            for (int c = 0; c < line.Count; c++)
            {
                RichChar richChar = line[c];
                char singleChar = richChar.Character;

                CharFX fx = new CharFX
                {
                    Char = singleChar,
                    GlyphIndexLine = c,
                    GlyphIndex = globalCharIndex,
                    GlyphCount = totalChars,
                    Time = (float)Lifetime,
                    Offset = Vector2.Zero,
                    Color = Settings.Color,
                    Alpha = 255,
                    Visible = true
                };

                // Apply all active effects for this character
                foreach (var effect in richChar.ActiveEffects)
                {
                    effect.Modify(ref fx);
                }

                string singleCharStr = fx.Char.ToString();

                // Draw Character
                if (fx.Visible)
                {

                    GlowModifier? check = (GlowModifier?)richChar.ActiveEffects.FirstOrDefault(e => e is GlowModifier);

                    bool isGlowing = check is GlowModifier glow;

                    if (GlowPass != null && isGlowing && !GlowPass.Drawing) GlowPass?.Begin(false);

                    Vector2 drawPos = new Vector2(
                        (int)(currentX + fx.Offset.X),
                        (int)(currentY + fx.Offset.Y)
                    );


                    Raylib.DrawTextEx(
                        Settings.Font,
                        singleCharStr,
                        drawPos,
                        Settings.FontSize,
                        spacing,
                        fx.Color with { A = (byte)fx.Alpha }
                    );

                    if (GlowPass != null && !isGlowing && GlowPass.Drawing) GlowPass?.End();
                }

                // Move X along by character width
                float charWidth = Raylib.MeasureTextEx(Settings.Font, singleCharStr, Settings.FontSize, spacing).X;
                currentX += charWidth + spacing;

                globalCharIndex++;
            }
        }

        if (GlowPass != null && GlowPass.Drawing) GlowPass?.End();
        if (GlowPass != null)
        {
            Raylib.BeginShaderMode(GlowShader);
            GlowPass.Draw();
            Raylib.EndShaderMode();
            GlowPass.Draw();

        }
    }
}