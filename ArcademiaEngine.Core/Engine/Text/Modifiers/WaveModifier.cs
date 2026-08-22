namespace ArcademiaEngine.Core.Engine.Text.Modifiers;

[TextTag("wave")]
public class WaveModifier : TextEffect
{
    public float Amplitude { get; set; } = 5.0f;
    public float Frequency { get; set; } = 1.0f;
    public float Speed { get; set; } = 5.0f;

    public override void Modify(ref CharFX fx)
    {
        float waveOffset = MathF.Sin((fx.Time * Speed) + (Frequency * (fx.GlyphIndexLine * 0.5f)));
        fx.Offset.Y += waveOffset * Amplitude;
    }
}