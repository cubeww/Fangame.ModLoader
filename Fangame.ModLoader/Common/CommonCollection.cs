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
                foreach (var sprite in list)
                {
                    if (sprite != null)
                    {
                        yield return (T)(object)new CommonSprite(sprite, Context);
                    }
                }
                break;
            case List<GM8Script?> list:
                foreach (var script in list)
                {
                    if (script != null)
                    {
                        yield return (T)(object)new CommonScript(script, Context);
                    }
                }
                break;
            case List<GM8Object?> list:
                foreach (var obj in list)
                {
                    if (obj != null)
                    {
                        yield return (T)(object)new CommonObject(obj, Context);
                    }
                }
                break;

            // GMS
            case IList<UndertaleSprite> list:
                foreach (var sprite in list)
                {
                    yield return (T)(object)new CommonSprite(sprite, Context);
                }
                break;
            case IList<UndertaleScript> list:
                foreach (var script in list)
                {
                    yield return (T)(object)new CommonScript(script, Context);
                }
                break;
            case IList<UndertaleGameObject> list:
                foreach (var obj in list)
                {
                    yield return (T)(object)new CommonObject(obj, Context);
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
                    var sprite = new GM8Sprite();
                    list.Add(sprite);
                    return (T)(object)new CommonSprite(sprite, Context);
                }
            case List<GM8Script?> list:
                {
                    var script = new GM8Script();
                    list.Add(script);
                    return (T)(object)new CommonScript(script, Context);
                }
            case List<GM8Object?> list:
                {
                    var obj = new GM8Object();
                    list.Add(obj);
                    return (T)(object)new CommonObject(obj, Context);
                }

            // GMS
            case IList<UndertaleSprite?> list:
                {
                    var sprite = new UndertaleSprite();
                    list.Add(sprite);
                    return (T)(object)new CommonSprite(sprite, Context);
                }
            case IList<UndertaleScript?> list:
                {
                    var script = new UndertaleScript();
                    script.Code = UndertaleCode.CreateEmptyEntry(Context.UndertaleData, $"gml_Script_{Random.Shared.NextInt64()}");
                    list.Add(script);
                    return (T)(object)new CommonScript(script, Context);
                }
            case IList<UndertaleGameObject?> list:
                {
                    var obj = new UndertaleGameObject();
                    list.Add(obj);
                    return (T)(object)new CommonObject(obj, Context);
                }

            default:
                throw new NotSupportedException();
        }
    }
}
