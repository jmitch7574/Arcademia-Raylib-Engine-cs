namespace ArcademiaEngine.Core.Engine.Text.Modifiers;

[TextTag("glow")]
public class GlowModifier : TextEffect
{
    public float Strength { get; set; } = 5.0f;

    public override void Modify(ref CharFX fx)
    {

    }
}