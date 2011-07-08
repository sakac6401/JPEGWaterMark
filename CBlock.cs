using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace ConsoleApplication1
{
    public class CBlock
    {
        /// <summary>
        /// 画像をブロック化したときのブロック幅
        /// </summary>
        public int block_width;
        /// <summary>
        /// 画像をブロック化したときのブロック高
        /// </summary>
        public int block_height;
        /// <summary>
        /// ブロック長=block_width * block_height
        /// </summary>
        public int b_len;
        /// <summary>
        /// 量子化DCT係数[色番号][ブロック番号][係数番号(0-63)]
        /// </summary>
        public int[][][] data = null;

        /// <summary>
        /// 画像をブロックに分割したときのブロック幅[色番号]
        /// </summary>
        public int[] blockWidth = null;
        /// <summary>
        /// 画像をブロックに分割したときのブロック高[色番号]
        /// </summary>
        public int[] blockHeight = null;
        /// <summary>
        /// ブロック長[色番号]
        /// </summary>
        public int[] blockLength = null;

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

        public CBlock(SOF0 sof0)
        {
            data = new int[sof0.numSample][][];         //色数
            blockWidth = new int[sof0.numSample];
            blockHeight = new int[sof0.numSample];      //
            blockLength = new int[sof0.numSample];      //
            double buf;
            for (int i = 0; i < sof0.numSample; i++)    //水平方向処理
            {
                buf = (double)sof0.width / 8.0 * ((double)sof0.SampleRatioH[i] / (double)sof0.SampleRatioH[0]);
                blockWidth[i] = (int)Math.Ceiling(buf);

                buf = (double)sof0.height / 8.0 * ((double)sof0.SampleRatioV[i] / (double)sof0.SampleRatioV[0]);
                blockHeight[i] = (int)Math.Ceiling(buf);

                blockLength[i] = blockWidth[i] * blockHeight[i];
                data[i] = new int[blockLength[i]][];
                for (int j = 0; j < 64; j++)
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
