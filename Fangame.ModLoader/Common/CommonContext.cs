using AtlasAllocator;
using Fangame.ModLoader.GM8;
using ImageMagick;
using Underanalyzer.Decompiler;
using UndertaleModLib;
using UndertaleModLib.Compiler;
using UndertaleModLib.Decompiler;
using UndertaleModLib.Models;
using UndertaleModLib.Util;

namespace Fangame.ModLoader.Common;

public class CommonContext
{
    public UndertaleData? UndertaleData;
    public GlobalDecompileContext? GlobalDecompileContext;
    public UndertaleEmbeddedTexture? TextureAtlas;
    public GuillotineAtlasAllocator? AtlasAllocator;
    public Dictionary<UndertaleEmbeddedTexture, List<TextureReplaceItem>>? TextureReplaceQueue;
    public CompileGroup? CompileGroup;
    public bool HasCodeReplace;

    public struct TextureReplaceItem
    {
        public UndertaleTexturePageItem Item;
        public MagickImage Image;
        public TextureReplaceItem(UndertaleTexturePageItem item, MagickImage image)
        {
            Item = item;
            Image = image;
        }
    }

    public CommonContext(GM8Data data)
    {
    }

    public CommonContext(UndertaleData data)
    {
        UndertaleData = data;
        GlobalDecompileContext = new GlobalDecompileContext(data);
        CompileGroup = new CompileGroup(data, GlobalDecompileContext);
        TextureReplaceQueue = [];
        NewTextureAtlas();
    }

    private void NewTextureAtlas()
    {
        if (UndertaleData == null)
        {
            return;
        }

        TextureAtlas = new UndertaleEmbeddedTexture();
        TextureAtlas.TextureData = new UndertaleEmbeddedTexture.TexData
        {
            Image = GMImage.FromPng(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Empty2048x2048.png"))),
        };
        UndertaleData.EmbeddedTextures.Add(TextureAtlas);
        AtlasAllocator = new GuillotineAtlasAllocator(new Size(2048, 2048));
    }

    public string Decompile(UndertaleCode code)
    {
        if (GlobalDecompileContext == null)
        {
            throw new NotSupportedException();
        }

        DecompileContext decompileContext = new DecompileContext(GlobalDecompileContext, code);
        return decompileContext.DecompileToString();
    }

    public void QueueCodeReplace(UndertaleCode code, string newCode)
    {
        if (CompileGroup == null)
        {
            throw new NotSupportedException();
        }

        CompileGroup.QueueCodeReplace(code, newCode);
        HasCodeReplace = true;
    }

    public UndertaleTexturePageItem AllocTexturePageItem(int width, int height)
    {
        if (AtlasAllocator == null || UndertaleData == null)
        {
            throw new NotSupportedException();
        }

        Allocation? alloc = AtlasAllocator.Allocate(new Size(width, height));
        if (!alloc.HasValue)
        {
            NewTextureAtlas();
            alloc = AtlasAllocator.Allocate(new Size(width, height));
            if (!alloc.HasValue)
            {
                throw new InvalidOperationException("Alloc texture failed.");
            }
        }
        Rectangle rect = alloc.Value.Rectangle;
        var item = new UndertaleTexturePageItem
        {
            Name = MakeString(""),
            TexturePage = TextureAtlas,
            SourceX = (ushort)rect.X,
            SourceY = (ushort)rect.Y,
            SourceWidth = (ushort)rect.Width,
            SourceHeight = (ushort)rect.Height,
            TargetX = 0,
            TargetY = 0,
            TargetWidth = (ushort)rect.Width,
            TargetHeight = (ushort)rect.Height,
            BoundingWidth = (ushort)rect.Width,
            BoundingHeight = (ushort)rect.Height,
        };
        UndertaleData.TexturePageItems.Add(item);
        return item;
    }

    public UndertaleString MakeString(string str)
    {
        if (UndertaleData == null)
        {
            throw new NotSupportedException();
        }

        return UndertaleData.Strings.MakeString(str);
    }

    public void QueueTextureReplace(UndertaleTexturePageItem item, MagickImage replaceImage)
    {
        if (TextureReplaceQueue == null)
        {
            throw new NotSupportedException();
        }

        if (!TextureReplaceQueue.TryGetValue(item.TexturePage, out var queue))
        {
            queue = [];
            TextureReplaceQueue[item.TexturePage] = queue;
        }

        queue.Add(new TextureReplaceItem(item, replaceImage));
    }

    public void FlushReplaceQueue()
    {
        if (TextureReplaceQueue != null)
        {
            foreach (var (embTex, replaces) in TextureReplaceQueue)
            {
                MagickImage embImage = embTex.TextureData.Image.GetMagickImage();
                foreach (var replace in replaces)
                {
                    embImage.Composite(replace.Image, replace.Item.SourceX, replace.Item.SourceY, CompositeOperator.Copy);
                    replace.Image.Dispose();
                }
                embTex.TextureData.Image = GMImage.FromMagickImage(embImage).ConvertToFormat(embTex.TextureData.Image.Format);
            }

            TextureReplaceQueue.Clear();
        }

        if (CompileGroup != null && HasCodeReplace)
        {
            CompileResult result = CompileGroup.Compile();
            Console.WriteLine(result.PrintAllErrors(true));
            HasCodeReplace = false;
        }
    }

    public byte[] ReadMaskData(MagickImage image)
    {
        byte[] bytes;

        // Get image pixels, and allocate enough capacity for mask
        using IPixelCollection<byte> pixels = image.GetPixels();
        bytes = new byte[(int)((image.Width + 7) / 8 * image.Height)];

        // Get white color, used to represent bits that are set
        IMagickColor<byte> white = MagickColors.White;

        // Read all pixels of image, and set a bit on the mask if a given pixel matches the white color
        int i = 0;
        for (int y = 0; y < image.Height; y++)
        {
            for (int xByte = 0; xByte < (image.Width + 7) / 8; xByte++)
            {
                byte fullByte = 0x00;
                int pxStart = xByte * 8;
                int pxEnd = Math.Min(pxStart + 8, (int)image.Width);

                for (int x = pxStart; x < pxEnd; x++)
                {
                    if (pixels.GetPixel(x, y).ToColor()!.Equals(white))
                    {
                        fullByte |= (byte)(0b1 << (7 - (x - pxStart)));
                    }
                }

                bytes[i++] = fullByte;
            }
        }
        return bytes;
    }

}
