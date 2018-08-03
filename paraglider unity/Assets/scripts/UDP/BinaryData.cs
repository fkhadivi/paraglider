using System;
using System.Text;

public class BinaryData
{
    public byte[] data = null;

    public BinaryData(int size = 0)
    {
        if (size != 0) alloc(size);
    }

    public int size
    {
        get { return data.Length; }
    }

    public override string ToString()
    {
        return Encoding.UTF8.GetString(data, 0, size).TrimEnd('\0');
    }

    public int ToInt()
    {
        return BitConverter.ToInt32(data, 0);
    }

    public object this[int i]
    {
        get { return data[i]; }
        set { data[i] = Convert.ToByte(value); }  // TODO: Not sure if that works right!
    }

    public void alloc(int size)
    {
        data = new byte[size];
        Array.Clear(data, 0, data.Length);
    }
}