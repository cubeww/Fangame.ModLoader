using System.Text;

namespace Fangame.ModLoader.Gui;

public class ControlWriter : TextWriter
{
    private Control textbox;
    public ControlWriter(Control textbox)
    {
        this.textbox = textbox;
    }

    public override void Write(char value)
    {
        textbox.Invoke(() => textbox.Text += value);
    }

    public override void Write(string? value)
    {
        textbox.Invoke(() => textbox.Text += value);
    }

    public override Encoding Encoding
    {
        get { return Encoding.ASCII; }
    }
}
