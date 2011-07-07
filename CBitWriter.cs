using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ConsoleApplication1
{
    public class CBitWriter
    {
        BinaryWriter bw = null;
        int len_now;
        byte buf;
        byte temp;

        public CBitWriter(ref BinaryWriter bw)
        {
            this.bw = bw;
            ///aaa
        }

        /// <summary>
        /// 値と値長さを受け取って書きこむ
        /// </summary>
        /// <param name="value">値</param>
        /// <param name="v_len">値長※32を超えてはならない</param>
        public void WriteBit(int value, int v_len)
        {
            if (value < 0)
            {
                value = -value;
                for (int i = 0; i < v_len; i++)
                {
                    temp = (byte)((value & 1 << (v_len - i - 1)) >> (v_len - i - 1));
                    temp = (byte)(temp ^ 0x1);
                    buf += (byte)(temp << (7 - len_now));
                    len_now++;
                    if (len_now == 8)
                    {
                        bw.Write(buf);
                        if (buf == 0xff)
                        {
                            bw.Write((byte)0);
                        }
                        buf = 0;
                        len_now = 0;
                    }
                }
            }
            else
            {
                for (int i = 0; i < v_len; i++)
                {
                    temp = (byte)((value & 1 << (v_len - i - 1)) >> (v_len - i - 1));

                    buf += (byte)(temp << (7 - len_now));
                    len_now++;
                    if (len_now == 8)
                    {
                        bw.Write(buf);
                        if (buf == 0xff)
                        {
                            bw.Write((byte)0);
                        }
                        buf = 0;
                        len_now = 0;
                    }
                }
            }
        }

        public void WriteFinal()
        {
            if (len_now > 0)
            {
                for (; len_now < 7; len_now++)
                {
                    buf = (byte)(buf << 1);
                    buf += 1;
                }
                buf = (byte)(buf << (7 - len_now));
                bw.Write((byte)buf);
            }
            //if (len_now != 0)
            //{
            //    //WriteBit(((1 << (8 - len_now)) - 1), (8 - len_now));
            //    WriteBit(0, (8 - len_now));
            //}
        }
    }
}
