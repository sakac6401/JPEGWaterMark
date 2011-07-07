using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace ConsoleApplication1
{
    public class CBlock
    {
        public int block_width;
        public int block_height;
        public int b_len;
        public int[][][] data = null;


        public CBlock()
        {

        }

        public CBlock(int w, int h)
        {
            this.block_height = h;
            this.block_width = w;
            b_len = w * h;

            data = new int[3][][];
            for (int i = 0; i < 3; i++)
            {
                data[i] = new int[b_len][];
                for (int j = 0; j < b_len; j++)
                {
                    data[i][j] = new int[64];
                }
            }
        }

        public CBlock(CBlock prev)
        {
            block_width = prev.block_width;
            block_height = prev.block_height;
            b_len = prev.b_len;

            data = new int[prev.data.Length][][];
            for (int i = 0; i < 3; i++)
            {
                data[i] = new int[prev.data[i].Length][];
                for (int j = 0; j < prev.data[i].Length; j++)
                {
                    data[i][j] = new int[prev.data[i][j].Length];
                    prev.data[i][j].CopyTo(data[i][j], 0);
                }
            }
        }



        public void WriteAll()
        {
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine("" + i);
                for (int j = 0; j < b_len; j++)
                {
                    Console.WriteLine("color:" + j);
                    for (int k = 0; k < 64; k++)
                    {
                        Console.Write("" + data[i][j][k] + ",");
                    }
                    Console.WriteLine();
                }
            }
        }

        public void WriteData(ref BinaryWriter bw)
        {
            for (int i = 0; i < b_len; i++)
            {
                for (int k = 0; k < 3; k++)
                {
                    for (int j = 0; j < 64; j++)
                    {
                        bw.Write((byte)data[k][i][j]);
                    }
                }
            }
        }
    }
}
