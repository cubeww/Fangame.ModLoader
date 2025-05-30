namespace Fangame.ModLoader.GM8;

public class GM8Sprite
{
    public string Name;
    public int OriginX;
    public int OriginY;
    public List<GM8SpriteFrame> Frames;
    public List<GM8CollisionMap> Colliders;
    public bool PerFrameColliders;

    public GM8Sprite(GM8Stream s)
    {
        Name = s.ReadString();
        s.ReadInt32();
        OriginX = s.ReadInt32();
        OriginY = s.ReadInt32();
        int frameCount = s.ReadInt32();
        Frames = new List<GM8SpriteFrame>(frameCount);
        Colliders = new List<GM8CollisionMap>(frameCount);
        for (int i = 0; i < frameCount; i++)
        {
            Frames.Add(new GM8SpriteFrame(s));
        }
        PerFrameColliders = s.ReadBoolean();
        if (frameCount > 0)
        {
            int colliderCount = PerFrameColliders ? Frames.Count : 1;
            for (int i = 0; i < colliderCount; i++)
            {
                Colliders.Add(new GM8CollisionMap(s));
            }
        }
    }

    public void Save(GM8Stream s)
    {
        s.WriteString(Name);
        s.WriteInt32(800);
        s.WriteInt32(OriginX);
        s.WriteInt32(OriginY);
        s.WriteInt32(Frames.Count);
        for (int i = 0; i < Frames.Count; i++)
        {
            Frames[i].Save(s);
        }
        s.WriteBoolean(PerFrameColliders);
        if (Frames.Count > 0)
        {
            for (int i = 0; i < Colliders.Count; i++)
            {
                Colliders[i].Save(s);
            }
        }
    }
}

public class GM8SpriteFrame
{
    public int Width;
    public int Height;
    public byte[] Data;

    public GM8SpriteFrame(GM8Stream s)
    {
        s.ReadInt32();
        Width = s.ReadInt32();
        Height = s.ReadInt32();
        Data = s.ReadData();
    }

    public void Save(GM8Stream s)
    {
        s.WriteInt32(800);
        s.WriteInt32(Width);
        s.WriteInt32(Height);
        s.WriteData(Data);
    }
}

public class GM8CollisionMap
{
    public int Width;
    public int Height;
    public int BBoxLeft;
    public int BBoxTop;
    public int BBoxRight;
    public int BBoxBottom;
    public bool[] Data;

    public GM8CollisionMap(GM8Stream s)
    {
        s.ReadInt32();
        Width = s.ReadInt32();
        Height = s.ReadInt32();
        BBoxLeft = s.ReadInt32();
        BBoxRight = s.ReadInt32();
        BBoxBottom = s.ReadInt32();
        BBoxTop = s.ReadInt32();
        int pixelCount = Width * Height;
        Data = new bool[pixelCount];
        for (int i = 0; i < pixelCount; i++)
        {
            Data[i] = s.ReadBoolean();
        }
    }

    public void Save(GM8Stream s)
    {
        s.WriteInt32(800);
        s.WriteInt32(Width);
        s.WriteInt32(Height);
        s.WriteInt32(BBoxLeft);
        s.WriteInt32(BBoxRight);
        s.WriteInt32(BBoxBottom);
        s.WriteInt32(BBoxTop);
        int pixelCount = Width * Height;
        for (int i = 0; i < pixelCount; i++)
        {
            s.WriteBoolean(Data[i]);
        }
    }
}