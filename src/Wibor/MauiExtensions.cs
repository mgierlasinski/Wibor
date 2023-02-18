namespace Wibor;

public static class MauiExtensions
{
    public static Color MakeTransparent(this Color color, float alpha) => Color.FromRgba(color.Red, color.Green, color.Blue, alpha);

    public static T GetResource<T>(this Application app, string name) where T : class
    {
        if (app.Resources.TryGetValue(name, out var resource))
        {
            return resource as T;
        }

        foreach (var res in app.Resources.MergedDictionaries)
        {
            if (res.TryGetValue(name, out var value))
            {
                return value as T;
            }
        }

        return default;
    }
}