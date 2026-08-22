using System.Globalization;
using System.Reflection;
using System.Xml;

public class RichChar
{
    public char Character;
    public List<TextEffect> ActiveEffects; // The effects active on THIS specific character

    public RichChar(char character, IEnumerable<TextEffect> activeEffects)
    {
        Character = character;
        ActiveEffects = new List<TextEffect>(activeEffects);
    }
}

public static class RichTextParser
{
    private static readonly Dictionary<string, Type> TagTypeRegistry = new();

    static RichTextParser()
    {
        foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (typeof(TextEffect).IsAssignableFrom(type) && !type.IsAbstract)
            {
                var attr = type.GetCustomAttribute<TextTagAttribute>();
                if (attr != null)
                {
                    TagTypeRegistry[attr.TagName] = type;
                }
            }
        }
    }

    public static List<RichChar> Parse(string input)
    {
        var result = new List<RichChar>(input.Length);
        var effectStack = new Stack<TextEffect>();

        var settings = new XmlReaderSettings
        {
            ConformanceLevel = ConformanceLevel.Fragment,
            CheckCharacters = false
        };

        using var xmlReader = XmlReader.Create(new System.IO.StringReader(input), settings);

        try
        {
            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        string tagName = xmlReader.Name.ToLower();

                        if (TagTypeRegistry.TryGetValue(tagName, out Type effectType))
                        {
                            var effect = (TextEffect)Activator.CreateInstance(effectType);

                            if (xmlReader.HasAttributes)
                            {
                                while (xmlReader.MoveToNextAttribute())
                                {
                                    string attrName = xmlReader.Name.Equals(tagName, StringComparison.OrdinalIgnoreCase)
                                        ? "Value"
                                        : xmlReader.Name;

                                    SetPropertyFromAttribute(effect, attrName, xmlReader.Value);
                                }
                                xmlReader.MoveToElement();
                            }

                            effectStack.Push(effect);
                        }
                        break;

                    case XmlNodeType.Text:
                        // Attach the currently active stack directly to every character
                        string text = xmlReader.Value;
                        for (int i = 0; i < text.Length; i++)
                        {
                            result.Add(new RichChar(text[i], effectStack));
                        }
                        break;

                    case XmlNodeType.EndElement:
                        string closeTag = xmlReader.Name.ToLower();
                        if (TagTypeRegistry.ContainsKey(closeTag) && effectStack.Count > 0)
                        {
                            effectStack.Pop();
                        }
                        break;
                }
            }
        }
        catch (XmlException)
        {
            Inspector.Error($"[TEXT] Parser encounted an error on {input}");
        }

        return result;
    }

    private static void SetPropertyFromAttribute(object instance, string propName, string propValue)
    {
        PropertyInfo prop = instance.GetType().GetProperty(
            propName,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        if (prop != null && prop.CanWrite)
        {
            try
            {
                // Convert string XML values to property types (float, int, bool, etc.)
                object convertedValue = Convert.ChangeType(propValue, prop.PropertyType, CultureInfo.InvariantCulture);
                prop.SetValue(instance, convertedValue);
            }
            catch
            {
                Inspector.Error($"[TEXT] Parser encounted invalid type assignment");
            }
        }
    }
}