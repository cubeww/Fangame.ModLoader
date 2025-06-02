namespace Fangame.ModLoader.GM8;

public class GM8Sprite
{
    public string Name;
    public int OriginX;
    public int OriginY;
    public List<GM8SpriteImage> Images;
    public List<GM8CollisionMask> CollisionMasks;
    public bool SepMasks;

    public GM8Sprite()
    {
        Name = "";
        Images = [];
        CollisionMasks = [];
    }

    public GM8Sprite(GM8Stream s)
    {
        Name = s.ReadString();
        s.ReadInt32();
        OriginX = s.ReadInt32();
        OriginY = s.ReadInt32();
        int frameCount = s.ReadInt32();
        Images = new List<GM8SpriteImage>(frameCount);
        CollisionMasks = new List<GM8CollisionMask>(frameCount);
        for (int i = 0; i < frameCount; i++)
        {
            Images.Add(new GM8SpriteImage(s));
        }
        SepMasks = s.ReadBoolean();
        if (frameCount > 0)
        {
            int maskCount = SepMasks ? Images.Count : 1;
            for (int i = 0; i < maskCount; i++)
            {
                CollisionMasks.Add(new GM8CollisionMask(s));
            }
        }
    }

    public void Save(GM8Stream s)
    {
        s.WriteString(Name);
        s.WriteInt32(800);
        s.WriteInt32(OriginX);
        s.WriteInt32(OriginY);
        s.WriteInt32(Images.Count);
        for (int i = 0; i < Images.Count; i++)
        {
            Images[i].Save(s);
        }
        s.WriteBoolean(SepMasks);
        if (Images.Count > 0)
        {
            for (int i = 0; i < CollisionMasks.Count; i++)
            {
                CollisionMasks[i].Save(s);
            }
        }
    }
}

public class GM8SpriteImage
{
    public int Width;
    public int Height;
    public byte[] Data;

    public GM8SpriteImage(int width, int height, byte[] data)
    {
        Width = width;
        Height = height;
        Data = data;
    }

    public GM8SpriteImage(GM8Stream s)
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

public class GM8CollisionMask
{
    public int Width;
    public int Height;
    public int BBoxLeft;
    public int BBoxTop;
    public int BBoxRight;
    public int BBoxBottom;
    public bool[] Data;

    public GM8CollisionMask(GM8SpriteImage image)
    {
        Width = image.Width;
        Height = image.Height;
        Data = new bool[Width * Height];
        BBoxLeft = Width - 1;
        BBoxRight = 0;
        BBoxTop = Height - 1;
        BBoxBottom = 0;
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                int idx = x + y * Height;
                bool hasAlpha = image.Data[idx * 4 + 3] > 0;
                Data[idx] = hasAlpha;
                if (hasAlpha)
                {
                    BBoxLeft = int.Min(BBoxLeft, x);
                    BBoxRight = int.Max(BBoxRight, x);
                    BBoxTop = int.Min(BBoxTop, y);
                    BBoxBottom = int.Max(BBoxBottom, y);
                }
            }
        }
    }

    public GM8CollisionMask(GM8Stream s)
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