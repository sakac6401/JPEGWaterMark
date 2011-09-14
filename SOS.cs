using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ConsoleApplication1
{
    public class SOS : marker_base
    {
        byte[] marker = new byte[] { 0xff, 0xda };  //マーカ
        int numColor;                               //色数
        public CbitStream cbs;
        public int[] DHTSelDC;                      //DCハフマンテーブル指定[色番号]
        public int[] DHTSelAC;                      //ACハフマンテーブル指定[色番号]

        public SOS(ref BinaryReader br_in)
        {
            try
            {
                read_headsize(ref br_in);
                numColor = br_in.ReadByte();

                DHTSelAC = new int[numColor];
                DHTSelDC = new int[numColor];

                for (int i = 0; i < numColor; i++)
                {
                    br_in.ReadByte();
                    int buf = br_in.ReadByte();
                    DHTSelDC[i] = ((buf & (1 << 4)) >> 4);
                    DHTSelAC[i] = (buf & 1);
                }
                //br_in.ReadBytes(this.head_length - 2);
                br_in.ReadBytes(3);
                cbs = new CbitStream(br_in.ReadBytes((int)(br_in.BaseStream.Length - br_in.BaseStream.Position - 1)));
            }
            catch
            {
                Console.WriteLine("sos.read error");
            }
        }

        public override void ReadMarker(ref BinaryReader br_in)
        {
            throw new NotImplementedException();
        }

        //public override void ReadMarker(ref BinaryReader br_in)
        //{
        //    try
        //    {
        //        read_headsize(ref br_in);
        //        numColor = br_in.ReadByte();

        //        DHTSelAC = new int[numColor];
        //        DHTSelDC = new int[numColor];

        //        for (int i = 0; i < numColor; i++)
        //        {
        //            br_in.ReadByte();
        //            int buf = br_in.ReadByte();
        //            DHTSelDC[i] = ((buf & (1 << 4))>>4);
        //            DHTSelAC[i] = (buf & 1);
        //        }
        //        //br_in.ReadBytes(this.head_length - 2);
        //        br_in.ReadBytes(3);
        //        cbs = new CbitStream(br_in.ReadBytes((int)(br_in.BaseStream.Length - br_in.BaseStream.Position - 2)));
        //    }
        //    catch
        //    {
        //        Console.WriteLine("sos.read error");
        //    }
        //}

        public SOS(SOS prev)
        {
            if (prev.id != null)
            {
                id = new byte[prev.id.Length];
                prev.id.CopyTo(id, 0);
            }
            head_length = prev.head_length;

            cbs = new CbitStream(prev.cbs);
            DHTSelAC = new int[prev.DHTSelAC.Length];
            prev.DHTSelAC.CopyTo(DHTSelAC, 0);
            DHTSelDC = new int[prev.DHTSelDC.Length];
            prev.DHTSelDC.CopyTo(DHTSelDC, 0);
        }

        public override void  WriteMarker(ref BinaryWriter bw)
        {
            bw.Write(marker);
            bw.Write((byte)((this.head_length & (0xff << 4)) >> 4));
            bw.Write((byte)(this.head_length & 0xff));
            
            bw.Write((byte)numColor);
            for (byte i = 0; i < numColor; i++)
            {
                bw.Write((byte)(i+1));
                byte buf = (byte)((DHTSelDC[i] << 4) + DHTSelAC[i]);
                bw.Write(buf);
            }
            byte[] aaa = new byte[3]{0x00,0x3f,0x00};
            bw.Write(aaa);
            
            //bw.Write(temp);
        }

        public override void WriteHeadSize(ref BinaryWriter bw)
        {
            throw new NotImplementedException();
        }

        public void WriteImgData(ref BinaryWriter bw)
        {

        }
    }
}
