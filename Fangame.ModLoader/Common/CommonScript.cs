using Fangame.ModLoader.GM8;
using UndertaleModLib.Models;

namespace Fangame.ModLoader.Common;

public class CommonScript
{
    private object _script;
    private CommonContext _context;

    public CommonScript(object script, CommonContext context)
    {
        _script = script;
        _context = context;
    }

    public string Name
    {
        get
        {
            switch (_script)
            {
                case GM8Script script:
                    return script.Name;
                case UndertaleScript script:
                    return script.Name.Content;
                default:
                    throw new NotSupportedException();
            }
        }
        set
        {
            switch (_script)
            {
                case GM8Script script:
                    script.Name = value;
                    break;
                case UndertaleScript script:
                    script.Name = _context.MakeString(value);
                    script.Code.Name = _context.MakeString($"gml_Script_{value}");
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }

    public string Source
    {
        get
        {
            switch (_script)
            {
                case GM8Script script:
                    return script.Source;
                case UndertaleScript script:
                    return _context.Decompile(script.Code);
                default:
                    throw new NotSupportedException();
            }
        }
        set
        {
            switch (_script)
            {
                case GM8Script script:
                    script.Source = value;
                    break;
                case UndertaleScript script:
                    _context.QueueCodeReplace(script.Code, value);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
