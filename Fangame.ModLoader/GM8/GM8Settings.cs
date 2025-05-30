namespace Fangame.ModLoader.GM8;

public class GM8Settings
{
    public bool Fullscreen;
    public int Scaling;
    public bool InterpolatePixels;
    public int ClearColor;
    public bool AllowResize;
    public bool WindowOnTop;
    public bool DontDrawBorder;
    public bool DontShowButtons;
    public bool DisplayCursor;
    public bool FreezeOnLoseFocus;
    public bool DisableScreensaver;
    public bool ForceCpuRender;
    public bool SetResolution;
    public int ColorDepth;
    public int Resolution;
    public int Frequency;
    public bool Vsync;
    public bool EscCloseGame;
    public bool TreatCloseAsEsc;
    public bool F1HelpMenu;
    public bool F4FullscreenToggle;
    public bool F5SaveF6Load;
    public bool F9Screenshot;
    public int Priority;
    public byte[]? CustomLoadImage;
    public bool Transparent;
    public int Translucency;
    public int LoadingBar;
    public byte[]? Backdata;
    public byte[]? Frontdata;
    public bool ScaleProgressBar;
    public bool ShowErrorMessages;
    public bool LogErrors;
    public bool AlwaysAbort;
    public bool ZeroUninitializedVars;
    public bool ErrorOnUninitializedArgs;
    public bool SwapCreationEvents;

    public GM8Settings(GM8Stream s, int version)
    {
        Fullscreen = s.ReadBoolean();
        InterpolatePixels = s.ReadBoolean();
        DontDrawBorder = s.ReadBoolean();
        DisplayCursor = s.ReadBoolean();
        Scaling = s.ReadInt32();
        AllowResize = s.ReadBoolean();
        WindowOnTop = s.ReadBoolean();
        ClearColor = s.ReadInt32();
        SetResolution = s.ReadBoolean();
        ColorDepth = s.ReadInt32();
        Resolution = s.ReadInt32();
        Frequency = s.ReadInt32();
        DontShowButtons = s.ReadBoolean();
        Vsync = s.ReadBoolean();
        DisableScreensaver = s.ReadBoolean();
        F4FullscreenToggle = s.ReadBoolean();
        F1HelpMenu = s.ReadBoolean();
        EscCloseGame = s.ReadBoolean();
        F5SaveF6Load = s.ReadBoolean();
        F9Screenshot = s.ReadBoolean();
        TreatCloseAsEsc = s.ReadBoolean();
        Priority = s.ReadInt32();
        FreezeOnLoseFocus = s.ReadBoolean();
        LoadingBar = s.ReadInt32();
        if (LoadingBar != 0)
        {
            if (s.ReadBoolean())
                Backdata = s.ReadData();
            if (s.ReadBoolean())
                Frontdata = s.ReadData();
        }
        if (s.ReadBoolean())
            CustomLoadImage = s.ReadData();
        Transparent = s.ReadBoolean();
        Translucency = s.ReadInt32();
        ScaleProgressBar = s.ReadBoolean();
        ShowErrorMessages = s.ReadBoolean();
        LogErrors = s.ReadBoolean();
        AlwaysAbort = s.ReadBoolean();
        int x = s.ReadInt32();
        switch (version)
        {
            case 800:
                ZeroUninitializedVars = x != 0;
                ErrorOnUninitializedArgs = true;
                break;
            case 810:
                ZeroUninitializedVars = (x & 1) != 0;
                ErrorOnUninitializedArgs = (x & 2) != 0;
                break;
        }
    }

    public void Save(GM8Stream s, int version)
    {
        s.WriteBoolean(Fullscreen);
        s.WriteBoolean(InterpolatePixels);
        s.WriteBoolean(DontDrawBorder);
        s.WriteBoolean(DisplayCursor);
        s.WriteInt32(Scaling);
        s.WriteBoolean(AllowResize);
        s.WriteBoolean(WindowOnTop);
        s.WriteInt32(ClearColor);
        s.WriteBoolean(SetResolution);
        s.WriteInt32(ColorDepth);
        s.WriteInt32(Resolution);
        s.WriteInt32(Frequency);
        s.WriteBoolean(DontShowButtons);
        s.WriteBoolean(Vsync);
        s.WriteBoolean(DisableScreensaver);
        s.WriteBoolean(F4FullscreenToggle);
        s.WriteBoolean(F1HelpMenu);
        s.WriteBoolean(EscCloseGame);
        s.WriteBoolean(F5SaveF6Load);
        s.WriteBoolean(F9Screenshot);
        s.WriteBoolean(TreatCloseAsEsc);
        s.WriteInt32(Priority);
        s.WriteBoolean(FreezeOnLoseFocus);
        s.WriteInt32(LoadingBar);
        if (LoadingBar != 0)
        {
            s.WriteBoolean(Backdata != null);
            if (Backdata != null)
            {
                s.WriteData(Backdata);
            }
            s.WriteBoolean(Frontdata != null);
            if (Frontdata != null)
            {
                s.WriteData(Frontdata);
            }
        }
        s.WriteBoolean(CustomLoadImage != null);
        if (CustomLoadImage != null)
        {
            s.WriteData(CustomLoadImage);
        }
        s.WriteBoolean(Transparent);
        s.WriteInt32(Translucency);
        s.WriteBoolean(ScaleProgressBar);
        s.WriteBoolean(ShowErrorMessages);
        s.WriteBoolean(LogErrors);
        s.WriteBoolean(AlwaysAbort);
        if (version == 800)
        {
            s.WriteBoolean(ZeroUninitializedVars);
        }
        else if (version == 810)
        {
            int x = ZeroUninitializedVars ? 1 : 0;
            if (ErrorOnUninitializedArgs)
            {
                x |= 2;
            }
            s.WriteInt32(x);
        }
    }
}
