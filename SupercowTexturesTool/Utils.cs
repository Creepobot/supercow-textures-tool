using EblanModule;
using Nevosoft;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TGASharpLib;

namespace SupercowTexturesTool
{
    public partial class Form1
    {
        private int ebl, converrs, converdone = 0;
        private string[][] result;
        private string cbox1 = "PNG", cbox2 = "JPGA";
        private string inputExt = ".png", outputExt = ".jpg";
        private string[] files;
        private string folder;

        private void ToggleUI(bool enabled = true)
        {
            pictureBox1.Enabled = enabled;
            pictureBox2.Enabled = enabled;
            comboBox1.Enabled = enabled;
            comboBox2.Enabled = enabled;
            pictureBox3.Enabled = enabled;
            richlabel1.Enabled = enabled;
            converrs = 0;
            converdone = 0;
        }

        private void SetLog()
        {
            string d = $"{converdone} done";
            string e = $"{converrs} rejected";
            if (converrs == 0)
                richlabel1.Text = d;
            else
            {
                richlabel1.Text = $"{d}, {e}";
                CheckKeyword(richlabel1, e, Color.Red);
            }
            CheckKeyword(richlabel1, d, Color.Green);
        }

        private void FolderSelect()
        {
            FolderPicker fp = new FolderPicker
            { Title = $"Select {Eblan.EblanRnd("folder").ToLower()} where to save the files" };
            if (!fp.ShowDialog(Handle)) return;
            folder = fp.ResultPath;
            ToggleUI(false);
            Worker.RunWorkerAsync();
        }

        public Bitmap BitmapFromFile(string filepath, bool cb1 = true)
        {
            switch (cb1 ? cbox1 : cbox2)
            {
                case "PNG":
                    return (Bitmap)Image.FromFile(filepath);
                case "JPGA":
                    return JPGA.FromFile(filepath).Source;
                case "TGA":
                    return TGA.FromFile(filepath).ToBitmap();
                case "BJPG":
                    return BJPG(filepath);
            }
            throw new FileLoadException($"Wait, what?");
        }

        public string FileFromBitmap(Bitmap img, string filepath)
        {
            filepath = IncrFilename(filepath += outputExt);
            switch (cbox2)
            {
                case "PNG":
                    img.Save(filepath, ImageFormat.Png);
                    break;
                case "JPGA":
                    JPGATag(ref filepath, false);
                    JPGA.FromBitmap(img).Save(IncrFilename(filepath));
                    break;
                case "TGA":
                    TGA.FromBitmap(img).Save(filepath);
                    break;
                case "BJPG":
                    using (LockBitmap lb = new LockBitmap(img))
                    {
                        lb.LockBits();
                        for (int i = 0; i < img.Height; i++)
                            for (int j = 0; j < img.Width; j++)
                                lb.SetPixel(j, i, AddBlack(lb.GetPixel(j, i)));
                        lb.UnlockBits();
                    }
                    img.Save(filepath, ImageFormat.Jpeg);
                    break;
            }
            return filepath;
        }

        private static Bitmap BJPG(string filename)
        {
            Bitmap img = (Bitmap)Image.FromFile(filename);
            Bitmap imgclone = img.Clone(new Rectangle(0, 0,
                    img.Width, img.Height), PixelFormat.Format32bppArgb);
            img.Dispose();
            using (LockBitmap lb = new LockBitmap(imgclone))
            {
                lb.LockBits();
                for (int i = 0; i < imgclone.Height; i++)
                    for (int j = 0; j < imgclone.Width; j++)
                        lb.SetPixel(j, i, ClearBlack(lb.GetPixel(j, i)));
                lb.UnlockBits();
            }
            return imgclone;
        }

        public static string JPGATag(ref string filepath, bool isRemove = true)
        {
            string folder = Path.GetDirectoryName(filepath);
            string file = Path.GetFileName(filepath);
            if (isRemove && file.StartsWith("_a_"))
                file = file.Remove(0, 3);
            else if (!isRemove && !file.StartsWith("_a_"))
                file = $"_a_{file}";
            return filepath = Path.Combine(folder, file);
        }

        public static string IncrFilename(string filepath)
        {
            string file = Path.GetFileNameWithoutExtension(filepath);
            string ext = Path.GetExtension(filepath);
            string folder = Path.GetDirectoryName(filepath);
            string filename = file;
            int count = 0;
            while (File.Exists(Path.Combine(folder, $"{filename}{ext}")))
            { count++; filename = $"{file} ({count})"; }
            return Path.Combine(folder, $"{filename}{ext}");
        }

        private void ComboboxToString(ComboBox cb, ref string str)
        {
            switch (cb.Text)
            {
                case "PNG":
                    str = ".png";
                    break;
                case "BJPG":
                case "JPGA":
                    str = ".jpg";
                    break;
                case "TGA":
                    str = ".tga";
                    break;
            }
        }

        private void AddItemsToCombobox2()
        {
            var test = comboBox1.Items.Cast<string>()
                .Where(o => o != (string)comboBox1.SelectedItem);
            comboBox2.Items.Clear();
            comboBox2.Items.AddRange(test.ToArray());
            comboBox2.SelectedIndex = 0;
        }

        public static Color ClearBlack(Color c) =>
            Color.FromArgb((int)(255 * c.GetBrightness()), c);

        public static Color AddBlack(Color c) =>
            Color.FromArgb(c.R * c.A / 255, c.G * c.A / 255, c.B * c.A / 255);

        public static Bitmap ResizeImage(Bitmap img, int size)
        {
            float nPercent, nPercentW, nPercentH;
            nPercentW = size / (float)img.Width;
            nPercentH = size / (float)img.Height;
            nPercent = nPercentH < nPercentW ? nPercentH : nPercentW;
            int destWidth = (int)(img.Width * nPercent);
            int destHeight = (int)(img.Height * nPercent);
            return new Bitmap(img, destWidth, destHeight);
        }

        public static void Clear(PictureBox pb)
        {
            if (pb.Image != null) pb.Image = null;
        }

        public static void Clear(Bitmap bmp) => bmp?.Dispose();

        public static void DefaultTextColor(RichTextLabel tl, string text)
        {
            tl.SelectAll();
            tl.SelectionColor = Color.Black;
            tl.Text = text;
        }

        public static Bitmap PicBoxDraw(string text)
        {
            Bitmap bmp = new Bitmap(300, 300);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.FillRectangle(Brushes.White, 0, 0, 300, 300);
                g.DrawRectangle(new Pen(Color.Black, 2), 0, 0, 150, 150);
                g.DrawRectangle(new Pen(Color.Black, 2), 150, 150, 150, 150);
                g.DrawRectangle(new Pen(Color.Black, 2), 150, 0, 150, 150);
                g.DrawRectangle(new Pen(Color.Black, 2), 0, 150, 150, 150);
                g.FillRectangle(Brushes.White, 5, 5, 290, 290);
                g.FillRectangle(Brushes.White, 0, 5, 300, 140);
                g.FillRectangle(Brushes.White, 5, 0, 140, 300);
                g.FillRectangle(Brushes.White, 0, 155, 300, 140);
                g.FillRectangle(Brushes.White, 155, 0, 140, 300);
                StringFormat sf = new StringFormat
                { Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center };
                g.DrawString(text, new Font("", 8.25f), Brushes.Black, new Point(150, 150), sf);
            }
            return bmp;
        }

        public static void CheckKeyword(RichTextBox rtb, string word, Color color, int startIndex = 0)
        {
            if (rtb.Text.Contains(word))
            {
                int index = -1;
                int selectStart = rtb.SelectionStart;
                while ((index = rtb.Text.IndexOf(word, index + 1)) != -1)
                {
                    rtb.Select(index + startIndex, word.Length);
                    rtb.SelectionColor = color;
                    rtb.Select(selectStart, 0);
                    rtb.SelectionColor = Color.Black;
                }
            }
        }
    }
}