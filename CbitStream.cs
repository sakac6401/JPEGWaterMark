using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    /// <summary>
    /// byte型配列をビット単位で扱えるようにする
    /// </summary>
    public class CbitStream
    {
        public int data_length = 0;
        public int bit_seek;
        public byte[] data;

        /// <summary>
        /// byte配列をビット単位で扱う時
        /// </summary>
        /// <param name="in_data"></param>
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

        /// <summary>
        /// 長さだけ確保（値格納用）
        /// </summary>
        /// <param name="length"></param>
        public CbitStream(int length)
        {
            this.data = new byte[length];
            data_length = length * 8;
        }

        public CbitStream(CbitStream prev)
        {
            data_length = prev.data_length;
            bit_seek = prev.bit_seek;

            data = new byte[prev.data.Length];
            prev.data.CopyTo(data, 0);
        }

        /// <summary>
        /// 1ビット吐き出す
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

        /// <summary>
        /// 1ビット受け取る．ビットシークは１進む
        /// </summary>
        /// <param name="src">受け取るデータ．1以上は1とみなす．</param>
        public void CatchBit(int bit)
        {
            this.data[(int)Math.Floor((double)bit_seek / 8)] ^= (byte)(Indicate(bit) << (7 - (bit_seek % 8)));
            bit_seek++;
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

        /// <summary>
        /// ビットシークを指定の位置にセットする
        /// </summary>
        /// <param name="n">シークのセット位置（ビット単位）</param>
        public void SetSeek(int n)
        {
            this.bit_seek = n;
        }

        /// <summary>
        /// 1以上なら1，0なら0を返す
        /// </summary>
        /// <param name="a"></param>
        private int Indicate(int a)
        {
            if (a > 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public void Initialize()
        {
            for (int i = 0; i < data.Length; i++)
            {
                this.data[i] = 0;
            }
            this.bit_seek = 0;
            this.data_length = 0;
        }

        public static bool operator == (CbitStream cbs1, CbitStream cbs2)
        {
            if (cbs1.data.Length != cbs2.data.Length)
            {
                return false;
            }
            for (int i = 0; i < cbs1.data.Length; i++)
            {
                if (cbs1.data[i] != cbs2.data[i])
                {
                    return false;
                }
            }
            return true;
        }
        public static bool operator !=(CbitStream cbs1, CbitStream cbs2)
        {
            if (cbs1.data.Length != cbs2.data.Length)
            {
                return true;
            }
            for (int i = 0; i < cbs1.data.Length; i++)
            {
                if (cbs1.data[i] != cbs2.data[i])
                {
                    return true;
                }
            }
            return false;
        }
    }
}
