using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    public class CbitStream
    {
        public int data_length = 0;
        private int bit_seek;
        private byte[] data;

        public CbitStream(byte[] in_data)
        {
            data_length = in_data.Length * 8;
            bit_seek = 0;
            data = new byte[data_length / 8];

            int j = 0;
            for (int i = 0; i+j < data_length/8-1; i++)
            {
                if ((in_data[i+j] == 0xff) && (in_data[i + j + 1] == 0x00))
                {
                    data[i] = in_data[i + j];
                    j++;
                }
                else
                {
                    data[i] = in_data[i + j];
                }
            }
        }

        public CbitStream(CbitStream prev)
        {
            data_length = prev.data_length;
            bit_seek = prev.bit_seek;

            data = new byte[prev.data.Length];
            prev.data.CopyTo(data, 0);
        }

        /// <summary>
        /// 1ビットゲロ
        /// </summary>
        /// <returns>seek位置がdata長を越えているときは-1</returns>
        public int GetBit()
        {
            int dst;

            if (bit_seek >= data_length)
            {
                return -1;
            }

            dst = (data[(int)Math.Floor((double)(bit_seek / 8))]
                & (1 << (7 - (bit_seek % 8))))
                >> (7 - (bit_seek % 8));
            bit_seek++;

            return dst;
        }

        /// <summary>
        /// nビットゲロゲロ
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public int GetBits(int n)
        {
            int dst = 0;
            if (bit_seek >= data_length)
            {
                return -1;
            }
            for (int i = 0; (i < n) && (bit_seek < data_length) ; i++)
            {
                dst += GetBit() << (n - i - 1);
            }
            
            return dst;
        }

        public void CatchData(int src)
        {            

        }

        public int Value_Length(int src)
        {
            int buf = src;
            for (int i = 0; ; i++)
            {
                if(buf == 0)return i;
                buf = (buf >> 1);
            }
        }

        public void SetSeek(int n)
        {
            this.bit_seek = n;
        }
    }
}
