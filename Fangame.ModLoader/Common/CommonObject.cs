using Fangame.ModLoader.GM8;
using UndertaleModLib.Models;

namespace Fangame.ModLoader.Common;

public class CommonObject
{
    private object Obj;
    private CommonContext Context;

    public CommonObject(object obj, CommonContext context)
    {
        Obj = obj;
        Context = context;
    }

    public string Name
    {
        get
        {
            switch (Obj)
            {
                case GM8Object obj:
                    return obj.Name;
                case UndertaleGameObject obj:
                    return obj.Name.Content;
                default:
                    throw new NotSupportedException();
            }
        }
        set
        {
            switch (Obj)
            {
                case GM8Object obj:
                    obj.Name = value;
                    break;
                case UndertaleGameObject obj:
                    obj.Name = Context.MakeString(value);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }

    public bool Visible
    {
        get
        {
            switch (Obj)
            {
                case GM8Object obj:
                    return obj.Visible;
                case UndertaleGameObject obj:
                    return obj.Visible;
                default:
                    throw new NotSupportedException();
            }
        }
        set
        {
            switch (Obj)
            {
                case GM8Object obj:
                    obj.Visible = value;
                    break;
                case UndertaleGameObject obj:
                    obj.Visible = value;
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }

    public bool Persistent
    {
        get
        {
            switch (Obj)
            {
                case GM8Object obj:
                    return obj.Persistent;
                case UndertaleGameObject obj:
                    return obj.Persistent;
                default:
                    throw new NotSupportedException();
            }
        }
        set
        {
            switch (Obj)
            {
                case GM8Object obj:
                    obj.Persistent = value;
                    break;
                case UndertaleGameObject obj:
                    obj.Persistent = value;
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }

    public int Depth
    {
        get
        {
            switch (Obj)
            {
                case GM8Object obj:
                    return obj.Depth;
                case UndertaleGameObject obj:
                    return obj.Depth;
                default:
                    throw new NotSupportedException();
            }
        }
        set
        {
            switch (Obj)
            {
                case GM8Object obj:
                    obj.Depth = value;
                    break;
                case UndertaleGameObject obj:
                    obj.Depth = value;
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }

    public void EventAddCode(EventType ev, int subtype, string code)
    {
        switch (Obj)
        {
            case GM8Object obj:
                obj.GetEventOrAdd((GM8ObjectEventType)ev, subtype).AddCode(code);
                break;
            case UndertaleGameObject obj:
                UndertaleCode undertaleCode = obj.EventHandlerFor(ev, (uint)subtype, Context.UndertaleData);
                string originalCode = Context.Decompile(undertaleCode);
                string newCode = originalCode + code;
                Context.QueueCodeReplace(undertaleCode, newCode);
                break;
            default:
                throw new NotSupportedException();
        }
    }
}
