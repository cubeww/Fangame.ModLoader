namespace Fangame.ModLoader.GM8;

public class GM8Extension
{
    public string Name;
    public string FolderName;
    public List<GM8ExtensionFile> Files;
    public byte[] Contents;

    public GM8Extension(GM8Stream s)
    {
        s.ReadInt32();
        Name = s.ReadString();
        FolderName = s.ReadString();
        int fileCount = s.ReadInt32();
        Files = new List<GM8ExtensionFile>(fileCount);
        for (int i = 0; i < fileCount; i++)
        {
            Files.Add(new GM8ExtensionFile(s));
        }
        Contents = s.ReadData();
    }

    public void Save(GM8Stream s)
    {
        s.WriteInt32(700);
        s.WriteString(Name);
        s.WriteString(FolderName);
        s.WriteInt32(Files.Count);
        foreach (var file in Files)
        {
            file.Save(s);
        }
        s.WriteData(Contents);
    }
}

public class GM8ExtensionFile
{
    public string Name;
    public GM8ExtensionFileKind Kind;
    public string Initializer;
    public string Finalizer;
    public List<GM8ExtensionFileFunction> Functions;
    public List<GM8ExtensionFileConst> Consts;

    public GM8ExtensionFile(GM8Stream s)
    {
        s.ReadInt32();
        Name = s.ReadString();
        Kind = (GM8ExtensionFileKind)s.ReadInt32();
        Initializer = s.ReadString();
        Finalizer = s.ReadString();
        int functionCount = s.ReadInt32();
        Functions = new List<GM8ExtensionFileFunction>(functionCount);
        for (int i = 0; i < functionCount; i++)
        {
            Functions.Add(new GM8ExtensionFileFunction(s));
        }
        int constCount = s.ReadInt32();
        Consts = new List<GM8ExtensionFileConst>(constCount);
        for (int i = 0; i < constCount; i++)
        {
            Consts.Add(new GM8ExtensionFileConst(s));
        }
    }

    public void Save(GM8Stream s)
    {
        s.WriteInt32(700);
        s.WriteString(Name);
        s.WriteInt32((int)Kind);
        s.WriteString(Initializer);
        s.WriteString(Finalizer);
        s.WriteInt32(Functions.Count);
        foreach (var function in Functions)
        {
            function.Save(s);
        }
        s.WriteInt32(Consts.Count);
        foreach (var cs in Consts)
        {
            cs.Save(s);
        }
    }
}

public class GM8ExtensionFileConst
{
    public string Name;
    public string Value;

    public GM8ExtensionFileConst(GM8Stream s)
    {
        s.ReadInt32();
        Name = s.ReadString();
        Value = s.ReadString();
    }

    public void Save(GM8Stream s)
    {
        s.WriteInt32(700);
        s.WriteString(Name);
        s.WriteString(Value);
    }
}

public enum GM8ExtensionFileKind
{
    DynamicLibrary = 1,
    GmlScript = 2,
    ActionLibrary = 3,
    Other = 4,
}

public class GM8ExtensionFileFunction
{
    public string Name;
    public string ExternalName;
    public GM8ExtensionCallingConvention Converntion;
    public int Id;
    public int ArgCount;
    public GM8ExtensionFunctionValueKind[] ArgTypes;
    public GM8ExtensionFunctionValueKind ReturnType;

    public GM8ExtensionFileFunction(GM8Stream s)
    {
        s.ReadInt32();
        Name = s.ReadString();
        ExternalName = s.ReadString();
        Converntion = (GM8ExtensionCallingConvention)s.ReadInt32();
        Id = s.ReadInt32();
        ArgCount = s.ReadInt32();
        ArgTypes = new GM8ExtensionFunctionValueKind[17];
        for (int i = 0; i < ArgTypes.Length; i++)
        {
            ArgTypes[i] = (GM8ExtensionFunctionValueKind)s.ReadInt32();
        }
        ReturnType = (GM8ExtensionFunctionValueKind)s.ReadInt32();
    }

    public void Save(GM8Stream s)
    {
        s.WriteInt32(700);
        s.WriteString(Name);
        s.WriteString(ExternalName);
        s.WriteInt32((int)Converntion);
        s.WriteInt32(Id);
        s.WriteInt32(ArgCount);
        for (int i = 0; i < ArgTypes.Length; i++)
        {
            s.WriteInt32((int)ArgTypes[i]);
        }
        s.WriteInt32((int)ReturnType);
    }
}

public enum GM8ExtensionFunctionValueKind
{
    GMString = 1,
    GMReal = 2,
}

public enum GM8ExtensionCallingConvention
{
    Gml = 2,
    Stdcall = 11,
    Cdecl = 12,
    Unknown,
}