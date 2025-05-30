namespace Fangame.ModLoader.GM8;

public class GM8Action
{
    public int Id;
    public int AppliesTo;
    public bool IsCondition;
    public bool InvertCondition;
    public bool IsRelative;
    public int LibId;
    public int ActionKind;
    public int ExecutionType;
    public int CanBeRelative;
    public bool AppliesToSomething;
    public string FnName;
    public string FnCode;
    public int ParamCount;
    public int[] ParamTypes;
    public string[] ParamStrings;

    public GM8Action(GM8Stream s)
    {
        s.ReadInt32();
        LibId = s.ReadInt32();
        Id = s.ReadInt32();
        ActionKind = s.ReadInt32();
        CanBeRelative = s.ReadInt32();
        IsCondition = s.ReadBoolean();
        AppliesToSomething = s.ReadBoolean();
        ExecutionType = s.ReadInt32();
        FnName = s.ReadString();
        FnCode = s.ReadString();
        ParamCount = s.ReadInt32();
        ParamTypes = new int[s.ReadInt32()];
        for (int i = 0; i < ParamTypes.Length; i++)
        {
            ParamTypes[i] = s.ReadInt32();
        }
        AppliesTo = s.ReadInt32();
        IsRelative = s.ReadBoolean();
        ParamStrings = new string[s.ReadInt32()];
        for (int i = 0; i < ParamStrings.Length; i++)
        {
            ParamStrings[i] = s.ReadString();
        }
        InvertCondition = s.ReadBoolean();
    }

    public void Save(GM8Stream s)
    {
        s.WriteInt32(440);
        s.WriteInt32(LibId);
        s.WriteInt32(Id);
        s.WriteInt32(ActionKind);
        s.WriteInt32(CanBeRelative);
        s.WriteBoolean(IsCondition);
        s.WriteBoolean(AppliesToSomething);
        s.WriteInt32(ExecutionType);
        s.WriteString(FnName);
        s.WriteString(FnCode);
        s.WriteInt32(ParamCount);
        s.WriteInt32(ParamTypes.Length);
        for (int i = 0; i < ParamTypes.Length; i++)
        {
            s.WriteInt32(ParamTypes[i]);
        }
        s.WriteInt32(AppliesTo);
        s.WriteBoolean(IsRelative);
        s.WriteInt32(ParamStrings.Length);
        for (int i = 0; i < ParamStrings.Length; i++)
        {
            s.WriteString(ParamStrings[i]);
        }
        s.WriteBoolean(InvertCondition);
    }

    public GM8Action()
    {
        FnName = "";
        FnCode = "";
        ParamTypes = new int[8];
        ParamStrings = new string[8];
        Array.Fill(ParamStrings, "");
    }

    public static GM8Action CreateCode(string code)
    {
        GM8Action action = new GM8Action
        {
            ActionKind = 7,
            AppliesTo = -1,
            AppliesToSomething = true,
            ExecutionType = 2,
            Id = 603,
            LibId = 1,
            ParamCount = 1,
        };
        action.ParamStrings[0] = code;
        action.ParamTypes[0] = 1;
        return action;
    }
}