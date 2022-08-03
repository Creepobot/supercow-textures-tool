using System.Drawing;
using System.IO;

public static class UsefulFuncsClass
{
    /// <summary>Just overused byte array</summary>
    public static byte[] FFD9 = new byte[] { 0xFF, 0xD9 };

    /// <summary>Finds bytes in a byte array</summary>
    /// <param name="src">Byte array to search</param>
    /// <param name="find">Byte array to be found</param>
    /// <returns>Byte index</returns>
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

    /// <summary>Adds or removes "_a_" from the beginning of the filename</summary>
    /// <param name="filepath">Path to the desired file</param>
    /// <param name="isRemove">Need to add "_a_" or remove?</param>
    /// <returns>Path for file</returns>
    public static string JpgaTag(this string filepath, bool isRemove = false)
    {
        string folder = Path.GetFullPath(filepath);
        string file = Path.GetFileName(filepath);
        if (isRemove)
        {
            if (file.StartsWith("_a_"))
                file = file.Replace("_a_", "");
        }
        else
            if (!file.StartsWith("_a_"))
                file = $"_a_{file}";
        return Path.Combine(folder, file);
    }

    /// <summary>If a file with the same name exists in the output folder, rename it by adding numbers at the end</summary>
    /// <param name="filepath">Path to input file</param>
    /// <param name="folder">Path to output folder</param>
    /// <param name="ext">Output file extension</param>
    /// <returns>Path for file</returns>
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

    /// <summary>Changes shades of black to transparency</summary>
    /// <param name="c">Input color</param>
    /// <returns>Color where black is almost replaced by transparency</returns>
    public static Color ClearBlack(this Color c)
    {
        return Color.FromArgb((int)(255 *
            c.GetBrightness()), c.R, c.G, c.B);
    }
}
