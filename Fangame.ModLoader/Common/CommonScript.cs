using Fangame.ModLoader.GM8;
using UndertaleModLib.Models;

namespace Fangame.ModLoader.Common;

public class CommonScript
{
    private object Script;
    private CommonContext Context;

    public CommonScript(object script, CommonContext context)
    {
        Script = script;
        Context = context;
    }

    public string Name
    {
        get
        {
            switch (Script)
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
            switch (Script)
            {
                case GM8Script script:
                    script.Name = value;
                    break;
                case UndertaleScript script:
                    script.Name = Context.MakeString(value);
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
            switch (Script)
            {
                case GM8Script script:
                    return script.Source;
                case UndertaleScript script:
                    return Context.Decompile(script.Code);
                default:
                    throw new NotSupportedException();
            }
        }
        set
        {
            switch (Script)
            {
                case GM8Script script:
                    script.Source = value;
                    break;
                case UndertaleScript script:
                    Context.QueueCodeReplace(script.Code, value);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
