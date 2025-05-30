using System.Buffers;
using System.Buffers.Binary;
using System.IO.Compression;
using System.Text;

namespace Fangame.ModLoader.GM8;

public unsafe class GM8Stream : Stream, IDisposable
{
    public const int DefaultCapacity = 1024 * 1024;

    public byte[] Buffer;
    public int Pos;
    public int Len;

    public override long Position
    {
        get => Pos;
        set => Pos = (int)value;
    }

    public override long Length => Len;
    public override bool CanWrite => true;
    public override bool CanRead => true;
    public override bool CanSeek => true;

    public GM8Stream()
    {
        Buffer = [];
        Len = 0;
    }

    protected override void Dispose(bool disposing)
    {
        if (Buffer.Length != 0)
        {
            ArrayPool<byte>.Shared.Return(Buffer);
            Buffer = null!;
        }
    }

    public static GM8Stream FromFile(string path)
    {
        using FileStream fs = File.OpenRead(path);
        GM8Stream s = new GM8Stream();
        s.EnsureCapacity((int)fs.Length);
        fs.CopyTo(s);
        s.Pos = 0;
        return s;
    }

    public void Save(string path)
    {
        File.WriteAllBytes(path, Buffer.AsSpan(0, Len));
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        if (origin == SeekOrigin.Begin)
        {
            Pos = (int)offset;
        }
        else if (origin == SeekOrigin.Current)
        {
            Pos += (int)offset;
        }
        else
        {
            Pos = Len + (int)offset;
        }
        return Pos;
    }

    public override void SetLength(long value)
    {
        Len = (int)value;
    }

    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        Array.Copy(Buffer, Pos, buffer, offset, count);
        Pos += count;
        return count;
    }

    public override int Read(Span<byte> data)
    {
        Buffer.AsSpan(Pos, data.Length).CopyTo(data);
        Pos += data.Length;
        return data.Length;
    }

    public override int ReadByte()
    {
        int result = Buffer[Pos];
        Pos += 1;
        return result;
    }

    public int ReadInt32()
    {
        int result = BinaryPrimitives.ReadInt32LittleEndian(Buffer.AsSpan(Pos));
        Pos += 4;
        return result;
    }

    public bool ReadBoolean()
    {
        return ReadInt32() != 0;
    }

    public long ReadInt64()
    {
        long result = BinaryPrimitives.ReadInt64LittleEndian(Buffer.AsSpan(Pos));
        Pos += 8;
        return result;
    }

    public double ReadDouble()
    {
        double result = BinaryPrimitives.ReadDoubleLittleEndian(Buffer.AsSpan(Pos));
        Pos += 8;
        return result;
    }

    public string ReadString()
    {
        int len = ReadInt32();
        string str = Encoding.UTF8.GetString(Buffer.AsSpan(Pos, len));
        Pos += len;
        return str;
    }

    public byte[] ReadData()
    {
        int len = ReadInt32();
        byte[] data = Buffer.AsSpan(Pos, len).ToArray();
        Pos += len;
        return data;
    }

    public void ReadCompressedData(GM8Stream s)
    {
        int len = ReadInt32();
        using MemoryStream inStream = new MemoryStream(Buffer, Pos, len);
        using ZLibStream zlibStream = new ZLibStream(inStream, CompressionMode.Decompress);
        zlibStream.CopyTo(s);
        s.Pos = 0;
        Pos += len;
    }

    private void EnsureCapacity(int capacity)
    {
        if (capacity > Buffer.Length)
        {
            int newCapacity = int.Max(capacity, DefaultCapacity);

            if (newCapacity < Buffer.Length * 2)
            {
                newCapacity = Buffer.Length * 2;
            }

            byte[] newBuffer = ArrayPool<byte>.Shared.Rent(newCapacity);
            if (Buffer.Length != 0)
            {
                Array.Copy(Buffer, 0, newBuffer, 0, Len);
                ArrayPool<byte>.Shared.Return(Buffer);
            }

            Buffer = newBuffer;
        }
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        EnsureCapacity(Pos + count);
        Array.Copy(buffer, offset, Buffer, Pos, count);
        Pos += count;
        Len = int.Max(Len, Pos);
    }

    public override void Write(ReadOnlySpan<byte> data)
    {
        EnsureCapacity(Pos + data.Length);
        data.CopyTo(Buffer.AsSpan(Pos));
        Pos += data.Length;
        Len = int.Max(Len, Pos);
    }

    public override void WriteByte(byte value)
    {
        EnsureCapacity(Pos + 1);
        Buffer[Pos] = value;
        Pos += 1;
        Len = int.Max(Len, Pos);
    }

    public void WriteInt32(int value)
    {
        EnsureCapacity(Pos + 4);
        BinaryPrimitives.WriteInt32LittleEndian(Buffer.AsSpan(Pos), value);
        Pos += 4;
        Len = int.Max(Len, Pos);
    }

    public void WriteBoolean(bool value)
    {
        WriteInt32(value ? 1 : 0);
    }

    public void WriteDouble(double value)
    {
        EnsureCapacity(Pos + 8);
        BinaryPrimitives.WriteDoubleLittleEndian(Buffer.AsSpan(Pos), value);
        Pos += 8;
        Len = int.Max(Len, Pos);
    }

    public void WriteString(string value)
    {
        int len = Encoding.UTF8.GetByteCount(value);
        EnsureCapacity(Pos + len + 4);
        WriteInt32(len);
        Encoding.UTF8.GetBytes(value, Buffer.AsSpan(Pos, len));
        Pos += len;
        Len = int.Max(Len, Pos);
    }

    public void WriteData(Span<byte> data)
    {
        EnsureCapacity(Pos + data.Length + 4);
        WriteInt32(data.Length);
        data.CopyTo(Buffer.AsSpan(Pos));
        Pos += data.Length;
        Len = int.Max(Len, Pos);
    }

    public void WriteData(GM8Stream s)
    {
        EnsureCapacity(Pos + s.Len + 4);
        WriteInt32(s.Len);
        s.Buffer.AsSpan(0, s.Len).CopyTo(Buffer.AsSpan(Pos));
        Pos += s.Len;
        Len = int.Max(Len, Pos);
    }

    public void WriteCompressedData(GM8Stream s)
    {
        int lenPos = Pos;
        WriteInt32(0);
        using MemoryStream inStream = new MemoryStream(s.Buffer, 0, s.Len);
        using (ZLibStream zlibStream = new ZLibStream(this, CompressionLevel.Fastest, true))
        {
            inStream.CopyTo(zlibStream);
        }
        int len = Pos - lenPos - 4;
        BinaryPrimitives.WriteInt32LittleEndian(Buffer.AsSpan(lenPos), len);
        Len = int.Max(Len, Pos);
    }

}
