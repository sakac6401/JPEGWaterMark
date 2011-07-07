using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace ConsoleApplication1
{
    //埋込の前にやっておくこと
    //・DCの逆差分化
    //・逆量子化
    //保存の前にやっておくこと
    //・DCの差分化
    //・量子化
    public class WaterMarkingT
    {     
        //埋込
        public static void Embed(ref Cjpeg cj, string passwd, int embed_bits, int offset, int color)
        {
            UnDiffDC(ref cj);
            //UnQuantize();

            int[][][] temp = GetCbTemp(ref cj);
            ValueCombing(ref cj, embed_bits, offset, color);
            EmbedHash(ref cj, temp, passwd, embed_bits, offset, color);
            //AddRand(embed_bits, offset, color);

            DiffDC(ref cj);
            //Quantize();
        }
        //値を間引く
        static void ValueCombing(ref Cjpeg cj, int embed_bits, int offset, int color)
        {
            for (int k = 0; k < color; k++)
            {
                for (int i = 0; i < cj.cb.b_len; i++)
                {
                    for (int j = offset; j < (embed_bits + offset); j++)
                    {
                        // -1<=value<=1 -> 0
                        if ((-1 <= cj.cb.data[k][i][j]) && (cj.cb.data[k][i][j] <= 1))
                        {
                            cj.cb.data[k][i][j] = 0;
                        }
                        // 1 < value
                        if (cj.cb.data[k][i][j] > 1)
                        {
                            cj.cb.data[k][i][j] -= (cj.cb.data[k][i][j] % 2);
                        }
                        // value < -1
                        if (cj.cb.data[k][i][j] < -1)
                        {
                            cj.cb.data[k][i][j] += (-cj.cb.data[k][i][j] % 2);
                        }
                    }
                }
            }
        }

        static int[][][] GetCbTemp(ref Cjpeg cj){
            int[][][] dst = new int[3][][];

            for (int k = 0; k < 3; k++)
            {
                dst[k] = new int[cj.cb.b_len][];
                for (int i = 0; i < cj.cb.b_len; i++)
                {
                    dst[k][i] = new int[64];
                }
            }

            for (int k = 0; k < 3; k++)
            {
                for (int i = 0; i < cj.cb.b_len; i++)
                {
                    for (int j = 0; j < 64; j++)
                    {
                        dst[k][i][j] = cj.cb.data[k][i][j];
                    }
                }
            }

            return dst;
        }

        static void EmbedHash(ref Cjpeg cj, int[][][] temp, string passwd, int embed_bits, int offset, int color)
        {
            int bit_seek = 0;
            int array_seek = 0;
            byte[] hash_key;
            byte[] hash_value;
            SHA256 sha = SHA256.Create();

            for (int k = 0; k < color; k++)
            {
                for (int i = 0; i < cj.cb.b_len; i++)
                {
                    hash_key = GetHashKey(ref cj, passwd, k, i);
                    hash_value = sha.ComputeHash(hash_key);

                    for (int j = offset; j < (embed_bits + offset); j++)
                    {
                        if ((hash_value[array_seek] & (1 << (7 - bit_seek))) != 0)
                        {
                            cj.cb.data[k][i][j] = SetValue(ref cj, temp, k, i, j);
                        }

                        bit_seek++;
                        if (bit_seek == 8)
                        {
                            array_seek++;
                            bit_seek = 0;
                        }
                    }
                    array_seek = 0;
                    bit_seek = 0;
                }
            }
        }

        //埋込ビットが1の場合の動作
        static int SetValue(ref Cjpeg cj, int[][][] temp, int color, int b_idx, int dct_idx)
        {
            Random rand = new Random(temp[color][b_idx][dct_idx]);
            if (cj.cb.data[color][b_idx][dct_idx] == 0)
            {
                //埋込先が0の場合、-1 か 1のどっちかにランダムにプロット
                if (cj.cb.data[color][b_idx][dct_idx] == 0)
                {
                    if (rand.Next() % 2 == 0)
                    {
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    return temp[color][b_idx][dct_idx];
                }
                
            }
            if (cj.cb.data[color][b_idx][dct_idx]< -1)
            {
                return (cj.cb.data[color][b_idx][dct_idx] - 1);
            }
            if (1 < cj.cb.data[color][b_idx][dct_idx])
            {
                return (cj.cb.data[color][b_idx][dct_idx] + 1);
            }

            return 0;
        }

        static byte[] GetHashKey(ref Cjpeg cj, string passwd, int color, int b_idx)
        {
            byte[] buf = new byte[71];
            byte[] pass = StringToByte(passwd);
            byte[] dst;

            buf.Initialize();
            for (int i = 0; i < 64; i++)
            {
                if (cj.cb.data[color][b_idx][i] > 0)
                {
                    buf[i] = (byte)cj.cb.data[color][b_idx][i];
                }
            }
            buf[64] = (byte)((cj.sof.width & 0xff00) >> 8);
            buf[65] = (byte)(cj.sof.width & 0xff);
            buf[66] = (byte)((cj.sof.height & 0xff00) >> 8);
            buf[67] = (byte)(cj.sof.height & 0xff);
            buf[68] = (byte)color;
            buf[69] = (byte)((b_idx & 0xff00) >> 8);
            buf[70] = (byte)(b_idx & 0xff);

            dst = new byte[buf.Length + pass.Length];
            buf.CopyTo(dst, 0);
            pass.CopyTo(dst, buf.Length);

            return dst;
        }

        static byte[] StringToByte(string src)
        {
            byte[] dst = new byte[12];

            for (int i = 0; (i < 12) && (i < src.Length); i++)
            {
                dst[i] = (byte)src[i];
            }

            return dst;
        }

        public static int[] Check(ref Cjpeg temp, string passwd, int embed_bits, int offset, int color)
        {
            Cjpeg cj = new Cjpeg(temp);
            int[] error = new int[cj.cb.b_len];
            error.Initialize();
            int err_count = 0;
            byte[] extract_data;
            byte[] hash_key;
            byte[] hash_value;
            SHA256 sha = SHA256.Create();

            UnDiffDC(ref cj);
            for (int k = 0; k < 3; k++)
            {
                for (int i = 0; i < cj.cb.b_len; i++)
                {
                    extract_data = DataSeparation(ref cj, embed_bits, offset, k, i);
                    hash_key = GetHashKey(ref cj, passwd, k, i);
                    hash_value = sha.ComputeHash(hash_key);
                    error[i] += CheckHash(extract_data, hash_value, embed_bits);
                    err_count += error[i];
                }
            }

            return error;
        }

        public static byte[] DataSeparation(ref Cjpeg cj, int embed_bits, int offset, int color, int b_idx)
        {
            byte[] dst = new byte[1 + (int)(embed_bits/8)];
            dst.Initialize();
            for (int i = offset; i < (offset + embed_bits); i++)
            {
                if (cj.cb.data[color][b_idx][i] < -1)
                {
                    dst[(int)(i / 8)] += (byte)((-cj.cb.data[color][b_idx][i] % 2) << (7 - (i % 8)));
                    cj.cb.data[color][b_idx][i] += (-cj.cb.data[color][b_idx][i] % 2);
                }
                else if (cj.cb.data[color][b_idx][i] > 1)
                {
                    dst[(int)(i / 8)] += (byte)((cj.cb.data[color][b_idx][i] % 2) << (7 - (i % 8)));
                    cj.cb.data[color][b_idx][i] -= (cj.cb.data[color][b_idx][i] % 2);
                }
                else
                {
                    if (cj.cb.data[color][b_idx][i] != 0)
                    {
                        dst[(int)(i / 8)] += (byte)(1 << (7 - (i % 8)));
                        cj.cb.data[color][b_idx][i] = 0;
                    }
                }
            }
            return dst;
        }

        static int CheckHash(byte[] hashA, byte[] hashB, int embed_bits)
        {

            for (int i = 0; i < embed_bits; i++)
            {
                if ( (hashA[(int)(i / 8)] & (byte)(1 << 7 - (i % 8))) !=
                     (hashB[(int)(i / 8)] & (byte)(1 << 7 - (i % 8))) )
                {
                    return 1;
                }
            }
            return 0;
        }

        //public void Extract(int embed_bits, int offset, int color)
        //{
        //    int bit_seek = 0;
        //    int array_seek = 0;
        //    byte[] extract_data = new byte[8];

        //    UnDiffDC();

        //    for (int k = 0; k < color; k++)
        //    {
        //        for (int i = 0; i < cj.cb.b_len; i++)
        //        {
        //            for (int j = 0; j < 8; j++)
        //            {
        //                extract_data[j] = 0;
        //            }
        //            if (i == 37)
        //            {
        //                int aaa;
        //            }

        //            for (int j = offset; j < (offset + embed_bits); j++)
        //            {
        //                extract_data[array_seek] += (byte)(SeparateValue(k,i,j) << (7 - bit_seek));
        //                bit_seek++;
        //                if (bit_seek > 7)
        //                {
        //                    array_seek++;
        //                    bit_seek = 0;
        //                }
        //            }
        //            GetHashBuf(k, i);
        //            hash_value = sha.ComputeHash(hash_buf);
        //            error[i] += (byte)CheckHash(i, embed_bits, extract_data, hash_value);
        //            bit_seek = 0;
        //            array_seek = 0;
        //        }
        //    }
        //}

        ////ハッシュ値を比較して、値が異なれば1を返す
        //public int CheckHash(int b_idx, int embed_bits, byte[] hashA, byte[] hashB)
        //{
        //    for (int i = 0; i < embed_bits; i++)
        //    {
        //        if ((hashA[(int)(i / 8)] & (byte)(1 << (7 - (i % 8))))
        //         != (hashB[(int)(i / 8)] & (byte)(1 << (7 - (i % 8)))))
        //        {
        //            //error[b_idx]++;
        //            return 1;
        //        }
        //    }

        //    return 0;
        //}

        ////透かし埋込済みDCT係数から値を取り出しつつ値を元に戻す
        //public int SeparateValue(int color, int b_idx, int dct_idx)
        //{
        //    int dst;

        //    if ((-1 <= cj.cb.data[color][b_idx][dct_idx]) && (cj.cb.data[color][b_idx][dct_idx] <= 1))
        //    {
        //        if (cj.cb.data[color][b_idx][dct_idx] == 0)
        //        {
        //            return 0;
        //        }
        //        else
        //        {
        //            cj.cb.data[color][b_idx][dct_idx] = 0;
        //            return 1;
        //        }
        //    }
        //    if (cj.cb.data[color][b_idx][dct_idx] > 1)
        //    {
        //        dst = (cj.cb.data[color][b_idx][dct_idx] % 2);
        //        cj.cb.data[color][b_idx][dct_idx] -= dst;
        //        return dst;
        //    }
        //    if (cj.cb.data[color][b_idx][dct_idx] < -1)
        //    {
        //        dst = (-cj.cb.data[color][b_idx][dct_idx] % 2);
        //        cj.cb.data[color][b_idx][dct_idx] += dst;
        //        return dst;
        //    }

        //    return 0;
        //}

        //public CBitmap CheckError()
        //{
        //    CBitmap dst = new CBitmap(cj.sof.width, cj.sof.height);
        //    for (int i = 0; i < dst.width; i++)
        //    {
        //        for (int j = 0; j < dst.height; j++)
        //        {
        //            if (error[i] == 1)
        //            {
        //                dst.SetPixel(i * 8, (i * 8) + 7, j * 8, (j * 8) + 7, 255, 0, 0);
        //            }
        //        }
        //    }
        //    return dst;
        //}

        //void 
        //儀式用関数群
        //逆差分化
        static void UnDiffDC(ref Cjpeg cj)
        {
            for (int i = 1; i < cj.cb.b_len; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    cj.cb.data[j][i][0] += cj.cb.data[j][i - 1][0];
                }
            }
        }
        //差分化
        static void DiffDC(ref Cjpeg cj)
        {
            int buf = cj.cb.data[0][0][0];
            for (int i = cj.cb.b_len-1; i > 0; i--)
            {
                for (int j = 0; j < 3; j++)
                {
                    cj.cb.data[j][i][0] -= cj.cb.data[j][i - 1][0];
                }
            }
        }
        //逆量子化
        static void UnQuantize(ref Cjpeg cj)
        {
            for (int i = 0; i < cj.cb.b_len; i++)
            {
                for (int k = 0; k < 3; k++)
                {
                    for (int j = 0; j < 64; j++)
                    {
                        cj.cb.data[k][i][j] *= cj.dqt.table[cj.sof.DQTSelecter[k]][j];
                    }
                }
            }
        }
        //量子化
        static void Quantize(ref Cjpeg cj)
        {
            for (int i = 0; i < cj.cb.b_len; i++)
            {
                for (int k = 0; k < 3; k++)
                {
                    for (int j = 0; j < 64; j++)
                    {
                        cj.cb.data[k][i][j] = (int)Math.Round((double)(cj.cb.data[k][i][j] / cj.dqt.table[cj.sof.DQTSelecter[k]][j]));
                    }
                }
            }
        }
        //平均
        double PutAverage(int[] data)
        {
            double sum = 0;
            for (int i = 0; i < data.Length; i++)
            {
                sum += data[i];
            }
            return (sum / data.Length);
        }

    }
}
