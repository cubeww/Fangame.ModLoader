using Fangame.ModLoader.GM8;
using ImageMagick;
using UndertaleModLib.Models;

namespace Fangame.ModLoader.Common;

public class CommonSprite
{
    private object Sprite;
    private CommonContext Context;

    public CommonSprite(object sprite, CommonContext context)
    {
        Sprite = sprite;
        Context = context;
    }

    public string Name
    {
        get
        {
            switch (Sprite)
            {
                case GM8Sprite sprite:
                    return sprite.Name;
                case UndertaleSprite sprite:
                    return sprite.Name.Content;
                default:
                    throw new NotSupportedException();
            }
        }
        set
        {
            switch (Sprite)
            {
                case GM8Sprite sprite:
                    sprite.Name = value;
                    break;
                case UndertaleSprite sprite:
                    sprite.Name = Context.MakeString(value);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }

    public int OriginX
    {
        get
        {
            switch (Sprite)
            {
                case GM8Sprite sprite:
                    return sprite.OriginX;
                case UndertaleSprite sprite:
                    return sprite.OriginX;
                default:
                    throw new NotSupportedException();
            }
        }
        set
        {
            switch (Sprite)
            {
                case GM8Sprite sprite:
                    sprite.OriginX = value;
                    break;
                case UndertaleSprite sprite:
                    sprite.OriginX = value;
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }

    public int OriginY
    {
        get
        {
            switch (Sprite)
            {
                case GM8Sprite sprite:
                    return sprite.OriginY;
                case UndertaleSprite sprite:
                    return sprite.OriginY;
                default:
                    throw new NotSupportedException();
            }
        }
        set
        {
            switch (Sprite)
            {
                case GM8Sprite sprite:
                    sprite.OriginY = value;
                    break;
                case UndertaleSprite sprite:
                    sprite.OriginY = value;
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }

    public bool SepMasks
    {
        get
        {
            switch (Sprite)
            {
                case GM8Sprite sprite:
                    return sprite.SepMasks;
                case UndertaleSprite sprite:
                    return (int)sprite.SepMasks == -1;
                default:
                    throw new NotSupportedException();
            }
        }
        set
        {
            switch (Sprite)
            {
                case GM8Sprite sprite:
                    sprite.SepMasks = value;
                    break;
                case UndertaleSprite sprite:
                    sprite.SepMasks = value ? UndertaleSprite.SepMaskType.Precise : ((UndertaleSprite.SepMaskType)unchecked((uint)-1));
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }

    public void ReplaceImages(string path, int imageNumber)
    {
        using MagickImage image = new MagickImage(path);
        image.ColorType = ColorType.TrueColorAlpha;
        image.Alpha(AlphaOption.Activate);
        int width = (int)(image.Width / imageNumber);
        int height = (int)image.Height;
        switch (Sprite)
        {
            case GM8Sprite sprite:
                sprite.Images.Clear();
                sprite.CollisionMasks.Clear();
                for (int i = 0; i < imageNumber; i++)
                {
                    using var subImage = image.CloneArea(new MagickGeometry(width * i, 0, (uint)width, (uint)height));
                    using var pixels = subImage.GetPixels();
                    GM8SpriteImage im = new GM8SpriteImage(width, height, pixels.ToByteArray(PixelMapping.BGRA)!);
                    sprite.Images.Add(im);
                    sprite.CollisionMasks.Add(new GM8CollisionMask(im));
                }
                break;
            case UndertaleSprite sprite:
                sprite.Textures.Clear();
                sprite.CollisionMasks.Clear();
                for (int i = 0; i < imageNumber; i++)
                {
                    var subImage = (MagickImage)image.CloneArea(new MagickGeometry(width * i, 0, (uint)width, (uint)height));
                    UndertaleTexturePageItem item = Context.AllocTexturePageItem(width, height);
                    Context.QueueTextureReplace(item, subImage);
                    sprite.Textures.Add(new UndertaleSprite.TextureEntry { Texture = item });
                    var entry = sprite.NewMaskEntry(Context.UndertaleData);
                    entry.Data = Context.ReadMaskData(subImage);
                }
                break;
            default:
                throw new NotSupportedException();
        }
    }
}
