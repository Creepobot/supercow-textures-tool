using EblanModule;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace SupercowTexturesTool
{
    //TODO: добавить еблану рандом из массива
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            pictureBox1.BackgroundImage = PicBoxDraw($"Imported {Eblan.EblanRnd("image").ToLower()} here\nDrag-n-drop support");
            pictureBox2.BackgroundImage = PicBoxDraw($"Converted {Eblan.EblanRnd("image").ToLower()} here\nClick to open folder");
        }

        private void Worker_DoWork(object s, DoWorkEventArgs e)
        {
            result = new string[files.Length][];
            for (int i = 0; i < files.Length; i++)
            {
                var timer = new Stopwatch();
                timer.Start();
                Bitmap importedImg = null;
                Bitmap exportedImg = null;
                result[i] = new string[4];
                result[i][0] = Path.GetFileNameWithoutExtension(files[i]);
                result[i][1] = $"{cbox1} => {cbox2}";
                try
                {
                    Clear(pictureBox1);
                    Clear(pictureBox2);
                    if (Path.GetExtension(files[i]).ToLower() != inputExt)
                        throw new FileLoadException($"Non-{inputExt} file");
                    Worker.ReportProgress(i + 1);
                    importedImg = BitmapFromFile(files[i]);
                    pictureBox1.Image = ResizeImage(importedImg, 300);
                    var test = Path.Combine(folder, JPGATag(ref result[i][0]));
                    exportedImg = BitmapFromFile(FileFromBitmap(importedImg, test), false);
                    pictureBox2.Image = ResizeImage(exportedImg, 300);
                    result[i][3] = "Done";
                    converdone++;
                }
                catch (Exception ex)
                {
                    result[i][3] = $"Error: {ex.Message}{ex.StackTrace}";
                    converrs++;
                }
                finally
                {
                    timer.Stop();
                    result[i][2] = $"{timer.ElapsedMilliseconds} ms";
                    Clear(importedImg);
                    Clear(exportedImg);
                }
            }
        }

        private void Worker_Changed(object s, ProgressChangedEventArgs e) =>
            DefaultTextColor(richlabel1, $"{e.ProgressPercentage}/{files.Length}");

        private void Worker_Completed(object s, RunWorkerCompletedEventArgs e)
        { SetLog(); BringToFront(); Activate(); ToggleUI(); }

        private void Drop(object s, DragEventArgs e)
        {
            object data = e.Data.GetData(DataFormats.FileDrop);
            if (data != null) files = (string[])data;
            FolderSelect();
        }

        private void Drag(object s, DragEventArgs e) =>
            e.Effect = DragDropEffects.Copy;

        private void PictureBox1_Click(object s, EventArgs e)
        {
            using (OpenFileDialog file = new OpenFileDialog
            {
                Filter = $"Image (*{inputExt})|*{inputExt}",
                RestoreDirectory = true, Multiselect = true,
                Title = $"Select {Eblan.EblanRnd("files", "eblans")} to convert"
            })
                if (file.ShowDialog() == DialogResult.OK)
                { files = file.FileNames; FolderSelect(); }
        }

        private void Form_Closing(object s, FormClosingEventArgs e)
        {
            if (!pictureBox1.Enabled)
            {
                DialogResult window = MessageBox.Show(
                    "You want to close the application during the conversion process." +
                    "\nIn this case, the exported files will be corrupted." +
                    "\n\nAre you sure?", Eblan.EblanRnd("You sure?", "Eblan?"),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                e.Cancel = window == DialogResult.No;
            }
        }

        private void PictureBox2_Click(object s, EventArgs e)
        {
            if (Directory.Exists(folder)) Process.Start(folder);
            else
                MessageBox.Show("Folder not found", Eblan.EblanRnd("Error", "Maybe eblan"),
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void ComboBox1_Changed(object s, EventArgs e)
        {
            cbox1 = (string)comboBox1.SelectedItem;
            ComboboxToString(comboBox1, ref inputExt);
            AddItemsToCombobox2();
        }

        private void ComboBox2_Changed(object s, EventArgs e)
        {
            cbox2 = (string)comboBox2.SelectedItem;
            ComboboxToString(comboBox2, ref outputExt);
            result = null;
            folder = "";
            Clear(pictureBox1);
            Clear(pictureBox2);
            DefaultTextColor(richlabel1, "Ready");
        }

        private void Info_Click(object s, EventArgs e)
        {
            using (About info = new About(this))
                info.ShowDialog();
        }

        private void Label_Click(object s, EventArgs e)
        {
            if (result != null)
                using (Result info = new Result(this, result))
                    info.ShowDialog();
        }

        private void Form1_KeyPress(object s, KeyPressEventArgs e)
        {
            string str = "eblan";
            if (str[ebl] == e.KeyChar)
                ebl++;
            else
                ebl = 0;

            if (ebl == str.Length)
            {
                using (UnmanagedMemoryStream stream = Properties.Resources.vineboom)
                    using (SoundPlayer snd = new SoundPlayer(stream))
                        snd.Play();
                pictureBox1.BackgroundImage = Properties.Resources.eblan;
                pictureBox2.BackgroundImage = Properties.Resources.eblan;
                KeyPress -= Form1_KeyPress;
            }
        }

        private void Label_Resized(object s, ContentsResizedEventArgs e) =>
            richlabel1.Width = e.NewRectangle.Width;
    }
}