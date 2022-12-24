using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

public class Result : Form
{
    private IContainer components;
    private ListView listView1;
    private ColumnHeader column1;
    private ColumnHeader column2;
    private ColumnHeader column3;
    private ToolTip toolTip1;
    private ColumnHeader column4;
    private Button button1;

    public Result(Form owner, in string[][] result)
    {
        Owner = owner;
        InitializeComponent();
        foreach (string[] i in result)
            listView1.Items.Add(new ListViewItem(i));
        foreach (ColumnHeader i in listView1.Columns)
            i.Width = -1;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            components?.Dispose();
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code
    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            this.listView1 = new System.Windows.Forms.ListView();
            this.column1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.column2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.column4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.column3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button1 = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.column1,
            this.column2,
            this.column4,
            this.column3});
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(12, 12);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(329, 224);
            this.listView1.TabIndex = 10;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ListView1_MouseClick);
            // 
            // column1
            // 
            this.column1.Text = "File";
            // 
            // column2
            // 
            this.column2.Text = "Type";
            this.column2.Width = 80;
            // 
            // column4
            // 
            this.column4.Text = "Time";
            // 
            // column3
            // 
            this.column3.Text = "Status";
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(134, 242);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "&OK";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // Result
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(354, 276);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Result";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Result";
            this.ResumeLayout(false);

    }
    #endregion

    private void ListView1_MouseClick(object s, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            ListViewItem item = listView1.GetItemAt(e.X, e.Y);
            if (item != null)
            {
                item.Selected = true;
                string text = "";
                for (int i = 0; i < item.SubItems.Count; i++)
                {
                    text += item.SubItems[i].Text;
                    if (i != item.SubItems.Count - 1)
                        text += ", ";
                }
                Clipboard.SetText(text);
            }
        }
        toolTip1.Show("Result copied", listView1, new Point(e.X, e.Y), 1000);
    }
}