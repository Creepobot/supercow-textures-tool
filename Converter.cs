using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using static UsefulFuncsClass;
using TGASharpLib;

public static class Converter
{
    public static string Convert(string filepath, string folderpath, string output)
    {
        switch (output)
        {
            case "PNGJPGA":
                return PngToJpga(filepath, folderpath);
            case "PNGTGA":
                return PngToTga(filepath, folderpath);
            case "JPGAPNG":
                return JpgaToPng(filepath, folderpath);
            case "TGAPNG":
                return TgaToPng(filepath, folderpath);
            case "BJPGPNG":
                return BjpgToPng(filepath, folderpath);
        }
        return "";
    }

    private static string JpgaToPng(string filepath, string folderpath)
    {
        string filename;
        using (var filestream = File.OpenRead(filepath))
        {
            byte[] buffer = new byte[1000000];
            int l = 0;
            filestream.Read(buffer, 0, 1000000);
            while (buffer.FindBytes(FFD9) == -1)
            {
                if (filestream.Position < filestream.Length)
                {
                    Array.Clear(buffer, 0, 999999);
                    buffer[0] = buffer[999999];
                    filestream.Read(buffer, 1, 999999);
                    l++;
                }
                else
                    break;
            }
            filestream.Position = (999999 * l) +
                buffer.FindBytes(FFD9) + 2;
            buffer = null;
            Bitmap img = (Bitmap)Image.FromFile(filepath);
            using (var imgclone = img.Clone(new Rectangle(0, 0,
                    img.Width, img.Height), PixelFormat.Format32bppArgb))
            {
                img.Dispose();
                if (filestream.Length - filestream.Position !=
                imgclone.Height * imgclone.Width)
                {
                    MessageBox.Show($"{Path.GetFileName(filepath)}" +
                        " is not JPGA texture", "Eblan",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return "";
                }
                for (int i = 0; i < imgclone.Height; i++)
                    for (int j = 0; j < imgclone.Width; j++)
                    {
                        Color p = imgclone.GetPixel(j, i);
                        imgclone.SetPixel(j, i,
                            Color.FromArgb(filestream.ReadByte(),
                            p.R, p.G, p.B));
                    }
                filename = IncrFilename(filepath.TGATag(true),
                    folderpath, "png");
                imgclone.Save(filename, ImageFormat.Png);
            }
        }
        GC.Collect();
        return filename;
    }

    private static string PngToJpga(string filepath, string folderpath)
    {
        string filename = IncrFilename(filepath.TGATag(),
            folderpath, "jpg");
        using (var filestr = File.Open(filename,
            FileMode.CreateNew, FileAccess.ReadWrite))
        {
            using (var img = (Bitmap)Image.FromFile(filepath))
            {
                BaseJPEGEncoder encoder = new BaseJPEGEncoder();
                encoder.EncodeImageToJpg(img, new BinaryWriter(filestr));
                encoder = null;
                GC.Collect();
                byte[] alpha = new byte[1000000];
                int l = 0;
                filestr.Read(alpha, 0, 1000000);
                while (filestr.Position < filestr.Length || l == 0)
                {
                    int i = alpha.FindBytes(FFD9);
                    if (i != -1 && (999999 * l) + i != filestr.Length - 2)
                    {
                        int oldpos = (int)filestr.Position;
                        filestr.Position = (999999 * l) + i + 1;
                        filestr.WriteByte(0xD8);
                        filestr.Position = oldpos;
                    }
                    Array.Clear(alpha, 0, 999999);
                    alpha[0] = alpha[999999];
                    filestr.Read(alpha, 1, 999999);
                    l++;
                }
                Array.Clear(alpha, 0, 1000000);
                l = 0;
                for (int i = 0; i < img.Height; i++)
                    for (int j = 0; j < img.Width; j++)
                    {
                        alpha[l] = img.GetPixel(j, i).A;
                        l++;
                        if (l >= alpha.Length)
                        {
                            filestr.Write(alpha, 0, alpha.Length);
                            Array.Clear(alpha, 0, alpha.Length);
                            l = 0;
                            GC.Collect();
                        }
                    }
                filestr.Write(alpha, 0, l);
                alpha = null;
            }
        }
        GC.Collect();
        return filename;
    }

    private static string TgaToPng(string filepath, string folderpath)
    {
        string filename = IncrFilename(filepath, folderpath, "png");
        TGA.FromFile(filepath).ToBitmap().Save(filename, ImageFormat.Png);
        GC.Collect();
        return filename;
    }

    private static string PngToTga(string filepath, string folderpath)
    {
        string filename = IncrFilename(filepath, folderpath, "tga");
        TGA.FromBitmap((Bitmap)Image.FromFile(filepath)).Save(filename);
        return filename;
    }

    private static string BjpgToPng(string filepath, string folderpath)
    {
        string filename = IncrFilename(filepath, folderpath, "png");
        Bitmap img = (Bitmap)Image.FromFile(filepath);
        using (var imgclone = img.Clone(new Rectangle(0, 0,
                img.Width, img.Height), PixelFormat.Format32bppArgb))
        {
            img.Dispose();
            for (int i = 0; i < imgclone.Height; i++)
                for (int j = 0; j < imgclone.Width; j++)
                {
                    Color p = imgclone.GetPixel(j, i);
                    imgclone.SetPixel(j, i, p.ClearBlack());
                }
            imgclone.Save(filename, ImageFormat.Png);
        }
        return filename;
    }
}