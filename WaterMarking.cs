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
    public class WaterMarking
    {
        Cjpeg cj;
        SHA256 sha = SHA256.Create();
        int[][][] temp;
        int[] t_sel = new int[3] { 0, 1, 1 };
        byte[] hash_buf = new byte[64];
        byte[] hash_value;
        byte[] error;
        BinaryWriter bw_embed = new BinaryWriter(File.Open(@"h:\hash.txt", FileMode.Create));
        Random rand = new Random(100);
        byte[] marker = new byte[2] { 0xff, 0x00 };
        

        public WaterMarking(ref Cjpeg cj)
        {
            this.cj = cj;
            error = new byte[cj.cb.b_len];

            temp = new int[3][][];
            for (int k = 0; k < 3; k++)
            {
                temp[k] = new int[cj.cb.b_len][];
                for (int i = 0; i < cj.cb.b_len; i++)
                {
                    temp[k][i] = new int[64];
                }
            }
            for (int k = 0; k < 3; k++)
            {
                for (int i = 0; i < cj.cb.b_len; i++)
                {
                    for (int j = 0; j < 64; j++)
                    {
                        temp[k][i][j] = cj.cb.data[k][i][j];
                    }
                }
            }


        }

        public void Embed(int embed_bits, int offset, int color)
        {
            UnDiffDC();
            //UnQuantize();

            ValueCombing(embed_bits, offset, color);
            EmbedHash(embed_bits, offset, color);
            //AddRand(embed_bits, offset, color);

            DiffDC();
            //Quantize();
        }


        //値を間引く
        void ValueCombing(int embed_bits, int offset, int color)
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

        void EmbedHash(int embed_bits, int offset, int color)
        {
            int bit_seek = 0;
            int array_seek = 0;
            for (int k = 0; k < color; k++)
            {
                for (int i = 0; i < cj.cb.b_len; i++)
                {
                    GetHashBuf(k, i);
                    hash_value = sha.ComputeHash(hash_buf);
                    
                    WriteHash(bw_embed, hash_value);

                    for (int j = offset; j < (embed_bits + offset); j++)
                    {
                        if ((hash_value[array_seek] & (1 << (7 - bit_seek))) != 0)
                        {
                            cj.cb.data[k][i][j] = SetValue(cj.cb.data[k][i][j], k, i, j);
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
            bw_embed.Close();
        }

        void WriteHash(BinaryWriter bw, byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                bw.Write("aaa");
                bw.Write(data[i].ToString()[0]);
                
            }
        }

        //埋込ビットが1の場合の動作
        int SetValue(int value, int color, int b_idx, int dct_idx)
        {
            if (value == 0)
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
            if (value < -1)
            {
                return (value - 1);
            }
            if (1 < value)
            {
                return (value + 1);
            }

            return 0;
        }

        void GetHashBuf(int color, int b_idx)
        {
            for (int i = 0; i < 64; i++)
            {
                hash_buf[i] = 0;
            }
            for (int i = 0; i < 64; i++)
            {
                if (cj.cb.data[color][b_idx][i] > 0)
                {
                    hash_buf[i] = (byte)cj.cb.data[color][b_idx][i];
                }
            }
        }


        public void Extract(int embed_bits, int offset, int color)
        {
            int bit_seek = 0;
            int array_seek = 0;
            byte[] extract_data = new byte[8];

            UnDiffDC();

            for (int k = 0; k < color; k++)
            {
                for (int i = 0; i < cj.cb.b_len; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        extract_data[j] = 0;
                    }
                    for (int j = offset; j < (offset + embed_bits); j++)
                    {
                        extract_data[array_seek] += (byte)(SeparateValue(k,i,j) << (7 - bit_seek));
                        bit_seek++;
                        if (bit_seek > 7)
                        {
                            array_seek++;
                            bit_seek = 0;
                        }
                    }
                    GetHashBuf(k, i);
                    hash_value = sha.ComputeHash(hash_buf);
                    error[i] += (byte)CheckHash(i, embed_bits, extract_data, hash_value);
                    bit_seek = 0;
                    array_seek = 0;
                }
            }
        }

        //ハッシュ値を比較して、値が異なれば1を返す
        public int CheckHash(int b_idx, int embed_bits, byte[] hashA, byte[] hashB)
        {
            for (int i = 0; i < embed_bits; i++)
            {
                if ((hashA[(int)(i / 8)] & (byte)(1 << (7 - (i % 8))))
                 != (hashB[(int)(i / 8)] & (byte)(1 << (7 - (i % 8)))))
                {
                    //error[b_idx]++;
                    return 1;
                }
            }

            return 0;
        }

        //透かし埋込済みDCT係数から値を取り出しつつ値を元に戻す
        public int SeparateValue(int color, int b_idx, int dct_idx)
        {
            int dst;

            if ((-1 <= cj.cb.data[color][b_idx][dct_idx]) && (cj.cb.data[color][b_idx][dct_idx] <= 1))
            {
                if (cj.cb.data[color][b_idx][dct_idx] == 0)
                {
                    return 0;
                }
                else
                {
                    cj.cb.data[color][b_idx][dct_idx] = 0;
                    return 1;
                }
            }
            if (cj.cb.data[color][b_idx][dct_idx] > 1)
            {
                dst = (cj.cb.data[color][b_idx][dct_idx] % 2);
                cj.cb.data[color][b_idx][dct_idx] -= dst;
                return dst;
            }
            if (cj.cb.data[color][b_idx][dct_idx] < -1)
            {
                dst = (-cj.cb.data[color][b_idx][dct_idx] % 2);
                cj.cb.data[color][b_idx][dct_idx] += dst;
                return dst;
            }

            return 0;
        }

        public CBitmap CheckError()
        {
            CBitmap dst = new CBitmap(cj.sof.width, cj.sof.height);
            for (int i = 0; i < dst.width; i++)
            {
                for (int j = 0; j < dst.height; j++)
                {
                    if (error[i] == 1)
                    {
                        dst.SetPixel(i * 8, (i * 8) + 7, j * 8, (j * 8) + 7, 255, 0, 0);
                    }
                }
            }
            return dst;
        }

        //void 
        //儀式用関数群
        //逆差分化
        void UnDiffDC()
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
        void DiffDC()
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
        void UnQuantize()
        {
            for (int i = 0; i < cj.cb.b_len; i++)
            {
                for (int k = 0; k < 3; k++)
                {
                    for (int j = 0; j < 64; j++)
                    {
                        cj.cb.data[k][i][j] *= cj.dqt.table[t_sel[k]][j];
                    }
                }
            }
        }
        //量子化
        void Quantize()
        {
            for (int i = 0; i < cj.cb.b_len; i++)
            {
                for (int k = 0; k < 3; k++)
                {
                    for (int j = 0; j < 64; j++)
                    {
                        cj.cb.data[k][i][j] = (int)Math.Round((double)(cj.cb.data[k][i][j] / cj.dqt.table[t_sel[k]][j]));
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
