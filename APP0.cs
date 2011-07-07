using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ConsoleApplication1
{
    /// <summary>
    /// 解像度情報
    /// </summary>
    public class APP0 : marker_base
    {
        byte[] id = new byte[]{ 0xff, 0xe0 };
        public int dpiX;            //X解像度
        public int dpiY;            //Y解像度
        public int thumb_w;         //サムネイル幅
        public int thumb_h;         //サムネイル高
        byte[] JFIF = new byte[4] { 0x4a, 0x46, 0x49, 0x46 };   //JFIF@hex
        byte[] temp = new byte[4] { 0x0, 0x1, 0x1, 0x1 };       //

        //Instance constructor.
        public APP0()
        {
            id = new byte[2] { 0xff, 0xe0 };
        }

        //Copy Constructor.
        public APP0(APP0 prev)
        {
            id = new byte[prev.id.Length];
            prev.id.CopyTo(id, 0);
            head_length = prev.head_length;
            dpiX = prev.dpiX;
            dpiY = prev.dpiY;
            thumb_w = prev.thumb_w;
            thumb_h = prev.thumb_h;
        }
        public APP0(ref BinaryReader br_in)
        {
            read_headsize(ref br_in);
            br_in.ReadBytes(8);
            dpiX = ByteArrToInt(br_in.ReadBytes(2));
            dpiY = ByteArrToInt(br_in.ReadBytes(2));
            thumb_w = br_in.ReadByte();
            thumb_h = br_in.ReadByte();
        }
        public override void ReadMarker(ref BinaryReader br_in)
        {
            read_headsize(ref br_in);
            br_in.ReadBytes(8);
            dpiX = ByteArrToInt(br_in.ReadBytes(2));
            dpiY = ByteArrToInt(br_in.ReadBytes(2));
            thumb_w = br_in.ReadByte();
            thumb_h = br_in.ReadByte();           
        }

        public override void WriteMarker(ref BinaryWriter bw)
        {
            bw.Write(id);
            WriteHeadsize(ref bw);
            bw.Write(JFIF);
            bw.Write(temp);
            WriteWord(ref bw, dpiX);
            WriteWord(ref bw, dpiY);
            bw.Write((byte)thumb_w);
            bw.Write((byte)thumb_h);
        }

        public override void WriteHeadSize(ref BinaryWriter bw)
        {
            throw new NotImplementedException();
        }
    }

}
