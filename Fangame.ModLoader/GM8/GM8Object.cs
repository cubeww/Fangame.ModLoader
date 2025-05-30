namespace Fangame.ModLoader.GM8;

public class GM8Object
{
    public string Name;
    public int SpriteIndex;
    public bool Solid;
    public bool Visible;
    public int Depth;
    public bool Persistent;
    public int ParentIndex;
    public int MaskIndex;
    public List<List<GM8ObjectEvent>> Events;

    public GM8Object()
    {
        Name = "";
        SpriteIndex = -1;
        ParentIndex = -100;
        MaskIndex = -1;
        Events = new List<List<GM8ObjectEvent>>(12);
        for (int i = 0; i < 12; i++)
        {
            Events.Add(new List<GM8ObjectEvent>());
        }
    }

    public GM8Object(GM8Stream s)
    {
        Name = s.ReadString();
        s.ReadInt32();
        SpriteIndex = s.ReadInt32();
        Solid = s.ReadBoolean();
        Visible = s.ReadBoolean();
        Depth = s.ReadInt32();
        Persistent = s.ReadBoolean();
        ParentIndex = s.ReadInt32();
        MaskIndex = s.ReadInt32();
        int eventCount = s.ReadInt32();
        Events = new List<List<GM8ObjectEvent>>(eventCount + 1);
        for (int i = 0; i <= eventCount; i++)
        {
            Events.Add(new List<GM8ObjectEvent>());
            while (true)
            {
                int subtype = s.ReadInt32();
                if (subtype == -1) break;
                Events[i].Add(new GM8ObjectEvent(subtype, s));
            }
        }
    }

    public void Save(GM8Stream s)
    {
        s.WriteString(Name);
        s.WriteInt32(430);
        s.WriteInt32(SpriteIndex);
        s.WriteBoolean(Solid);
        s.WriteBoolean(Visible);
        s.WriteInt32(Depth);
        s.WriteBoolean(Persistent);
        s.WriteInt32(ParentIndex);
        s.WriteInt32(MaskIndex);
        s.WriteInt32(Events.Count - 1);
        for (int i = 0; i < Events.Count; i++)
        {
            for (int j = 0; j < Events[i].Count; j++)
            {
                Events[i][j].Save(s);
            }
            s.WriteInt32(-1);
        }
    }

    public GM8ObjectEvent? GetEvent(GM8ObjectEventType eventType, int subtype = 0)
    {
        List<GM8ObjectEvent> eventList = Events[(int)eventType];
        for (int i = 0; i < eventList.Count; i++)
        {
            if (eventList[i].Subtype == subtype)
            {
                return eventList[i];
            }
        }
        return null;
    }

    public GM8ObjectEvent GetEventOrAdd(GM8ObjectEventType eventType, int subtype = 0)
    {
        if (GetEvent(eventType) is { } ev)
        {
            return ev;
        }

        GM8ObjectEvent newEvent = new GM8ObjectEvent(subtype);
        Events[(int)eventType].Add(newEvent);
        return newEvent;
    }

    public bool RemoveEvent(GM8ObjectEventType eventType, int subtype = 0)
    {
        List<GM8ObjectEvent> eventList = Events[(int)eventType];
        for (int i = 0; i < eventList.Count; i++)
        {
            if (eventList[i].Subtype == subtype)
            {
                eventList.RemoveAt(i);
                return true;
            }
        }
        return false;
    }
}


public class GM8ObjectEvent
{
    public int Subtype;
    public List<GM8Action> Actions;

    public GM8ObjectEvent(int subtype)
    {
        Subtype = subtype;
        Actions = new List<GM8Action>();
    }

    public GM8ObjectEvent(int subtype, GM8Stream s)
    {
        Subtype = subtype;
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
        s.WriteInt32(Subtype);
        s.WriteInt32(400);
        s.WriteInt32(Actions.Count);
        for (int i = 0; i < Actions.Count; i++)
        {
            Actions[i].Save(s);
        }
    }

    public void AddCode(string code)
    {
        Actions.Add(GM8Action.CreateCode(code));
    }
}

public enum GM8ObjectEventType
{
    Create,
    Destroy,
    Alarm,
    Step,
    Collision,
    Keyboard,
    Mouse,
    Other,
    Draw,
    KeyPress,
    KeyRelease,
    Trigger,
}


public enum GM8ObjectEventSubtypeStep
{
    StepNormal = 0,
    StepBegin = 1,
    StepEnd = 2
}

public enum GM8ObjectEventSubtypeOther
{
    OutsideBoundary = 0,
    BoundaryCollision = 1,
    GameStart = 2,
    GameEnd = 3,
    RoomStart = 4,
    RoomEnd = 5,
    NoMoreLives = 6,
    EndOfAnimation = 7,
    EndOfPath = 8,
    NoMoreHealth = 9,
    User0 = 10,
    User1 = 11,
    User2 = 12,
    User3 = 13,
    User4 = 14,
    User5 = 15,
    User6 = 16,
    User7 = 17,
    User8 = 18,
    User9 = 19,
    User10 = 20,
    User11 = 21,
    User12 = 22,
    User13 = 23,
    User14 = 24,
    User15 = 25
}

public enum GM8ObjectEventSubtypeMouse
{
    LeftButton = 0,
    RightButton = 1,
    MiddleButton = 2,
    NoButton = 3,
    LeftPress = 4,
    RightPress = 5,
    MiddlePress = 6,
    LeftRelease = 7,
    RightRelease = 8,
    MiddleRelease = 9,
    MouseEnter = 10,
    MouseLeave = 11,
    Joystick1Left = 16,
    Joystick1Right = 17,
    Joystick1Up = 18,
    Joystick1Down = 19,
    Joystick1Button1 = 21,
    Joystick1Button2 = 22,
    Joystick1Button3 = 23,
    Joystick1Button4 = 24,
    Joystick1Button5 = 25,
    Joystick1Button6 = 26,
    Joystick1Button7 = 27,
    Joystick1Button8 = 28,
    Joystick2Left = 31,
    Joystick2Right = 32,
    Joystick2Up = 33,
    Joystick2Down = 34,
    Joystick2Button1 = 36,
    Joystick2Button2 = 37,
    Joystick2Button3 = 38,
    Joystick2Button4 = 39,
    Joystick2Button5 = 40,
    Joystick2Button6 = 41,
    Joystick2Button7 = 42,
    Joystick2Button8 = 43,
    GlobalLeftButton = 50,
    GlobalRightButton = 51,
    GlobalMiddleButton = 52,
    GlobalLeftPress = 53,
    GlobalRightPress = 54,
    GlobalMiddlePress = 55,
    GlobalLeftRelease = 56,
    GlobalRightRelease = 57,
    GlobalMiddleRelease = 58,
    MouseWheelUp = 60,
    MouseWheelDown = 61,
}

public enum GM8ObjectEventSubtypeKeyboard
{
    KeyBackspace = 8,
    KeyEnter = 13,
    KeyShift = 16,
    KeyControl = 17,
    KeyAlt = 18,
    KeyEscape = 27,
    KeySpace = 32,
    KeyPageUp = 33,
    KeyPageDown = 34,
    KeyEnd = 35,
    KeyHome = 36,
    KeyLeft = 37,
    KeyUp = 38,
    KeyRight = 39,
    KeyDown = 40,
    KeyInsert = 45,
    KeyDelete = 46,
    Key0 = 48,
    Key1 = 49,
    Key2 = 50,
    Key3 = 51,
    Key4 = 52,
    Key5 = 53,
    Key6 = 54,
    Key7 = 55,
    Key8 = 56,
    Key9 = 57,
    KeyA = 65,
    KeyB = 66,
    KeyC = 67,
    KeyD = 68,
    KeyE = 69,
    KeyF = 70,
    KeyG = 71,
    KeyH = 72,
    KeyI = 73,
    KeyJ = 74,
    KeyK = 75,
    KeyL = 76,
    KeyM = 77,
    KeyN = 78,
    KeyO = 79,
    KeyP = 80,
    KeyQ = 81,
    KeyR = 82,
    KeyS = 83,
    KeyT = 84,
    KeyU = 85,
    KeyV = 86,
    KeyW = 87,
    KeyX = 88,
    KeyY = 89,
    KeyZ = 90,
    KeyNumpad0 = 96,
    KeyNumpad1 = 97,
    KeyNumpad2 = 98,
    KeyNumpad3 = 99,
    KeyNumpad4 = 100,
    KeyNumpad5 = 101,
    KeyNumpad6 = 102,
    KeyNumpad7 = 103,
    KeyNumpad8 = 104,
    KeyNumpad9 = 105,
    KeyMultiply = 106,
    KeyAdd = 107,
    KeySubtract = 109,
    KeyDecimal = 110,
    KeyDivide = 111,
    KeyF1 = 112,
    KeyF2 = 113,
    KeyF3 = 114,
    KeyF4 = 115,
    KeyF5 = 116,
    KeyF6 = 117,
    KeyF7 = 118,
    KeyF8 = 119,
    KeyF9 = 120,
    KeyF10 = 121,
    KeyF11 = 122,
    KeyF12 = 123
}