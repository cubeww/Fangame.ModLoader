namespace Fangame.ModLoader.GM8;

public class GM8IncludedFile
{
    public string FileName;
    public string SourcePath;
    public bool DataExists;
    public int SourceLength;
    public bool StoredInGmk;
    public byte[]? EmbeddedData;
    public GM8IncludedFileExportSettings ExportSettings;
    public string? CustomFolderPath;
    public bool OverwriteFile;
    public bool FreeMemory;
    public bool RemoveAtEnd;

    public GM8IncludedFile(GM8Stream s)
    {
        s.ReadInt32();
        FileName = s.ReadString();
        SourcePath = s.ReadString();
        DataExists = s.ReadBoolean();
        SourceLength = s.ReadInt32();
        StoredInGmk = s.ReadBoolean();
        if (StoredInGmk && DataExists)
        {
            EmbeddedData = s.ReadData();
        }
        ExportSettings = (GM8IncludedFileExportSettings)s.ReadInt32();
        CustomFolderPath = s.ReadString();
        OverwriteFile = s.ReadBoolean();
        FreeMemory = s.ReadBoolean();
        RemoveAtEnd = s.ReadBoolean();
    }

    public void Save(GM8Stream s)
    {
        s.WriteInt32(800);
        s.WriteString(FileName);
        s.WriteString(SourcePath);
        s.WriteBoolean(DataExists);
        s.WriteInt32(SourceLength);
        s.WriteBoolean(StoredInGmk);
        if (StoredInGmk && DataExists)
        {
            s.WriteData(EmbeddedData);
        }
        s.WriteInt32((int)ExportSettings);
        s.WriteString(CustomFolderPath ?? "");
        s.WriteBoolean(OverwriteFile);
        s.WriteBoolean(FreeMemory);
        s.WriteBoolean(RemoveAtEnd);
    }
}

public enum GM8IncludedFileExportSettings
{
    NoExport = 0,
    TempFolder = 1,
    GameFolder = 2,
    CustomFolder = 3,
}