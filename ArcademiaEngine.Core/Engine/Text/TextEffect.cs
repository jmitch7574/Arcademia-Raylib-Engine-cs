[AttributeUsage(AttributeTargets.Class)]
public class TextTagAttribute : Attribute
{
    public string TagName { get; }

    public TextTagAttribute(string tagName)
    {
        TagName = tagName.ToLower();
    }
}

public abstract class TextEffect
{
    public abstract void Modify(ref CharFX fx);
}