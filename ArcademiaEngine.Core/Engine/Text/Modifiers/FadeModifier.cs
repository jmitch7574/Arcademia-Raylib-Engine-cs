namespace ArcademiaEngine.Core.Engine.Text.Modifiers;

[TextTag("fade")]
public class FadeModifier : TextEffect
{
    // At what fx.Time does the text begin fading in
    public float InTime { get; set; } = 0.0f;

    // At what fx.Time does the text finish fading out (-1 to disable)
    public float OutTime { get; set; } = -1.0f;

    // Characters per second
    public float Speed { get; set; } = 5.0f;

    // How long each individual glyph takes to fade (in/out)
    private const float FadeDuration = 1.0f;

    public override void Modify(ref CharFX fx)
    {
        float safeSpeed = Speed <= 0f ? 0.001f : Speed;

        // Fade-In: cascade starting at InTime
        float fadeIn = fx.Time - InTime - (fx.GlyphIndex / safeSpeed);
        fadeIn = float.Clamp(fadeIn, 0f, 1f);

        float fadeOut = 0f;
        if (OutTime >= 0f)
        {
            float fadeDuration = (float)fx.GlyphCount / Speed;

            // Start early enough that glyph (GlyphCount - 1) finishes right at OutTime
            float fadeOutStart = OutTime - (fadeDuration + FadeDuration);

            fadeOut = fx.Time - fadeOutStart - (fx.GlyphIndex / safeSpeed);
            fadeOut = float.Clamp(fadeOut, 0f, 1f);
        }

        float alphaProgress = fadeIn - fadeOut;
        fx.Alpha = (int)(float.Clamp(alphaProgress, 0f, 1f) * 255f);
    }
}