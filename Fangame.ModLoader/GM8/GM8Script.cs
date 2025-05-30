namespace Fangame.ModLoader.GM8;

public class GM8Script
{
    public string Name;
    public string Source;

    public GM8Script(GM8Stream s)
    {
        Name = s.ReadString();
        s.ReadInt32();
        Source = s.ReadString();
    }

    public void Save(GM8Stream s)
    {
        s.WriteString(Name);
        s.WriteInt32(800);
        s.WriteString(Source);
    }
}
