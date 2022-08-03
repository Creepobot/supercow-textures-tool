using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using static UsefulFuncsClass;
using TGASharpLib;

public static class Converter
{
    /// <summary>Converts the image depending on the conversion method</summary>
    /// <param name="filepath">Path to input file</param>
    /// <param name="folderpath">Path to output folder</param>
    /// <param name="output">Conversion method type. Kludge, which was formed because original program use two ComboBoxes with extension names</param>
    /// <returns>Path to output file or exception</returns>
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
        throw new Exception("Invalid conversion method");
    }

    /// <summary>Convert JPG image with transparency bytes to transparent PNG</summary>
    /// <param name="filepath">Path to input file</param>
    /// <param name="folderpath">Path to output folder</param>
    /// <returns>Path to output file</returns>
    private static string JpgaToPng(string filepath, string folderpath)
    {
        string filename;
        using (var filestream = File.OpenRead(filepath))
        {

            #region Skip to transparency bytes
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
            #endregion

            Bitmap img = (Bitmap)Image.FromFile(filepath);
            using (var imgclone = img.Clone(new Rectangle(0, 0,
                    img.Width, img.Height), PixelFormat.Format32bppArgb))
            {
                img.Dispose();

                #region Transparency bytes to real transparency
                if (filestream.Length - filestream.Position !=
                imgclone.Height * imgclone.Width)
                    throw new Exception("File is not a JPGA texture");
                for (int i = 0; i < imgclone.Height; i++)
                    for (int j = 0; j < imgclone.Width; j++)
                    {
                        Color p = imgclone.GetPixel(j, i);
                        imgclone.SetPixel(j, i,
                            Color.FromArgb(filestream.ReadByte(),
                            p.R, p.G, p.B));
                    }
                #endregion

                filename = IncrFilename(filepath.JpgaTag(true),
                    folderpath, "png");
                imgclone.Save(filename, ImageFormat.Png);
            }
        }
        GC.Collect();
        return filename;
    }

    /// <summary>Convert PNG image to JPG with transparency bytes</summary>
    /// <param name="filepath">Path to input file</param>
    /// <param name="folderpath">Path to output folder</param>
    /// <returns>Path to output file</returns>
    private static string PngToJpga(string filepath, string folderpath)
    {
        string filename = IncrFilename(filepath.JpgaTag(),
            folderpath, "jpg");
        using (var filestr = File.Open(filename,
            FileMode.CreateNew, FileAccess.ReadWrite))
        {
            using (var img = (Bitmap)Image.FromFile(filepath))
            {
                #region Convert image to jpg

                #region ArpanJpegEncoder Code
                BaseJPEGEncoder encoder = new BaseJPEGEncoder();
                encoder.EncodeImageToJpg(img, new BinaryWriter(filestr));
                encoder = null;
                #endregion

                #region Unused Code
                //img.Save(filestr, ImageFormat.Jpeg);
                #endregion

                GC.Collect();
                #endregion

                #region Replace FFD9
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
                #endregion

                #region Write transparency bytes
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
                #endregion
            }
        }
        GC.Collect();
        return filename;
    }

    /// <summary>Convert TGA image to PNG. Uses TGASharpLib library</summary>
    /// <param name="filepath">Path to input file</param>
    /// <param name="folderpath">Path to output folder</param>
    /// <returns>Path to output file</returns>
    private static string TgaToPng(string filepath, string folderpath)
    {
        string filename = IncrFilename(filepath, folderpath, "png");
        TGA.FromFile(filepath).ToBitmap().Save(filename, ImageFormat.Png);
        GC.Collect();
        return filename;
    }

    /// <summary>Convert PNG image to TGA. Uses TGASharpLib library</summary>
    /// <param name="filepath">Path to input file</param>
    /// <param name="folderpath">Path to output folder</param>
    /// <returns>Path to output file</returns>
    private static string PngToTga(string filepath, string folderpath)
    {
        string filename = IncrFilename(filepath, folderpath, "tga");
        TGA.FromBitmap((Bitmap)Image.FromFile(filepath)).Save(filename);
        return filename;
    }

    /// <summary>Convert JPG image with black background to transparent PNG</summary>
    /// <param name="filepath">Path to input file</param>
    /// <param name="folderpath">Path to output folder</param>
    /// <returns>Path to output file</returns>
    private static string BjpgToPng(string filepath, string folderpath)
    {
        string filename = IncrFilename(filepath, folderpath, "png");
        Bitmap img = (Bitmap)Image.FromFile(filepath);
        using (var imgclone = img.Clone(new Rectangle(0, 0,
                img.Width, img.Height), PixelFormat.Format32bppArgb))
        {
            img.Dispose();

            #region Replace shades of black with transparency
            for (int i = 0; i < imgclone.Height; i++)
                for (int j = 0; j < imgclone.Width; j++)
                {
                    Color p = imgclone.GetPixel(j, i);
                    imgclone.SetPixel(j, i, p.ClearBlack());
                }
            #endregion

            imgclone.Save(filename, ImageFormat.Png);
        }
        return filename;
    }
}
