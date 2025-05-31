using Fangame.ModLoader;
using Fangame.ModLoader.GM8;
using Underanalyzer.Decompiler;
using UndertaleModLib;
using UndertaleModLib.Compiler;
using UndertaleModLib.Decompiler;

namespace Godmode;

public class GodmodeMod : Mod
{
    public override void ModGM8(GM8Data data)
    {
        foreach (var script in data.Scripts)
        {
            if (script?.Name is "scrKillPlayer" or "killPlayer" or "player_kill")
            {
                script.Source = "exit;\n" + script.Source;
            }
        }
    }

    public override void ModGMS(UndertaleData data)
    {
        GlobalDecompileContext globalDecompilerContext = new GlobalDecompileContext(data);
        foreach (var script in data.Scripts)
        {
            if (script.Name.Content is "scrKillPlayer" or "killPlayer" or "player_kill")
            {
                DecompileContext decompilerContext = new DecompileContext(globalDecompilerContext, script.Code);
                string source = decompilerContext.DecompileToString();
                source = "exit;\n" + source;
                CompileGroup group = new CompileGroup(data, globalDecompilerContext);
                group.QueueCodeReplace(script.Code, source);
                group.Compile();
            }
        }
    }
}
