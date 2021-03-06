﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ConsoleApplication1
{
    public class CJpegEncoderT
    {
        const int EOB = 0;
        const int ZRL = 0xf0;
        const int DC = 0;
        const int AC = 1;
        CBitWriter cbw = null;
        BinaryWriter bw = null;



        static public void WriteFile(ref Cjpeg cj, string path)
        {
            byte[] soi =  new byte[2]{ 0xff, 0xd8 };
            byte[] eoi =  { 0xff, 0xd9 };
            BinaryWriter bw = new BinaryWriter(File.Open(path, FileMode.Create));
            bw.Write(soi);
            cj.app0.WriteMarker(ref bw);
            cj.dqt.WriteMarker(ref bw);
            cj.sof.WriteMarker(ref bw);
            cj.dht.WriteMarker(ref bw);
            cj.sos.WriteMarker(ref bw);
            try
            {
                WriteImgData(ref cj, ref bw);
            }
            catch
            {
                Console.WriteLine("WriteImgData Erorr");
            }
            bw.Write(eoi);
            bw.Close();
        }

        static public void WriteImgData(ref Cjpeg cj, ref BinaryWriter bw)
        {
            byte v_len;
            byte zero_run;
            int code;
            int v_code;
            int l_code;
            int EOB_idx;
            CBitWriter cbw = new CBitWriter(ref bw);
            //ブロック長ループ
            for (int i = 0; i < cj.cb.b_len; i++)
            {
                //色ループ
                for (int j = 0; j < 3; j++)
                {
                            if (i == 4095)
                            {
                                int aaaaa;
                            }

                    EOB_idx = SearchEOBStart(ref cj, j, i);
                    //DC
                    v_len = GetValueLength(cj.cb.data[j][i][0]);
                    GetCode(ref cj, v_len, j, DC, out v_code,out l_code);
                    cbw.WriteBit(v_code, l_code);
                    cbw.WriteBit(cj.cb.data[j][i][0], v_len);

                    //AC
                    zero_run = 0;
                    for (int k = 1; k < 64; k++)
                    {
                        if (k == EOB_idx)
                        {
                            GetCode(ref cj, EOB, j, AC, out v_code, out l_code);
                            cbw.WriteBit(v_code, l_code);

                            break;
                        }

                        else if (cj.cb.data[j][i][k] == 0)
                        {
                            zero_run++;
                            if (zero_run == 16)
                            {
                                GetCode(ref cj, ZRL, j, AC, out v_code, out l_code);
                                cbw.WriteBit(v_code, l_code);
                                zero_run = 0;
                            }
                        }
                        else
                        {
                            v_len = (byte)((zero_run << 4) + GetValueLength(cj.cb.data[j][i][k]));
                            GetCode(ref cj, v_len, j, AC, out v_code, out l_code);
                            cbw.WriteBit(v_code, l_code);
                            cbw.WriteBit(cj.cb.data[j][i][k], v_len&0x0f);
                            zero_run = 0;
                        }
                    }
                }
            }
            cbw.WriteFinal();
        }



        /// <summary>
        /// １０進数を２進数に置き換えたときのビット長
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static public byte GetValueLength(int value)
        {
            int buf = value;
            if (value < 0)
            {
                buf = -value;
            }
            for (byte i = 0; ; i++)
            {
                if (buf == 0) return i;
                buf = (buf >> 1);
            }
        }

        static public int SearchEOBStart(ref Cjpeg cj, int color, int b_idx)
        {
            for (int i = 63; i > -1; i--)
            {
                if (cj.cb.data[color][b_idx][i] != 0)
                {
                    return i+1;
                }
            }

            return 1;
        }

        /// <summary>
        /// 値長からコードを取得
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static public void GetCode(ref Cjpeg cj, int v_len, int YCbCr, int AC_DC,  out int v_code, out int c_len)
        {
            int dst_node = 0;
            int dst = 0;
            int seek = 0;
            int code_len = 0;
            c_len = 0;
            v_code = 0;
            CbinaryTree cbt = cj.dht.table[cj.sof.t_sel[YCbCr], AC_DC];

            //葉探索、終着ノードを求める
            for (int i = 0; i < cbt.t_reaf.Length; i++)
            {
                if (cbt.nodes[cbt.t_reaf[i]].value == v_len)
                {
                    dst_node = cbt.t_reaf[i];
                    code_len = cbt.nodes[dst_node].depth;
                    seek = dst_node;
                    c_len = code_len;
                    break;
                }
            }

            if (dst_node == 0)
            {

                return;
            }

            //ルート探索、見ている親ノードと見ているノードが一致するか
            //for (int i = 0; i < code_len; i++)
            for (int i = 0; seek > 0; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    if (cbt.nodes[cbt.nodes[seek].parent].child[j] == seek)
                    {
                        //Console.Write(j);
                        dst += (j << i);
                        seek = cbt.nodes[seek].parent;
                        break;
                    }
                }
            }

            v_code = dst;
        }

    }
}
