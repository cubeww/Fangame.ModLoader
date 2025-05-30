namespace Fangame.ModLoader.GM8;

public class GM8Room
{
    public string Name;
    public string Caption;
    public int Width;
    public int Height;
    public int Speed;
    public bool Persistent;
    public int BgColor;
    public bool ClearScreen;
    public bool ClearRegion;
    public string CreationCode;
    public List<GM8RoomBackground> Backgrounds;
    public bool ViewsEnabled;
    public List<GM8RoomView> Views;
    public List<GM8RoomInstance> Instances;
    public List<GM8RoomTile> Tiles;
    public bool Uses810Features;
    public bool Uses811Features;

    public GM8Room(GM8Stream s, int version)
    {
        Name = s.ReadString();
        int entryVersion = s.ReadInt32();
        Uses810Features = entryVersion >= 810;
        Uses811Features = entryVersion >= 811;
        Caption = s.ReadString();
        Width = s.ReadInt32();
        Height = s.ReadInt32();
        Speed = s.ReadInt32();
        Persistent = s.ReadBoolean();
        BgColor = s.ReadInt32();
        int x = s.ReadInt32();
        if (version == 800)
        {
            ClearScreen = x != 0;
            ClearRegion = true;
        }
        else
        {
            ClearScreen = (x & 0b01) != 0;
            ClearRegion = (x & 0b10) == 0;
        }
        CreationCode = s.ReadString();
        int backgroundCount = s.ReadInt32();
        Backgrounds = new List<GM8RoomBackground>(backgroundCount);
        for (int i = 0; i < backgroundCount; i++)
        {
            Backgrounds.Add(new GM8RoomBackground(s));
        }
        ViewsEnabled = s.ReadBoolean();
        int viewCount = s.ReadInt32();
        Views = new List<GM8RoomView>(viewCount);
        for (int i = 0; i < viewCount; i++)
        {
            Views.Add(new GM8RoomView(s));
        }
        int instanceCount = s.ReadInt32();
        Instances = new List<GM8RoomInstance>(instanceCount);
        for (int i = 0; i < instanceCount; i++)
        {
            Instances.Add(new GM8RoomInstance(s, Uses810Features, Uses811Features));
        }
        int tileCount = s.ReadInt32();
        Tiles = new List<GM8RoomTile>(tileCount);
        for (int i = 0; i < tileCount; i++)
        {
            Tiles.Add(new GM8RoomTile(s, Uses810Features));
        }
    }

    public void Save(GM8Stream s, int version)
    {
        s.WriteString(Name);
        if (Uses811Features)
        {
            s.WriteInt32(811);
        }
        else if (Uses810Features)
        {
            s.WriteInt32(810);
        }
        else
        {
            s.WriteInt32(541);
        }
        s.WriteString(Caption);
        s.WriteInt32(Width);
        s.WriteInt32(Height);
        s.WriteInt32(Speed);
        s.WriteBoolean(Persistent);
        s.WriteInt32(BgColor);
        if (version == 800)
        {
            s.WriteBoolean(ClearScreen);
        }
        else
        {
            int x = ClearScreen ? 1 : 0;
            if (ClearRegion) x |= 2;
            s.WriteInt32(x);
        }
        s.WriteString(CreationCode);
        s.WriteInt32(Backgrounds.Count);
        for (int i = 0; i < Backgrounds.Count; i++)
        {
            Backgrounds[i].Save(s);
        }
        s.WriteBoolean(ViewsEnabled);
        s.WriteInt32(Views.Count);
        for (int i = 0; i < Views.Count; i++)
        {
            Views[i].Save(s);
        }
        s.WriteInt32(Instances.Count);
        for (int i = 0; i < Instances.Count; i++)
        {
            Instances[i].Save(s, Uses810Features, Uses811Features);
        }
        s.WriteInt32(Tiles.Count);
        for (int i = 0; i < Tiles.Count; i++)
        {
            Tiles[i].Save(s, Uses810Features);
        }
    }
}

public class GM8RoomBackground
{
    public bool VisibleOnStart;
    public bool IsForeground;
    public int SourceBg;
    public int XOffset;
    public int YOffset;
    public bool TileHorz;
    public bool TileVert;
    public int HSpeed;
    public int VSpeed;
    public bool Stretch;

    public GM8RoomBackground(GM8Stream s)
    {
        VisibleOnStart = s.ReadBoolean();
        IsForeground = s.ReadBoolean();
        SourceBg = s.ReadInt32();
        XOffset = s.ReadInt32();
        YOffset = s.ReadInt32();
        TileHorz = s.ReadBoolean();
        TileVert = s.ReadBoolean();
        HSpeed = s.ReadInt32();
        VSpeed = s.ReadInt32();
        Stretch = s.ReadBoolean();
    }

    public void Save(GM8Stream s)
    {
        s.WriteBoolean(VisibleOnStart);
        s.WriteBoolean(IsForeground);
        s.WriteInt32(SourceBg);
        s.WriteInt32(XOffset);
        s.WriteInt32(YOffset);
        s.WriteBoolean(TileHorz);
        s.WriteBoolean(TileVert);
        s.WriteInt32(HSpeed);
        s.WriteInt32(VSpeed);
        s.WriteBoolean(Stretch);
    }
}

public class GM8RoomInstance
{
    public int X;
    public int Y;
    public int Object;
    public int Id;
    public string CreationCode;
    public double XScale;
    public double YScale;
    public int Blend;
    public double Angle;

    public GM8RoomInstance(GM8Stream s, bool uses810Features, bool uses811Features)
    {
        X = s.ReadInt32();
        Y = s.ReadInt32();
        Object = s.ReadInt32();
        Id = s.ReadInt32();
        CreationCode = s.ReadString();
        XScale = uses810Features ? s.ReadDouble() : 1.0;
        YScale = uses810Features ? s.ReadDouble() : 1.0;
        Blend = uses810Features ? s.ReadInt32() : unchecked((int)0xFFFFFFFF);
        Angle = uses811Features ? s.ReadDouble() : 0.0;
    }

    public void Save(GM8Stream s, bool uses810Features, bool uses811Features)
    {
        s.WriteInt32(X);
        s.WriteInt32(Y);
        s.WriteInt32(Object);
        s.WriteInt32(Id);
        s.WriteString(CreationCode);
        if (uses810Features)
        {
            s.WriteDouble(XScale);
            s.WriteDouble(YScale);
            s.WriteInt32(Blend);
        }
        if (uses811Features)
        {
            s.WriteDouble(Angle);
        }
    }
}

public class GM8RoomTile
{
    public int X;
    public int Y;
    public int SourceBg;
    public int TileX;
    public int TileY;
    public int Width;
    public int Height;
    public int Depth;
    public int Id;
    public double XScale;
    public double YScale;
    public int Blend;

    public GM8RoomTile(GM8Stream s, bool uses810Features)
    {
        X = s.ReadInt32();
        Y = s.ReadInt32();
        SourceBg = s.ReadInt32();
        TileX = s.ReadInt32();
        TileY = s.ReadInt32();
        Width = s.ReadInt32();
        Height = s.ReadInt32();
        Depth = s.ReadInt32();
        Id = s.ReadInt32();
        XScale = uses810Features ? s.ReadDouble() : 1.0;
        YScale = uses810Features ? s.ReadDouble() : 1.0;
        Blend = uses810Features ? s.ReadInt32() : unchecked((int)0xFFFFFFFF);
    }

    public void Save(GM8Stream s, bool uses810Features)
    {
        s.WriteInt32(X);
        s.WriteInt32(Y);
        s.WriteInt32(SourceBg);
        s.WriteInt32(TileX);
        s.WriteInt32(TileY);
        s.WriteInt32(Width);
        s.WriteInt32(Height);
        s.WriteInt32(Depth);
        s.WriteInt32(Id);
        if (uses810Features)
        {
            s.WriteDouble(XScale);
            s.WriteDouble(YScale);
            s.WriteInt32(Blend);
        }
    }
}

public class GM8RoomView
{
    public bool Visible;
    public int SourceX;
    public int SourceY;
    public int SourceW;
    public int SourceH;
    public int PortX;
    public int PortY;
    public int PortW;
    public int PortH;
    public int HBorder;
    public int VBorder;
    public int HSpeed;
    public int VSpeed;
    public int Target;

    public GM8RoomView(GM8Stream s)
    {
        Visible = s.ReadBoolean();
        SourceX = s.ReadInt32();
        SourceY = s.ReadInt32();
        SourceW = s.ReadInt32();
        SourceH = s.ReadInt32();
        PortX = s.ReadInt32();
        PortY = s.ReadInt32();
        PortW = s.ReadInt32();
        PortH = s.ReadInt32();
        HBorder = s.ReadInt32();
        VBorder = s.ReadInt32();
        HSpeed = s.ReadInt32();
        VSpeed = s.ReadInt32();
        Target = s.ReadInt32();
    }

    public void Save(GM8Stream s)
    {
        s.WriteBoolean(Visible);
        s.WriteInt32(SourceX);
        s.WriteInt32(SourceY);
        s.WriteInt32(SourceW);
        s.WriteInt32(SourceH);
        s.WriteInt32(PortX);
        s.WriteInt32(PortY);
        s.WriteInt32(PortW);
        s.WriteInt32(PortH);
        s.WriteInt32(HBorder);
        s.WriteInt32(VBorder);
        s.WriteInt32(HSpeed);
        s.WriteInt32(VSpeed);
        s.WriteInt32(Target);
    }
}