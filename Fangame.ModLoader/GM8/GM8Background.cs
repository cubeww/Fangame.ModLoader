namespace Fangame.ModLoader.GM8;

public class GM8Background
{
    public string Name;
    public int Width;
    public int Height;
    public byte[]? Data;

    public GM8Background(GM8Stream s)
    {
        Name = s.ReadString();
        s.ReadInt32();
        s.ReadInt32();
        Width = s.ReadInt32();
        Height = s.ReadInt32();
        if (Width > 0 && Height > 0)
        {
            Data = s.ReadData();
        }
    }

    public void Save(GM8Stream s)
    {
        s.WriteString(Name);
        s.WriteInt32(710);
        s.WriteInt32(800);
        s.WriteInt32(Width);
        s.WriteInt32(Height);
        if (Width > 0 && Height > 0)
        {
            s.WriteData(Data);
        }
    }
}