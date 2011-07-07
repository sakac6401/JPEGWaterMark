using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ConsoleApplication1
{
    public class SOF0 : marker_base
    {
        byte[] id = new byte[]{ 0xff, 0xc0 };   //id
        public int height;              //画像高
        public int width;               //画像幅
        public byte acc_sampling;       //1ブロックのサンプル数
        public byte n_sample;           //色サンプル数
        public byte[] subsample_ratio;  //サブサンプリング比[色番号]
        //public byte[] t_sel;            //量子化テーブルセレクタ[色番号]
        public int[] SampleRatioV;      //垂直サンプリング比[色番号]
        public int[] SampleRatioH;      //水平サンプリング比[色番号]
        public byte[] DQTSelecter;      //量子化テーブルセレクタ[色番号]

        //コンストラクタ
        public SOF0(ref BinaryReader br_in)
        {
            try
            {
                read_headsize(ref br_in);
                acc_sampling = br_in.ReadByte();
                height = (br_in.ReadByte() << 8) + br_in.ReadByte();
                width = (br_in.Read() << 8) + br_in.ReadByte();
                n_sample = br_in.ReadByte();

                subsample_ratio = new byte[n_sample];
                SampleRatioV = new int[n_sample];
                SampleRatioH = new int[n_sample];
                DQTSelecter = new byte[n_sample];
                //t_sel = new byte[n_sample];

                //色情報読取
                for (int i = 0; i < n_sample; i++)
                {
                    br_in.ReadByte();
                    subsample_ratio[i] = br_in.ReadByte();
                    DQTSelecter[i] = br_in.ReadByte();
                    //t_sel[i] = br_in.ReadByte();
                }
            }
            catch
            {
                Console.WriteLine("sof0.read error");
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
        //        acc_sampling = br_in.ReadByte();
        //        height = (br_in.ReadByte() << 8) + br_in.ReadByte();
        //        width = (br_in.Read() << 8) + br_in.ReadByte();
        //        n_sample = br_in.ReadByte();

        //        subsample_ratio = new byte[n_sample];
        //        SampleRatioV = new int[n_sample];
        //        SampleRatioH = new int[n_sample];
        //        DQTSelecter = new byte[n_sample];
        //        //t_sel = new byte[n_sample];

        //        //色情報読取
        //        for (int i = 0; i < n_sample; i++)          
        //        {
        //            br_in.ReadByte();
        //            subsample_ratio[i] = br_in.ReadByte();
        //            DQTSelecter[i] = br_in.ReadByte();
        //            //t_sel[i] = br_in.ReadByte();
        //        }
        //    }
        //    catch
        //    {
        //        Console.WriteLine("sof0.read error");
        //    }
        //}

        public SOF0()
        {
            id = new byte[2] { 0xff, 0xc0 };
        }

        //コピーコンストラクタ
        public SOF0(SOF0 prev)
        {
            id = new byte[prev.id.Length];
            prev.id.CopyTo(id, 0);
            head_length = prev.head_length;
            height = prev.height;
            width = prev.width;

            acc_sampling = prev.acc_sampling;
            n_sample = prev.n_sample;

            subsample_ratio = new byte[prev.subsample_ratio.Length];
            prev.subsample_ratio.CopyTo(subsample_ratio, 0);
            DQTSelecter = new byte[prev.DQTSelecter.Length];
            prev.DQTSelecter.CopyTo(DQTSelecter, 0);
            //t_sel = new byte[prev.t_sel.Length];
            //prev.t_sel.CopyTo(t_sel, 0);
        }
        


        public override void WriteMarker(ref BinaryWriter bw)
        {
            bw.Write(id);
            WriteWord(ref bw, head_length);
            bw.Write(acc_sampling);
            WriteWord(ref bw, height);
            WriteWord(ref bw, width);
            bw.Write(n_sample);
            for (int i = 0; i < n_sample; i++)
            {
                bw.Write((byte)(i + 1));
                bw.Write(subsample_ratio[i]);
                bw.Write(DQTSelecter[i]);
            }
        }

        public override void WriteHeadSize(ref BinaryWriter bw)
        {
            throw new NotImplementedException();
        }
    }
}
