namespace Fangame.ModLoader.GM8;

public class GM8Path
{
    public string Name;
    public GM8PathConnectionKind Connection;
    public int Precision;
    public bool Closed;
    public List<GM8PathPoint> Points;

    public GM8Path(GM8Stream s)
    {
        Name = s.ReadString();
        s.ReadInt32();
        Connection = (GM8PathConnectionKind)s.ReadInt32();
        Closed = s.ReadBoolean();
        Precision = s.ReadInt32();
        int pointCount = s.ReadInt32();
        Points = new List<GM8PathPoint>(pointCount);
        for (int i = 0; i < pointCount; i++)
        {
            Points.Add(new GM8PathPoint(s.ReadDouble(), s.ReadDouble(), s.ReadDouble()));
        }
    }

    public void Save(GM8Stream s)
    {
        s.WriteString(Name);
        s.WriteInt32(530);
        s.WriteInt32((int)Connection);
        s.WriteBoolean(Closed);
        s.WriteInt32(Precision);
        s.WriteInt32(Points.Count);
        for (int i = 0; i < Points.Count; i++)
        {
            s.WriteDouble(Points[i].X);
            s.WriteDouble(Points[i].Y);
            s.WriteDouble(Points[i].Speed);
        }
    }
}

public struct GM8PathPoint
{
    public double X;
    public double Y;
    public double Speed;

    public GM8PathPoint(double x, double y, double speed)
    {
        X = x;
        Y = y;
        Speed = speed;
    }
}

public enum GM8PathConnectionKind
{
    StraightLine = 0,
    SmoothCurve = 1,
}