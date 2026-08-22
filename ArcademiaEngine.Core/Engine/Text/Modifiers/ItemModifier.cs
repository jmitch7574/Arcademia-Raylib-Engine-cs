using Raylib_cs;

namespace ArcademiaEngine.Core.Engine.Text.Modifiers;

[TextTag("item")]
public class ItemModifier : TextEffect
{
    public static Color ItemColor = Color.SkyBlue;

    public override void Modify(ref CharFX fx)
    {
        fx.Color = ItemColor;
    }
}