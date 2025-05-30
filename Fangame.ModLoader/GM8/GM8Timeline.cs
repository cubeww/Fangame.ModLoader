namespace Fangame.ModLoader.GM8;

public class GM8Timeline
{
    public string Name;
    public List<GM8TimelineMoment> Moments;

    public GM8Timeline(GM8Stream s)
    {
        Name = s.ReadString();
        s.ReadInt32();
        int momentCount = s.ReadInt32();
        Moments = new List<GM8TimelineMoment>(momentCount);
        for (int i = 0; i < momentCount; i++)
        {
            Moments.Add(new GM8TimelineMoment(s));
        }
    }

    public void Save(GM8Stream s)
    {
        s.WriteString(Name);
        s.WriteInt32(500);
        s.WriteInt32(Moments.Count);
        for (int i = 0; i < Moments.Count; i++)
        {
            Moments[i].Save(s);
        }
    }
}

public class GM8TimelineMoment
{
    public int Moment;
    public List<GM8Action> Actions;

    public GM8TimelineMoment(GM8Stream s)
    {
        Moment = s.ReadInt32();
        s.ReadInt32();
        int actionCount = s.ReadInt32();
        Actions = new List<GM8Action>(actionCount);
        for (int i = 0; i < actionCount; i++)
        {
            Actions.Add(new GM8Action(s));
        }
    }

    public void Save(GM8Stream s)
    {
        s.WriteInt32(Moment);
        s.WriteInt32(400);
        s.WriteInt32(Actions.Count);
        for (int i = 0; i < Actions.Count; i++)
        {
            Actions[i].Save(s);
        }
    }
}