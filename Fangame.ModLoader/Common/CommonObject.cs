using Fangame.ModLoader.GM8;
using UndertaleModLib.Models;

namespace Fangame.ModLoader.Common;

public class CommonObject
{
    private object _object;
    private CommonContext _context;

    public CommonObject(object obj, CommonContext context)
    {
        _object = obj;
        _context = context;
    }

    public string Name
    {
        get
        {
            switch (_object)
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
            switch (_object)
            {
                case GM8Object obj:
                    obj.Name = value;
                    break;
                case UndertaleGameObject obj:
                    obj.Name = _context.MakeString(value);
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
            switch (_object)
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
            switch (_object)
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
            switch (_object)
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
            switch (_object)
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
            switch (_object)
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
            switch (_object)
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
        switch (_object)
        {
            case GM8Object obj:
                obj.GetEventOrAdd((GM8ObjectEventType)ev, subtype).AddCode(code);
                break;
            case UndertaleGameObject obj:
                UndertaleCode undertaleCode = obj.EventHandlerFor(ev, (uint)subtype, _context.UndertaleData);
                string originalCode = _context.Decompile(undertaleCode);
                string newCode = originalCode + code;
                _context.QueueCodeReplace(undertaleCode, newCode);
                break;
            default:
                throw new NotSupportedException();
        }
    }
}
