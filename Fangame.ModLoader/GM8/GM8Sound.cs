namespace Fangame.ModLoader.GM8;

public class GM8Sound
{
    public string Name;
    public string Source;
    public string Extension;
    public byte[]? Data;
    public GM8SoundKind Kind;
    public double Volume;
    public double Pan;
    public bool Preload;
    public GM8SoundFX FX;

    public GM8Sound(GM8Stream s)
    {
        Name = s.ReadString();
        s.ReadInt32();
        Kind = (GM8SoundKind)s.ReadInt32();
        Extension = s.ReadString();
        Source = s.ReadString();
        if (s.ReadBoolean())
        {
            Data = s.ReadData();
        }
        int effects = s.ReadInt32();
        FX = new GM8SoundFX
        {
            Chorus = (effects & 0b1) != 0,
            Echo = (effects & 0b10) != 0,
            Flanger = (effects & 0b100) != 0,
            Gargle = (effects & 0b1000) != 0,
            Reverb = (effects & 0b10000) != 0
        };
        Volume = s.ReadDouble();
        Pan = s.ReadDouble();
        Preload = s.ReadBoolean();
    }

    public void Save(GM8Stream s)
    {
        s.WriteString(Name);
        s.WriteInt32(800);
        s.WriteInt32((int)Kind);
        s.WriteString(Extension);
        s.WriteString(Source);
        s.WriteBoolean(Data != null);
        if (Data != null)
        {
            s.WriteData(Data);
        }
        int x = 0;
        if (FX.Chorus) x |= 1;
        if (FX.Echo) x |= 1 << 1;
        if (FX.Flanger) x |= 1 << 2;
        if (FX.Gargle) x |= 1 << 3;
        if (FX.Reverb) x |= 1 << 4;
        s.WriteInt32(x);
        s.WriteDouble(Volume);
        s.WriteDouble(Pan);
        s.WriteBoolean(Preload);
    }
}

public enum GM8SoundKind
{
    Normal = 0,
    BackgroundMusic = 1,
    ThreeDimensional = 2,
    Multimedia = 3,
}

public class GM8SoundFX
{
    public bool Chorus;
    public bool Echo;
    public bool Flanger;
    public bool Gargle;
    public bool Reverb;
}