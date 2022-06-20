using System.Drawing;
using System.IO;

public static class UsefulFuncsClass
{
    public static byte[] FFD9 = new byte[] { 0xFF, 0xD9 };

    public static int FindBytes(this byte[] src, byte[] find)
    {
        int index = -1;
        int matchIndex = 0;
        for (int i = 0; i < src.Length; i++)
        {
            if (src[i] == find[matchIndex])
            {
                if (matchIndex == (find.Length - 1))
                {
                    index = i - matchIndex;
                    break;
                }
                matchIndex++;
            }
            else if (src[i] == find[0])
                matchIndex = 1;
            else
                matchIndex = 0;
        }
        return index;
    }

    public static string TGATag(this string filepath, bool isRemove = false)
    {
        string folder = Path.GetFullPath(filepath);
        string file = Path.GetFileName(filepath);
        if (isRemove)
            if (file.StartsWith("_a_"))
                file = file.Replace("_a_", "");
            else
            if (!file.StartsWith("_a_"))
                file = $"_a_{file}";
        return Path.Combine(folder, file);
    }

    public static string IncrFilename(string filepath, string folder, string ext)
    {
        string file = Path.GetFileNameWithoutExtension(filepath);
        string filename = file;
        int count = 0;
        while (File.Exists(Path.Combine(folder, $"{filename}.{ext}")))
        {
            count++;
            filename = $"{file}({count})";
        }
        return Path.Combine(folder, $"{filename}.{ext}");
    }

    public static Color ClearBlack(this Color c)
    {
        return Color.FromArgb((int)(255 *
            c.GetBrightness()), c.R, c.G, c.B);
    }
}