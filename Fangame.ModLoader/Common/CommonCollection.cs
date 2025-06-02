using Fangame.ModLoader.GM8;
using UndertaleModLib.Models;

namespace Fangame.ModLoader.Common;

public class CommonCollection<T> where T : class
{
    private object List;
    private CommonContext Context;

    public CommonCollection(object list, CommonContext context)
    {
        List = list;
        Context = context;
    }

    public IEnumerator<T> GetEnumerator()
    {
        switch (List)
        {
            // GM8
            case List<GM8Sprite?> list:
                foreach (var asset in list)
                {
                    if (asset != null)
                    {
                        yield return (T)(object)new CommonSprite(asset, Context);
                    }
                }
                break;
            case List<GM8Script?> list:
                foreach (var asset in list)
                {
                    if (asset != null)
                    {
                        yield return (T)(object)new CommonScript(asset, Context);
                    }
                }
                break;
            case List<GM8Object?> list:
                foreach (var asset in list)
                {
                    if (asset != null)
                    {
                        yield return (T)(object)new CommonObject(asset, Context);
                    }
                }
                break;

            // GMS
            case IList<UndertaleSprite> list:
                foreach (var asset in list)
                {
                    yield return (T)(object)new CommonSprite(asset, Context);
                }
                break;
            case IList<UndertaleScript> list:
                foreach (var asset in list)
                {
                    yield return (T)(object)new CommonScript(asset, Context);
                }
                break;
            case IList<UndertaleGameObject> list:
                foreach (var asset in list)
                {
                    yield return (T)(object)new CommonObject(asset, Context);
                }
                break;
            default:
                throw new NotSupportedException();
        }
    }

    public T CreateNew()
    {
        switch (List)
        {
            // GM8
            case List<GM8Sprite?> list:
                {
                    var x = new GM8Sprite();
                    list.Add(x);
                    return (T)(object)new CommonSprite(x, Context);
                }
            case List<GM8Script?> list:
                {
                    var x = new GM8Script();
                    list.Add(x);
                    return (T)(object)new CommonScript(x, Context);
                }
            case List<GM8Object?> list:
                {
                    var x = new GM8Object();
                    list.Add(x);
                    return (T)(object)new CommonObject(x, Context);
                }

            // GMS
            case IList<UndertaleSprite?> list:
                {
                    var x = new UndertaleSprite();
                    list.Add(x);
                    return (T)(object)new CommonSprite(x, Context);
                }
            case IList<UndertaleScript?> list:
                {
                    var x = new UndertaleScript();
                    list.Add(x);
                    return (T)(object)new CommonScript(x, Context);
                }
            case IList<UndertaleGameObject?> list:
                {
                    var x = new UndertaleGameObject();
                    list.Add(x);
                    return (T)(object)new CommonObject(x, Context);
                }

            default:
                throw new NotSupportedException();
        }
    }
}
