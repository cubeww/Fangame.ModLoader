namespace Fangame.ModLoader.GM8;

public class GM8Font
{
    public string Name;
    public string SysName;
    public int Size;
    public bool Bold;
    public bool Italic;
    public int RangeStart;
    public int RangeEnd;
    public int Charset;
    public int AALevel;
    public int[] DMap;
    public int MapWidth;
    public int MapHeight;
    public byte[] PixelMap;

    public GM8Font(GM8Stream s, int version)
    {
        Name = s.ReadString();
        s.ReadInt32();
        SysName = s.ReadString();
        Size = s.ReadInt32();
        Bold = s.ReadBoolean();
        Italic = s.ReadBoolean();
        RangeStart = s.ReadInt32();
        RangeEnd = s.ReadInt32();
        if (version == 810)
        {
            AALevel = (int)((RangeStart & 0xFF000000) >> 24);
            Charset = (RangeStart & 0x00FF0000) >> 16;
            RangeStart &= 0x0000FFFF;
        }
        DMap = new int[0x600];
        for (int i = 0; i < DMap.Length; i++)
        {
            DMap[i] = s.ReadInt32();
        }
        MapWidth = s.ReadInt32();
        MapHeight = s.ReadInt32();
        PixelMap = s.ReadData();
    }

    public void Save(GM8Stream s, int version)
    {
        s.WriteString(Name);
        s.WriteInt32(800);
        s.WriteString(SysName);
        s.WriteInt32(Size);
        s.WriteBoolean(Bold);
        s.WriteBoolean(Italic);
        if (version == 810)
        {
            s.WriteInt32(RangeStart | ((AALevel % 0x100) << 24) | ((Charset % 0x100) << 16));
        }
        else
        {
            s.WriteInt32(RangeStart);
        }
        s.WriteInt32(RangeEnd);
        for (int i = 0; i < DMap.Length; i++)
        {
            s.WriteInt32(DMap[i]);
        }
        s.WriteInt32(MapWidth);
        s.WriteInt32(MapHeight);
        s.WriteData(PixelMap);
    }
}
