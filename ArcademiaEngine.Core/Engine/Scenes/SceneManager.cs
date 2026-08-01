public static class SceneManager
{
    static Scene currentScene;
    static Scene? nextScene;


    public static void Update()
    {
        currentScene.SceneUpdate();
    }

    public static void Draw()
    {
        currentScene.Draw();
    }

    public static void SetScene(Scene scene)
    {
        nextScene = scene;
    }

    public static void SwapScene()
    {
        if (nextScene != null)
        {
            currentScene = nextScene;
            nextScene = null;
        }
    }

    public static Scene GetScene()
    {
        return currentScene;
    }
}