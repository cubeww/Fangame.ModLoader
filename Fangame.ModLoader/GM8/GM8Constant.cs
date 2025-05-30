namespace Fangame.ModLoader.GM8;

public class GM8Constant
{
    public string Name;
    public string Expression;

    public GM8Constant(GM8Stream s)
    {
        Name = s.ReadString();
        Expression = s.ReadString();
    }

    public void Save(GM8Stream s)
    {
        s.WriteString(Name);
        s.WriteString(Expression);
    }
}
