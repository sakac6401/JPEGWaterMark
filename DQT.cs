using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ConsoleApplication1
{
    /// <summary>
    /// 量子化テーブル定義
    /// </summary>
    public class DQT : marker_base
    {
        public int num_color;
        /// <summary>
        /// 量子化テーブル[テーブル番号][要素番号]
        /// </summary>
        public byte[][] table;
        public int t_size;

        //Instance Constructor
        public DQT()
        {
            id = new byte[2] { 0xff, 0xdb };
            num_color = 0;
            table = new byte[2][];
        }

        public DQT(DQT prev)
        {
            id = new byte[prev.id.Length];
            prev.id.CopyTo(id, 0);
            head_length = prev.head_length;
            num_color = prev.num_color;
            t_size = prev.t_size;

            table = new byte[num_color][];
            for (int i = 0; i < num_color; i++)
            {
                table[i] = new byte[64];
                prev.table[i].CopyTo(table[i], 0);
            }
        }

        public override void ReadMarker(ref BinaryReader br_in)
        {
            try
            {
                read_headsize(ref br_in);
                t_size = head_length - 3;
                this.table[br_in.ReadByte()] = new byte[t_size];

                for (int i = 0; i < 64; i++)
                {
                    this.table[this.num_color][i] = (byte)br_in.ReadByte();
                }

                this.num_color++;
            }
            catch
            {
                Console.WriteLine("dqt.read error");
            }
        }

        public override void WriteMarker(ref BinaryWriter bw)
        {
            for (int i = 0; i < num_color; i++)
            {
                bw.Write(id);

                WriteHeadSize(ref bw);
                bw.Write((byte)(i));
                for (int j = 0; j < 64; j++)
                {
                    bw.Write((byte)table[i][j]);
                }
            }
        }

        public override void WriteHeadSize(ref BinaryWriter bw)
        {
            WriteWord(ref bw, t_size + 3);
        }
    }
}
