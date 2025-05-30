using System.Runtime.InteropServices;

namespace Fangame.ModLoader.GM8;

public class GM8Data
{
    public GM8Stream Executable;
    public int Version;
    public int SettingsPosition;
    public GM8Settings Settings;
    public List<GM8Extension> Extensions;
    public List<GM8Trigger?> Triggers;
    public List<GM8Constant> Constants;
    public List<GM8Sound?> Sounds;
    public List<GM8Sprite?> Sprites;
    public List<GM8Background?> Backgrounds;
    public List<GM8Path?> Paths;
    public List<GM8Script?> Scripts;
    public List<GM8Font?> Fonts;
    public List<GM8Timeline?> Timelines;
    public List<GM8Object?> Objects;
    public List<GM8Room?> Rooms;
    public int LastInstanceId;
    public int LastTileId;
    public List<GM8IncludedFile?> IncludedFiles;
    public GM8HelpDialog HelpDialog;
    public List<string> LibraryInitStrings;
    public List<int> RoomOrder;
    public string DX8DllName;
    public byte[] DX8Dll;
    public int GameId;
    public (int, int, int, int) Guid;

    private enum XorMethod
    {
        Normal,
        Sudalv,
    }

    private class MaskGenerator
    {
        public uint Seed1;
        public uint Seed2;
        public uint[]? Iter;
        public int Index;

        public MaskGenerator(uint seed1, uint seed2, uint[]? iter)
        {
            Seed1 = seed1;
            Seed2 = seed2;
            Iter = iter;
        }

        public uint Next()
        {
            uint n1 = 0x9069;
            uint n2 = 0x4650;
            if (Iter != null)
            {
                n1 = Iter[Index++];
                if (Index >= Iter.Length) Index = 0;
                n2 = Iter[Index++];
                if (Index >= Iter.Length) Index = 0;
            }
            Seed1 = ((0xFFFF & Seed1) >> 0) * n1 + (Seed1 >> 16);
            Seed2 = ((0xFFFF & Seed2) >> 0) * n2 + (Seed2 >> 16);

            uint result = ((Seed1 << 16) >> 0) + ((Seed2 & 0xFFFF) >> 0);
            return result;
        }
    }

    public GM8Data(GM8Stream s)
    {
        Executable = s;
        Version = FindSettings(s);
        SettingsPosition = s.Pos;
        Settings = ReadCompressedAsset(s, 0, (s, v) => new GM8Settings(s, v));
        DX8DllName = s.ReadString();
        DX8Dll = s.ReadData();
        DecryptGameData(s);
        int garbage = s.ReadInt32();
        s.Pos += garbage * 4;
        s.ReadBoolean();
        GameId = s.ReadInt32();
        Guid = (s.ReadInt32(), s.ReadInt32(), s.ReadInt32(), s.ReadInt32());
        Extensions = ReadAssets(s, 700, (s, v) => new GM8Extension(s));
        Triggers = ReadCompressedAssets(s, 800, (s, v) => new GM8Trigger(s));
        Constants = ReadAssets(s, 800, (s, v) => new GM8Constant(s));
        Sounds = ReadCompressedAssets(s, 800, (s, v) => new GM8Sound(s));
        Sprites = ReadCompressedAssets(s, 800, (s, v) => new GM8Sprite(s));
        Backgrounds = ReadCompressedAssets(s, 800, (s, v) => new GM8Background(s));
        Paths = ReadCompressedAssets(s, 800, (s, v) => new GM8Path(s));
        Scripts = ReadCompressedAssets(s, 800, (s, v) => new GM8Script(s));
        Fonts = ReadCompressedAssets(s, 800, (s, v) => new GM8Font(s, v));
        Timelines = ReadCompressedAssets(s, 800, (s, v) => new GM8Timeline(s));
        Objects = ReadCompressedAssets(s, 800, (s, v) => new GM8Object(s));
        Rooms = ReadCompressedAssets(s, 800, (s, v) => new GM8Room(s, v));
        LastInstanceId = s.ReadInt32();
        LastTileId = s.ReadInt32();
        IncludedFiles = ReadCompressedAssets(s, 800, (s, v) => new GM8IncludedFile(s));
        HelpDialog = ReadCompressedAsset(s, 800, (s, v) => new GM8HelpDialog(s));
        LibraryInitStrings = ReadAssets(s, 500, (s, v) => s.ReadString());
        RoomOrder = ReadAssets(s, 700, (s, v) => s.ReadInt32());
    }

    private T ReadCompressedAsset<T>(GM8Stream s, int versionHeader, Func<GM8Stream, int, T> factory) where T : class
    {
        if (versionHeader != 0)
        {
            if (s.ReadInt32() != versionHeader)
            {
                throw new IOException("Incorrect version header.");
            }
        }
        using GM8Stream ss = new GM8Stream();
        s.ReadCompressedData(ss);
        return factory(ss, Version);
    }

    private List<T?> ReadCompressedAssets<T>(GM8Stream s, int versionHeader, Func<GM8Stream, int, T> factory) where T : class
    {
        if (s.ReadInt32() != versionHeader)
        {
            throw new IOException("Incorrect version header.");
        }
        int count = s.ReadInt32();
        List<T?> list = new List<T?>(count);
        for (int i = 0; i < count; i++)
        {
            using GM8Stream ss = new GM8Stream();
            s.ReadCompressedData(ss);
            if (ss.ReadInt32() != 0)
            {
                list.Add(factory(ss, Version));
            }
            else
            {
                list.Add(null);
            }
        }

        return list;
    }

    private List<T> ReadAssets<T>(GM8Stream s, int versionHeader, Func<GM8Stream, int, T> factory)
    {
        if (s.ReadInt32() != versionHeader)
        {
            throw new IOException("Incorrect version header.");
        }
        int count = s.ReadInt32();
        List<T> list = new List<T>(count);
        for (int i = 0; i < count; i++)
        {
            list.Add(factory(s, Version));
        }

        return list;
    }

    private static int FindSettings(GM8Stream s)
    {
        if (CheckGM81(s))
        {
            return 810;
        }
        else if (CheckGM80(s))
        {
            return 800;
        }
        else throw new IOException("Not supported game data.");
    }

    private static bool CheckGM81(GM8Stream s)
    {
        if (s.Len < 0x226D8A)
        {
            return false;
        }
        s.Pos = 0x00226CF3;
        Span<byte> buf = stackalloc byte[8];
        s.ReadExactly(buf);
        if (buf.SequenceEqual(new byte[] { 0xE8, 0x80, 0xF2, 0xDD, 0xFF, 0xC7, 0x45, 0xF0 }))
        {
            int headerStart = s.ReadInt32();
            s.Pos += 125;
            int? gm81Magic = null;
            buf = stackalloc byte[3];
            s.ReadExactly(buf);
            if (buf.SequenceEqual(new byte[] { 0x81, 0x7D, 0xEC }))
            {
                int magic = s.ReadInt32();
                if (s.ReadByte() == 0x74)
                {
                    gm81Magic = magic;
                }
            }
            s.Pos = 0x0010BB83;
            XorMethod xorMethod;
            buf = stackalloc byte[8];
            s.ReadExactly(buf);
            if (buf.SequenceEqual(new byte[] { 0x8B, 0x02, 0xC1, 0xE0, 0x10, 0x8B, 0x11, 0x81 }))
            {
                xorMethod = XorMethod.Sudalv;
            }
            else
            {
                xorMethod = XorMethod.Normal;
            }
            s.Pos = headerStart;
            if (gm81Magic != null)
            {
                if (SeekValue(s, (uint)gm81Magic.Value) == null)
                {
                    return false;
                }
            }
            else
            {
                s.Pos += 8;
            }
            DecryptGM81(s, xorMethod);
            s.Pos += 20;
            return true;
        }
        return false;
    }

    private static uint? SeekValue(GM8Stream s, uint value)
    {
        int pos = s.Pos;
        while (true)
        {
            s.Pos = pos;
            uint d1 = (uint)s.ReadInt32();
            uint d2 = (uint)s.ReadInt32();
            uint parsedValue = (d1 & 0xFF00FF00) | (d2 & 0x00FF00FF);
            uint parsedXor = (d1 & 0x00FF00FF) | (d2 & 0xFF00FF00);
            if (parsedValue == value)
            {
                return parsedXor;
            }
            pos += 1;
            if ((pos + 8) >= s.Len)
            {
                return null;
            }
        }
    }

    private static void DecryptGM81(GM8Stream s, XorMethod xorMethod)
    {
        static uint Crc32(ReadOnlySpan<byte> hashKey, uint[] crcTable)
        {
            uint result = 0xFFFFFFFF;
            foreach (var c in hashKey)
            {
                result = (result >> 8) ^ crcTable[(result & 0xFF) ^ c];
            }
            return result;
        }

        static uint Crc32Reflect(uint value, sbyte c)
        {
            uint rvalue = 0;
            for (sbyte i = 1; i <= c; i++)
            {
                if ((value & 1) != 0)
                {
                    rvalue |= (uint)(1 << (c - i));
                }
                value >>= 1;
            }
            return rvalue;
        }

        uint sudalvMagicPoint = (uint)(s.Pos - 12);
        string hashKey = $"_MJD{s.ReadInt32()}#RWK";
        uint[] crcTable = new uint[256];
        uint crcPolynomial = 0x04C11DB7;
        for (uint i = 0; i < crcTable.Length; i++)
        {
            crcTable[i] = Crc32Reflect(i, 8) << 24;
            for (int j = 0; j < 8; j++)
            {
                crcTable[i] = (crcTable[i] << 1) ^ (((crcTable[i] & (1 << 31)) != 0) ? crcPolynomial : 0);
            }
            crcTable[i] = Crc32Reflect(crcTable[i], 32);
        }

        uint seed1 = (uint)s.ReadInt32();
        uint seed2 = Crc32(MemoryMarshal.AsBytes(hashKey.AsSpan()), crcTable);
        int encryptionStart = s.Pos + (int)(seed2 & 0xFF) + 10;
        int offsetBackup = s.Pos;
        MaskGenerator generator;
        if (xorMethod == XorMethod.Normal)
        {
            generator = new MaskGenerator(seed1, seed2, null);
        }
        else
        {
            Span<byte> maskData = s.Buffer.AsSpan(0, (int)sudalvMagicPoint + 4);
            List<(byte, byte)> rChunksMaskData = [];
            for (int i = maskData.Length - 2; i >= 0; i -= 2)
            {
                rChunksMaskData.Add((maskData[i], maskData[i + 1]));
            }
            List<((byte, byte), (byte, byte))> maskCountArray = [];
            for (int i = 1; i < rChunksMaskData.Count; i++)
            {
                maskCountArray.Add((rChunksMaskData[i], rChunksMaskData[i - 1]));
            }
            int? maskCount = null;
            for (int i = 0; i < maskCountArray.Count; i++)
            {
                int a = maskCountArray[i].Item1.Item1;
                int b = maskCountArray[i].Item1.Item2;
                int c = maskCountArray[i].Item2.Item1;
                int d = maskCountArray[i].Item2.Item2;
                if (a == 0 && b == 0 && c == 0 && d == 0)
                {
                    maskCount = i;
                    break;
                }
            }
            if (maskCount == null)
            {
                throw new IOException("Unable to find the maskCount.");
            }

            List<uint> iter = [];
            for (int i = 1; i < maskCount + 2; i++)
            {
                (byte, byte) x = rChunksMaskData[i];
                iter.Add((uint)(x.Item1 | (x.Item2 << 8)));
            }
            generator = new MaskGenerator(seed1, seed2, [.. iter]);
        }
        for (int loopOffset = encryptionStart; loopOffset <= s.Len - 4; loopOffset += 4)
        {
            s.Pos = loopOffset;
            uint chunk = (uint)s.ReadInt32();
            chunk = chunk ^ generator.Next();
            s.Pos -= 4;
            s.WriteInt32((int)chunk);
        }
        s.Pos = offsetBackup;
    }

    private static bool CheckGM80(GM8Stream s)
    {
        s.Pos = 1980000;
        if (s.ReadInt32() == 1234321)
        {
            s.Pos = 1980000 + 16;
            return true;
        }
        else
        {
            int num = 2000000;
            s.Pos = num;
            while (num < s.Len)
            {
                if (s.ReadInt32() == 1234321)
                {
                    s.Pos = num + 16;
                    return true;
                }
                num += 10000;
                s.Pos = num;
            }
            return false;
        }
    }

    private static void DecryptGameData(GM8Stream s)
    {
        Span<byte> swapTable = stackalloc byte[256];
        Span<byte> reverseTable = stackalloc byte[256];
        int garbage1 = s.ReadInt32();
        int garbage2 = s.ReadInt32();
        s.Pos += garbage1 * 4;
        s.ReadExactly(swapTable);
        s.Pos += garbage2 * 4;
        for (int i = 0; i < 256; i++)
        {
            reverseTable[swapTable[i]] = (byte)i;
        }
        int len = s.ReadInt32();
        for (int i = s.Pos + len; i >= s.Pos + 2; i--)
        {
            s.Buffer[i - 1] = (byte)(reverseTable[s.Buffer[i - 1]] - (s.Buffer[i - 2] + (i - (s.Pos + 1))));
        }
        for (int i = s.Pos + len - 1; i >= s.Pos; i--)
        {
            int b = int.Max(i - swapTable[(i - s.Pos) & 0xFF], s.Pos);
            (s.Buffer[i], s.Buffer[b]) = (s.Buffer[b], s.Buffer[i]);
        }
    }

    public static GM8Data FromExecutable(string path)
    {
        GM8Stream exe = GM8Stream.FromFile(path);
        return new GM8Data(exe);
    }

    public void SaveExecutable(string path)
    {
        Executable.Pos = SettingsPosition;
        WriteFromSettings(Executable);
        Executable.Len = Executable.Pos;
        if (Version == 810)
        {
            CheckGM81(Executable);
            if (Executable.Buffer[0x10BD49] == 0x74)
            {
                Executable.Buffer[0x10BD49] = 0xEB;
            }
        }
        Executable.Save(path);
    }

    private void WriteFromSettings(GM8Stream s)
    {
        WriteCompressedAsset(s, Settings, 0, (a, s, v) => a.Save(s, v));
        s.WriteString(DX8DllName);
        s.WriteData(DX8Dll);
        s.WriteInt32(0);
        s.WriteInt32(0);
        for (int i = 0; i < 256; i++)
        {
            s.WriteByte((byte)i);
        }

        using GM8Stream gameData = new GM8Stream();
        WriteGameData(gameData);

        for (int i = 0; i < gameData.Len; i++)
        {
            (gameData.Buffer[i], gameData.Buffer[i & (~0xFF)]) = (gameData.Buffer[i & (~0xFF)], gameData.Buffer[i]);
        }

        for (int i = 1; i < gameData.Len; i++)
        {
            gameData.Buffer[i] = (byte)(gameData.Buffer[i] + gameData.Buffer[i - 1] + i);
        }

        s.WriteData(gameData);
    }

    private void WriteGameData(GM8Stream s)
    {
        s.WriteInt32(0);
        s.WriteBoolean(true);
        s.WriteInt32(GameId);
        s.WriteInt32(Guid.Item1);
        s.WriteInt32(Guid.Item2);
        s.WriteInt32(Guid.Item3);
        s.WriteInt32(Guid.Item4);
        WriteAssets(s, Extensions, 700, (a, s) => a.Save(s));
        WriteCompressedAssets(s, Triggers, 800, (a, s, v) => a.Save(s));
        WriteAssets(s, Constants, 800, (a, s) => a.Save(s));
        WriteCompressedAssets(s, Sounds, 800, (a, s, v) => a.Save(s));
        WriteCompressedAssets(s, Sprites, 800, (a, s, v) => a.Save(s));
        WriteCompressedAssets(s, Backgrounds, 800, (a, s, v) => a.Save(s));
        WriteCompressedAssets(s, Paths, 800, (a, s, v) => a.Save(s));
        WriteCompressedAssets(s, Scripts, 800, (a, s, v) => a.Save(s));
        WriteCompressedAssets(s, Fonts, 800, (a, s, v) => a.Save(s, v));
        WriteCompressedAssets(s, Timelines, 800, (a, s, v) => a.Save(s));
        WriteCompressedAssets(s, Objects, 800, (a, s, v) => a.Save(s));
        WriteCompressedAssets(s, Rooms, 800, (a, s, v) => a.Save(s, v));
        s.WriteInt32(LastInstanceId);
        s.WriteInt32(LastTileId);
        WriteCompressedAssets(s, IncludedFiles, 800, (a, s, v) => a.Save(s));
        WriteCompressedAsset(s, HelpDialog, 800, (a, s, v) => a.Save(s));
        WriteAssets(s, LibraryInitStrings, 500, (a, s) => s.WriteString(a));
        WriteAssets(s, RoomOrder, 700, (a, s) => s.WriteInt32(a));
    }

    public static void WriteAssets<T>(GM8Stream s, List<T> list, int versionHeader, Action<T, GM8Stream> saver)
    {
        s.WriteInt32(versionHeader);
        s.WriteInt32(list.Count);
        for (int i = 0; i < list.Count; i++)
        {
            saver(list[i], s);
        }
    }

    public void WriteCompressedAsset<T>(GM8Stream s, T asset, int versionHeader, Action<T, GM8Stream, int> saver) where T : class
    {
        if (versionHeader != 0)
        {
            s.WriteInt32(versionHeader);
        }
        using GM8Stream ss = new GM8Stream();
        saver(asset, ss, Version);
        s.WriteCompressedData(ss);
    }

    public void WriteCompressedAssets<T>(GM8Stream s, List<T?> list, int versionHeader, Action<T, GM8Stream, int> saver)
    {
        s.WriteInt32(versionHeader);
        s.WriteInt32(list.Count);
        for (int i = 0; i < list.Count; i++)
        {
            T? asset = list[i];
            if (asset == null)
            {
                s.WriteData([0x78, 0x9C, 0x63, 0x60, 0x60, 0x60, 0x00, 0x00, 0x00, 0x04, 0x00, 0x01]);
            }
            else
            {
                using GM8Stream ss = new GM8Stream();
                ss.WriteBoolean(true);
                saver(asset, ss, Version);
                s.WriteCompressedData(ss);
            }
        }
    }

}
