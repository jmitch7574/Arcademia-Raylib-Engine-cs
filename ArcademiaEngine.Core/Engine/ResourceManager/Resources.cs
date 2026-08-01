using System.Runtime.CompilerServices;

public class Resources
{
    public static string GetResourcePath()
    {

#if DEBUG
        string sourceDir = Path.GetDirectoryName(GetSourceFilePath());

        string resourcesPath = Path.GetFullPath(Path.Combine(sourceDir, "..", "..", "..", "Resources"));

        return Path.GetFullPath(resourcesPath);
#endif

        return Path.Combine(AppContext.BaseDirectory, "Resources");
    }

    public static string GetResourcePath(string file)
    {
        return Path.Combine(GetResourcePath(), file);
    }


#if DEBUG
    private static string GetSourceFilePath([CallerFilePath] string path = "") => path;
#endif
}