using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace ConsoleApplication1
{
    public class CJpegDecoderT
    {
        const int DC = 0;
        const int AC = 1;
        const int EOB = 0x00;
        static int[] t_sel = { 0, 1, 1 };
        
        public static void HuffmanDecode(ref Cjpeg cj)
        {
            int count = 0;
            int c_value;
            int v_length;
            int z_run;
            
            for (int i = 0; i < (cj.cb.block_width * cj.cb.block_height); i++)
            {
                for (int k = 0; k < 3; k++)
                {
                    //DC
                    c_value = GetValueLength(ref cj, t_sel[k], DC);
                    cj.cb.data[k][i][0] = GetValue(ref cj, c_value);

                    //AC
                    for (int j = 1; j < 64;j++)
                    {
                        c_value = GetValueLength(ref cj, t_sel[k], AC);

                        if (c_value == EOB)
                        {
                            break;
                        }
                        else
                        {
                            z_run = (c_value & 0xf0) >> 4;
                            if (z_run != 0)
                            {
                                int aaa;
                            }
                            j += z_run;
                            if (j > 63)
                            {
                                break;
                            }
                            v_length = (c_value & 0x0f);
                            cj.cb.data[k][i][j] = GetValue(ref cj, v_length);
                            //Console.Write(cj.cb.data[count][i, j] + ",");
                        }
                    }

                    //count = ((count + 1) % 3);
                }
            }
        }

        /// <summary>
        /// ルートから探索
        /// </summary>
        /// <returns></returns>
        static public int GetValueLength(ref Cjpeg cj, int t_sel, int ac_dc)
        {
            int seek=0;
            int RL;
            while (true)
            {
                RL = cj.sos.cbs.GetBit();
                seek = cj.dht.table[t_sel, ac_dc].nodes[seek].child[RL];
                if (cj.dht.table[t_sel, ac_dc].nodes[seek].is_reaf == true)
                {
                    return cj.dht.table[t_sel, ac_dc].nodes[seek].value;
                }
            }
        }
        static public int GetValue(ref Cjpeg cj, int v_length)
        {
            if (v_length == 0)
            {
                return 0;
            }

            int msb = cj.sos.cbs.GetBit();
            int dst = 0;

            if (msb == 1)
            {
                dst += 1 << (v_length - 1);
                for (int i = 1; i < v_length; i++)
                {
                    dst += (cj.sos.cbs.GetBit() << (v_length - i - 1));
                }
            }
            else
            {
                dst += 1 << (v_length - 1);
                for (int i = 1; i < v_length; i++)
                {
                    dst += ( (cj.sos.cbs.GetBit()^1) << (v_length - i - 1));
                }
                dst = -dst;
            }

            return dst;
        }
    }
}
