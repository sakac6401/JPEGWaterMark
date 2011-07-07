using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    public class CbitStream
    {
        public int data_length;
        private int bit_seek;
        private byte[] data;

        public CbitStream(byte[] in_data)
        {
            data_length = in_data.Length * 8;
            bit_seek = 0;
            data = new byte[data_length / 8];
            for (int i = 0; i < data_length/8; i++)
            {
                data[i] = in_data[i];
            }
        }

        /// <summary>
        /// 1ビットゲロ
        /// </summary>
        /// <returns>seek位置がdata長を越えているときは-1</returns>
        public int getBit()
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
        public int getBits(int n)
        {
            int dst = 0;
            if (bit_seek >= data_length)
            {
                return -1;
            }
            for (int i = 0; (i < n) && (bit_seek < data_length) ; i++)
            {
                dst += getBit() << (n - i - 1);
            }
            
            return dst;
        }

        
        public void setSeek(int n)
        {
            this.bit_seek = n;
        }
    }
}
