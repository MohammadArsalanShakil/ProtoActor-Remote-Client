using System;
using System.Collections.Generic;

namespace ProtoActor_Remote_Client.Utils
{
    //Buffer buffer = new Buffer(new byte[] { }, true);
    //buffer.WriteInt16(6);
    //buffer.WriteString("Weslei");
    //Console.WriteLine(buffer.ReadString());
    //Console.ReadKey();

    public class Buffer
    {
        int position = 0;
        bool isBigEndian = false;
        List<byte> buffer = new List<byte>();

        public byte this[int i]
        {
            get
            {
                return this.buffer[i];
            }
        }

        public Buffer(byte[] bytes, bool bigEndian)
        {
            this.isBigEndian = bigEndian;
            this.buffer.AddRange(bytes);
        }

        public byte[] ToArray()
        {
            return this.buffer.ToArray();
        }

        public int Length()
        {
            return this.buffer.Count;
        }

        // Read values
        public short ReadInt16()
        {
            return BitConverter.ToInt16(this.getBytes(2), 0);
        }

        public int ReadInt32()
        {
            return BitConverter.ToInt32(this.getBytes(4), 0);
        }

        public long ReadInt64()
        {
            return BitConverter.ToInt64(this.getBytes(8), 0);
        }

        public string ReadString()
        {
            string str = "";
            short size = this.ReadInt16();
            byte[] estring = new byte[size];

            for (int x = this.position, i = 0; x < this.position + size; x++, i++)
            {
                estring[i] = this.buffer[x];
            }

            foreach (byte x in estring)
            {
                str += (char)x;
            }

            this.position += size;
            return str;
        }

        public float ReadFloat()
        {
            return BitConverter.ToSingle(this.getBytes(4), 0);
        }

        public double ReadDouble()
        {
            return BitConverter.ToDouble(this.getBytes(8), 0);
        }

        public bool ReadBool()
        {
            return this.getBytes(1)[0] == 0 ? false : true;
        }

        public byte ReadByte()
        {
            return this.getBytes(1)[0];
        }

        public char ReadChar()
        {
            return (char)this.ReadByte();
        }

        // Write values
        public void WriteInt16(short v)
        {
            byte[] bytes = BitConverter.GetBytes(v);
            if (BitConverter.IsLittleEndian && this.isBigEndian)
            {
                Array.Reverse(bytes);
            }
            this.buffer.AddRange(bytes);
        }

        public void WriteInt32(int v)
        {
            byte[] bytes = BitConverter.GetBytes(v);
            if (BitConverter.IsLittleEndian && this.isBigEndian)
            {
                Array.Reverse(bytes);
            }
            this.buffer.AddRange(bytes);
        }

        public void WriteInt64(long v)
        {
            byte[] bytes = BitConverter.GetBytes(v);
            if (BitConverter.IsLittleEndian && this.isBigEndian)
            {
                Array.Reverse(bytes);
            }
            this.buffer.AddRange(bytes);
        }

        public void WriteFloat(float v)
        {
            byte[] bytes = BitConverter.GetBytes(v);
            if (BitConverter.IsLittleEndian && this.isBigEndian)
            {
                Array.Reverse(bytes);
            }
            this.buffer.AddRange(bytes);
        }

        public void WriteDouble(double v)
        {
            byte[] bytes = BitConverter.GetBytes(v);
            if (BitConverter.IsLittleEndian && this.isBigEndian)
            {
                Array.Reverse(bytes);
            }
            this.buffer.AddRange(bytes);
        }

        public void WriteString(string v)
        {
            byte[] bytes = new byte[v.Length];

            for (int x = 0; x < v.Length; x++)
            {
                bytes[x] = (byte)v[x];
            }

            this.buffer.AddRange(bytes);
        }

        public void WriteByte(byte v)
        {
            this.buffer.Add(v);
        }

        public void WriteBytes(byte[] bytes)
        {
            this.buffer.AddRange(bytes);
        }

        public void WriteChar(char v)
        {
            this.WriteByte((byte)v);
        }


        byte[] getBytes(int pos)
        {
            byte[] bytes = new byte[pos];

            for (int x = this.position, i = 0; x < this.position + pos; x++, i++)
            {
                bytes[i] = this.buffer[x];
            }
            if (BitConverter.IsLittleEndian && this.isBigEndian)
            {
                Array.Reverse(bytes);
            }

            this.position += pos;
            return bytes;
        }
    }
}
