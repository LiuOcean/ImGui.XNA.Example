namespace Example;

public class FileHelper
{
    public static string GetPath(params string[] paths)
    {
        var path = Path.Join(paths);

        if(Path.IsPathRooted(path))
        {
            // Already rooted, so yay?
            return path;
        }

        return Path.GetFullPath(Path.Join(Path.GetDirectoryName(AppContext.BaseDirectory), path));
    }
}