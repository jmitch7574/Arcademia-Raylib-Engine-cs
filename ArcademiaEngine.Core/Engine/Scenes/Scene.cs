using Raylib_cs;

public abstract class Scene
{
    public string Name { get; protected set; }
    public bool isPaused { get; set; }
    public float Lifetime { get; private set; }

    protected Scene(string name)
    {
        Name = name;
    }

    public virtual void SceneUpdate()
    {
        Lifetime += Raylib.GetFrameTime();
        if (isPaused) return;
        Update();
    }

    protected abstract void Update();

    public abstract void Draw();
}