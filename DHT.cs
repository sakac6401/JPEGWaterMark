using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ConsoleApplication1
{
    /// <summary>
    /// ハフマンテーブル定義
    /// </summary>
    public class DHT : marker_base
    {
        public CbinaryTree[,] table;
        public DHT()
        {
            id = new byte[2] { 0xff, 0xc4 };
            table = new CbinaryTree[2, 2];
        }

        public DHT(DHT prev)
        {
            id = new byte[prev.id.Length];
            head_length = prev.head_length;
            prev.id.CopyTo(id, 0);
            table = prev.table;
        }

        public override void ReadMarker(ref BinaryReader br_in)
        {
            try
            {
                read_headsize(ref br_in);
                int table_id = br_in.Read();
                int color = (table_id & 0x0f);
                int ac_dc = (table_id & 0xf0) >> 4;

                table[color, ac_dc] = new CbinaryTree(br_in.ReadBytes(16));
                table[color, ac_dc].SetValue(br_in.ReadBytes(table[color, ac_dc].sum_reaves));
            }
            catch
            {
                Console.WriteLine("dht.read error");
            }
        }

        public override void WriteMarker(ref BinaryWriter bw)
        {

            for (int i = 0; i < 2; i++)
            {

                for (int j = 0; j < 2; j++)
                {
                    bw.Write(id);
                    WriteWord(ref bw, CalcHeadLength(i,j));
                    bw.Write((byte)((j<<4) + i));
                    for (int k = 0; k < 16; k++)
                    {
                        bw.Write(table[i, j].depth_reaf_num[k]);
                    }
                    for (int k = 0; k < table[i, j].sum_nodes - 1; k++)
                    {
                        if (table[i, j].nodes[k].is_reaf)
                        {
                            bw.Write((byte)table[i, j].nodes[k].value);
                        }
                    }
                }
            }
        }

        public int CalcHeadLength(int YCbCr, int AC_DC)
        {
            int dst = 16;
            for (int i = 0; i < 16; i++)
            {
                dst += table[YCbCr, AC_DC].depth_reaf_num[i];
            }

            return (dst+3);
        }

        public override void WriteHeadSize(ref BinaryWriter bw)
        {
            throw new NotImplementedException();
        }
    }
}
