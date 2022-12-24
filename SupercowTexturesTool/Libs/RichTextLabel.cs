using System.Windows.Forms;

public class RichTextLabel : RichTextBox
{
    public RichTextLabel()
    {
        ReadOnly = true;
        BorderStyle = BorderStyle.None;
        SetStyle(ControlStyles.Selectable, false);
        SetStyle(ControlStyles.UserMouse, true);
        SetStyle(ControlStyles.SupportsTransparentBackColor, true);
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == 0x204) return;
        if (m.Msg == 0x205) return;
        base.WndProc(ref m);
    }
}