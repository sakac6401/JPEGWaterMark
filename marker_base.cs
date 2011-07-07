using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ConsoleApplication1
{
    public abstract class marker_base
    {
        public int head_length;
        public byte[] id;

        public void read_headsize(ref BinaryReader br_in)
        {
            head_length = (br_in.Read() << 8) + br_in.Read();
        }
        public void WriteHeadsize(ref BinaryWriter bw)
        {
            byte buf = (byte)((head_length & 0xff00) >> 8);
            bw.Write(buf);
            buf = (byte)(head_length & 0xff);
            bw.Write(buf);
        }

        public int ByteArrToInt(byte[] src)
        {
            int dst = 0;
            for (int i = 0; i < src.Length; i++)
            {
                dst += (src[i] << ((src.Length - i - 1) * 8));
            }

            return dst;
        }

        public void WriteWord(ref BinaryWriter bw, int src)
        {
            byte buf = (byte)((src & 0xff00) >> 8);
            bw.Write(buf);
            buf = (byte)(src & 0xff);
            bw.Write(buf);
        }
        public abstract void ReadMarker(ref BinaryReader br_in);
        public abstract void WriteMarker(ref BinaryWriter bw);
        public abstract void WriteHeadSize(ref BinaryWriter bw);
    }
}
