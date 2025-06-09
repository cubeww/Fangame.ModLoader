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
    private readonly GlobalDecompileContext? _globalDecompileContext;
    private readonly CompileGroup? _compileGroup;
    private bool _hasCodeReplace;
    private readonly Dictionary<UndertaleEmbeddedTexture, List<TextureReplaceItem>>? _textureReplaceQueue;
    private UndertaleEmbeddedTexture? _textureAtlas;
    private GuillotineAtlasAllocator? _atlasAllocator;
    public UndertaleData? UndertaleData { get; }

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
        _globalDecompileContext = new GlobalDecompileContext(data);
        _compileGroup = new CompileGroup(data, _globalDecompileContext);
        _textureReplaceQueue = [];
        NewTextureAtlas();
    }

    private void NewTextureAtlas()
    {
        if (UndertaleData == null)
        {
            return;
        }

        _textureAtlas = new UndertaleEmbeddedTexture();
        _textureAtlas.TextureData = new UndertaleEmbeddedTexture.TexData
        {
            Image = GMImage.FromPng(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Empty2048x2048.png"))),
        };
        UndertaleData.EmbeddedTextures.Add(_textureAtlas);
        _atlasAllocator = new GuillotineAtlasAllocator(new Size(2048, 2048));
    }

    public string Decompile(UndertaleCode code)
    {
        if (_globalDecompileContext == null)
        {
            throw new NotSupportedException();
        }

        DecompileContext decompileContext = new DecompileContext(_globalDecompileContext, code);
        return decompileContext.DecompileToString();
    }

    public void QueueCodeReplace(UndertaleCode code, string newCode)
    {
        if (_compileGroup == null)
        {
            throw new NotSupportedException();
        }

        _compileGroup.QueueCodeReplace(code, newCode);
        _hasCodeReplace = true;
    }

    public UndertaleTexturePageItem AllocTexturePageItem(int width, int height)
    {
        if (_atlasAllocator == null || UndertaleData == null)
        {
            throw new NotSupportedException();
        }

        Allocation? alloc = _atlasAllocator.Allocate(new Size(width, height));
        if (!alloc.HasValue)
        {
            NewTextureAtlas();
            alloc = _atlasAllocator.Allocate(new Size(width, height));
            if (!alloc.HasValue)
            {
                throw new InvalidOperationException("Alloc texture failed.");
            }
        }
        Rectangle rect = alloc.Value.Rectangle;
        var item = new UndertaleTexturePageItem
        {
            Name = MakeString(""),
            TexturePage = _textureAtlas,
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
        if (_textureReplaceQueue == null)
        {
            throw new NotSupportedException();
        }

        if (!_textureReplaceQueue.TryGetValue(item.TexturePage, out var queue))
        {
            queue = [];
            _textureReplaceQueue[item.TexturePage] = queue;
        }

        queue.Add(new TextureReplaceItem(item, replaceImage));
    }

    public void FlushReplaceQueue()
    {
        if (_textureReplaceQueue != null)
        {
            foreach (var (embTex, replaces) in _textureReplaceQueue)
            {
                MagickImage embImage = embTex.TextureData.Image.GetMagickImage();
                foreach (var replace in replaces)
                {
                    embImage.Composite(replace.Image, replace.Item.SourceX, replace.Item.SourceY, CompositeOperator.Copy);
                    replace.Image.Dispose();
                }
                embTex.TextureData.Image = GMImage.FromMagickImage(embImage).ConvertToFormat(embTex.TextureData.Image.Format);
            }

            _textureReplaceQueue.Clear();
        }

        if (_compileGroup != null && _hasCodeReplace)
        {
            CompileResult result = _compileGroup.Compile();
            Console.WriteLine(result.PrintAllErrors(true));
            _hasCodeReplace = false;
        }
    }

    public byte[] ReadMaskData(MagickImage image)
    {
        byte[] bytes;

        // Get image pixels, and allocate enough capacity for mask
        using IPixelCollection<byte> pixels = image.GetPixels();
        bytes = new byte[(int)((image.Width + 7) / 8 * image.Height)];

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
                    bool hasPixel = pixels.GetPixel(x, y).ToColor()!.A != 0;
                    if (hasPixel)
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
