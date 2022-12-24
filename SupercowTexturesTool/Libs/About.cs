using EblanModule;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

public class About : Form
{
    private Button button1;
    private TextBox textBox1;
    private Label label1;
    private Label label2;
    private Label label3;
    private LinkLabel linkLabel1;
    private LinkLabel linkLabel2;
    private readonly Container components = null;

    public About(Form owner)
    {
        Owner = owner;
        InitializeComponent();
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
        button1 = new System.Windows.Forms.Button();
        textBox1 = new System.Windows.Forms.TextBox();
        label1 = new System.Windows.Forms.Label();
        label2 = new System.Windows.Forms.Label();
        label3 = new System.Windows.Forms.Label();
        linkLabel1 = new System.Windows.Forms.LinkLabel();
        linkLabel2 = new System.Windows.Forms.LinkLabel();
        SuspendLayout();
        // 
        // button1
        // 
        button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        button1.Location = new System.Drawing.Point(212, 206);
        button1.Name = "button1";
        button1.Size = new System.Drawing.Size(75, 23);
        button1.TabIndex = 1;
        button1.Text = "&OK";
        button1.UseVisualStyleBackColor = true;
        // 
        // textBox1
        // 
        textBox1.Location = new System.Drawing.Point(12, 61);
        textBox1.Multiline = true;
        textBox1.Name = "textBox1";
        textBox1.ReadOnly = true;
        textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
        textBox1.Size = new System.Drawing.Size(477, 134);
        textBox1.TabIndex = 2;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new System.Drawing.Point(12, 9);
        label1.Name = "label1";
        label1.Size = new System.Drawing.Size(83, 13);
        label1.TabIndex = 3;
        label1.Text = "Program Name: ";
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new System.Drawing.Point(12, 27);
        label2.Name = "label2";
        label2.Size = new System.Drawing.Size(48, 13);
        label2.TabIndex = 4;
        label2.Text = "Version: ";
        // 
        // label3
        // 
        label3.AutoSize = true;
        label3.Location = new System.Drawing.Point(12, 45);
        label3.Name = "label3";
        label3.Size = new System.Drawing.Size(63, 13);
        label3.TabIndex = 5;
        label3.Text = "Description:";
        // 
        // linkLabel1
        // 
        linkLabel1.AutoSize = true;
        linkLabel1.Location = new System.Drawing.Point(414, 217);
        linkLabel1.Name = "linkLabel1";
        linkLabel1.Size = new System.Drawing.Size(75, 13);
        linkLabel1.TabIndex = 6;
        linkLabel1.TabStop = true;
        linkLabel1.Tag = "https://discord.com/invite/JzCvwh5";
        linkLabel1.Text = "Discord server";
        linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(LinkLabel_Clicked);
        // 
        // linkLabel2
        // 
        linkLabel2.AutoSize = true;
        linkLabel2.Location = new System.Drawing.Point(421, 203);
        linkLabel2.Name = "linkLabel2";
        linkLabel2.Size = new System.Drawing.Size(68, 13);
        linkLabel2.TabIndex = 7;
        linkLabel2.TabStop = true;
        linkLabel2.Tag = "https://github.com/Creepobot/SupercowTexturesTool";
        linkLabel2.Text = "Source code";
        linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(LinkLabel_Clicked);
        // 
        // About
        // 
        AutoScaleBaseSize = new System.Drawing.Size(5, 13);
        ClientSize = new System.Drawing.Size(501, 236);
        ControlBox = false;
        Controls.Add(linkLabel2);
        Controls.Add(linkLabel1);
        Controls.Add(label3);
        Controls.Add(label2);
        Controls.Add(label1);
        Controls.Add(textBox1);
        Controls.Add(button1);
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "About";
        ShowInTaskbar = false;
        Text = "About";
        Load += new System.EventHandler(About_Load);
        ResumeLayout(false);
        PerformLayout();

    }
    #endregion
    private void About_Load(object s, EventArgs e)
    {
        ComponentResourceManager resources = new ComponentResourceManager(typeof(About));
        textBox1.Text = Eblan.EblanRnd(resources.GetString("1"), resources.GetString("2"));
        AssemblyName an = Assembly.GetExecutingAssembly().GetName();
        label1.Text += an.Name;
        label2.Text += an.Version.ToString();
    }

    private void LinkLabel_Clicked(object s, LinkLabelLinkClickedEventArgs e) =>
        Process.Start((string)(s as LinkLabel).Tag);
}