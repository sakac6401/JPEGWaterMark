using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ConsoleApplication1
{
    public class SOS : marker_base
    {
        public CbitStream cbs;
        byte[] temp = new byte[] { 0x00, 0x0C, 0x03, 0x01, 0x00, 0x02, 0x11, 0x03, 0x11, 0x00, 0x3F, 0x00 };
        byte[] marker = new byte[] { 0xff, 0xda };

        public SOS()
        {

        }

        public SOS(SOS prev)
        {
            if (prev.id != null)
            {
                id = new byte[prev.id.Length];
                prev.id.CopyTo(id, 0);
            }
            head_length = prev.head_length;

            cbs = new CbitStream(prev.cbs);

        }

        public override void ReadMarker(ref BinaryReader br_in)
        {
            try
            {
                read_headsize(ref br_in);
                br_in.ReadBytes(this.head_length - 2);
                cbs = new CbitStream(br_in.ReadBytes((int)(br_in.BaseStream.Length - br_in.BaseStream.Position - 2)));
            }
            catch
            {
                Console.WriteLine("sos.read error");
            }
        }

        public override void  WriteMarker(ref BinaryWriter bw)
        {
            bw.Write(marker);
            bw.Write(temp);
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
