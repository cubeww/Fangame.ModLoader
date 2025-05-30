namespace Fangame.ModLoader.GM8;

public class GM8HelpDialog
{
    public int BgColor;
    public bool NewWindow;
    public string Caption;
    public int Left;
    public int Top;
    public int Width;
    public int Height;
    public bool Border;
    public bool Resizable;
    public bool WindowOnTop;
    public bool FreezeGame;
    public string Info;

    public GM8HelpDialog(GM8Stream s)
    {
        BgColor = s.ReadInt32();
        NewWindow = s.ReadBoolean();
        Caption = s.ReadString();
        Left = s.ReadInt32();
        Top = s.ReadInt32();
        Width = s.ReadInt32();
        Height = s.ReadInt32();
        Border = s.ReadBoolean();
        Resizable = s.ReadBoolean();
        WindowOnTop = s.ReadBoolean();
        FreezeGame = s.ReadBoolean();
        Info = s.ReadString();
    }

    public void Save(GM8Stream s)
    {
        s.WriteInt32(BgColor);
        s.WriteBoolean(NewWindow);
        s.WriteString(Caption);
        s.WriteInt32(Left);
        s.WriteInt32(Top);
        s.WriteInt32(Width);
        s.WriteInt32(Height);
        s.WriteBoolean(Border);
        s.WriteBoolean(Resizable);
        s.WriteBoolean(WindowOnTop);
        s.WriteBoolean(FreezeGame);
        s.WriteString(Info);
    }
}
