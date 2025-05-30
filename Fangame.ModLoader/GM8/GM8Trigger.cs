namespace Fangame.ModLoader.GM8;

public class GM8Trigger
{
    public string Name;
    public string Condition;
    public GM8TriggerKind Moment;
    public string ConstantName;

    public GM8Trigger(GM8Stream s)
    {
        s.ReadInt32();
        Name = s.ReadString();
        Condition = s.ReadString();
        Moment = (GM8TriggerKind)s.ReadInt32();
        ConstantName = s.ReadString();
    }

    public void Save(GM8Stream s)
    {
        s.WriteInt32(800);
        s.WriteString(Name);
        s.WriteString(Condition);
        s.WriteInt32((int)Moment);
        s.WriteString(ConstantName);
    }
}

public enum GM8TriggerKind
{
    Step = 0,
    BeginStep = 1,
    EndStep = 2,
}