using Fangame.ModLoader.GM8;
using UndertaleModLib;

namespace Fangame.ModLoader.Common;

public class CommonData
{
    public CommonContext Context { get; }
    public CommonCollection<CommonSprite> Sprites { get; }
    public CommonCollection<CommonScript> Scripts { get; }
    public CommonCollection<CommonObject> Objects { get; }

    public CommonData(GM8Data data)
    {
        Context = new CommonContext(data);
        Sprites = new CommonCollection<CommonSprite>(data.Sprites, Context);
        Scripts = new CommonCollection<CommonScript>(data.Scripts, Context);
        Objects = new CommonCollection<CommonObject>(data.Objects, Context);
    }

    public CommonData(UndertaleData data)
    {
        Context = new CommonContext(data);
        Sprites = new CommonCollection<CommonSprite>(data.Sprites, Context);
        Scripts = new CommonCollection<CommonScript>(data.Scripts, Context);
        Objects = new CommonCollection<CommonObject>(data.GameObjects, Context);
    }

    public void FlushReplaceQueue()
    {
        Context.FlushReplaceQueue();
    }
}
