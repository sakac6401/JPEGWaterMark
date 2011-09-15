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

    //逆さ文化
    //間引く
    //ハッシュ入力容易
    //ハッシュ生成
    //ハッシュ埋め込み

    //Caution! 0915
    //GetHashKey書き換えた
    //Caution!

    /// <summary>
    /// ハッシュ値を埋め込む．画像のサイズは縦横ともに32の倍数であることを仮定
    /// </summary>
    public class WaterMarkingDC
    {
        const int emb_pix = 1;
        const int key_len = 4096;
        const int PROCWIDTH = 4;
        const int PROCHEIGHT = 4;

        /// <summary>
        /// フラジャイル電子透かしを埋め込む埋め込み
        /// </summary>
        /// <param name="cj">埋め込み対象Cjpegクラス（参照入力）</param>
        /// <param name="passwd">鍵</param>
        public static void Embed(ref Cjpeg cj, string passwd)
        {
            cj.UnDiffDC();

            //cj.deQuantize();

            Cjpeg temp = (Cjpeg)cj.Clone();

            ValueCombing(ref cj);

            EmbedHash(ref cj, ref temp, passwd);

            //cj.Quantize();

            cj.DiffDC();
        }

        /// <summary>
        /// Y成分のDCT係数を間引く
        /// </summary>
        /// <param name="cj">間引対象CJpegクラス</param>
        /// <param name="embed_bits">埋め込みビット数</param>
        static void ValueCombing(ref Cjpeg cj)
        {
            for (int i = 0; i < cj.mcuarray.MCULength; i++)
            {
                for (int j = 0; j < cj.mcuarray.numBlock; j++)
                {
                    if ((-1 <= cj.mcuarray.MCUs[i].DCTCoef[j][0]) && (cj.mcuarray.MCUs[i].DCTCoef[j][0] <= 1))
                    {
                        cj.mcuarray.MCUs[i].DCTCoef[j][0] = 0;
                    }
                    else
                    {
                        cj.mcuarray.MCUs[i].DCTCoef[j][0] += (cj.mcuarray.MCUs[i].DCTCoef[j][0] % 2);
                    }
                }
            }
        }


        /// <summary>
        /// ハッシュ値を埋め込む
        /// </summary>
        /// <param name="cj">値を間引いた埋め込み対象CJpegクラス</param>
        /// <param name="temp">値を間引く前の埋め込み対象Cjpegクラス</param>
        /// <param name="passwd">鍵</param>
        static void EmbedHash(ref Cjpeg cj, ref Cjpeg temp, string passwd)
        {
            byte[] hash_key;
            byte[] hash_value;
            SHA256 sha = SHA256.Create();
            CbitStream embed_data = null;
            Random rand = new Random(DateTime.Now.Millisecond);
            int idx=0;

            for (int j = 0; j < cj.mcuarray.MCUHeight * cj.mcuarray.VY / PROCHEIGHT; j++)
            {
                for (int i = 0; i < cj.mcuarray.MCUWidth * cj.mcuarray.HY / PROCWIDTH; i++)
                {
                    hash_key = GetHashKey(ref cj, passwd, 0, i, j);
                    hash_value = sha.ComputeHash(hash_key);
                    
                    embed_data = new CbitStream(hash_value);

                    for (int y = 0; y < 4 / cj.mcuarray.VY; y++)
                    {
                        for (int x = 0; x < 4 / cj.mcuarray.HY; x++)
                        {
                            for (int l = 0; l < cj.mcuarray.numBlock; l++)
                            {
                                if (embed_data.GetBit() != 0)
                                {
                                    idx = (i * 4 / cj.mcuarray.HY) + x + (y * cj.mcuarray.MCUWidth) + (j * 4 * cj.mcuarray.MCUWidth / cj.mcuarray.VY);
                                    SetValue(ref cj, ref temp, idx, l, 0, rand);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 埋め込みビット"1"を指定箇所に埋め込む
        /// </summary>
        /// <param name="cj">埋め込み対象Cjpeg</param>
        /// <param name="temp">間引前のCjpeg</param>
        /// <param name="Yidx">埋め込み対象Y成分</param>
        /// <param name="MCUidx">埋め込み対象のMCU番号</param>
        /// <param name="DCTidx">埋め込み対象のDCT係数</param>
        /// <param name="rand">乱数</param>
        static void SetValue(ref Cjpeg cj, ref Cjpeg temp, int MCUidx, int Yidx, int DCTidx, Random rand)
        {
            int depth = 1;
            //if (Yidx == 0)
            //{
            //    depth = cj.dqt.table[0][0];
            //}
            //else
            //{
            //    depth = cj.dqt.table[1][0];
            //}
            //埋め込み対象のDCT係数が0の時
            if(cj.mcuarray.MCUs[MCUidx].DCTCoef[Yidx][DCTidx] == 0)
            {
                //間引前のDCT係数を参照し，０でないならその値にする
                if (temp.mcuarray.MCUs[MCUidx].DCTCoef[Yidx][DCTidx] != 0)
                {
                    cj.mcuarray.MCUs[MCUidx].DCTCoef[Yidx][DCTidx] =
                        temp.mcuarray.MCUs[MCUidx].DCTCoef[Yidx][DCTidx];
                }
                else
                {
                    if (rand.Next(0,2) % 2 == 0)
                    {
                        cj.mcuarray.MCUs[MCUidx].DCTCoef[Yidx][DCTidx] += depth;
                    }
                    else
                    {
                        cj.mcuarray.MCUs[MCUidx].DCTCoef[Yidx][DCTidx] -= depth;
                    }
                }
                return;
            }
            if (cj.mcuarray.MCUs[MCUidx].DCTCoef[Yidx][DCTidx] < -1)
            {
                cj.mcuarray.MCUs[MCUidx].DCTCoef[Yidx][DCTidx] -= depth;
                return;
            }
            if (1 < cj.mcuarray.MCUs[MCUidx].DCTCoef[Yidx][DCTidx])
            {
                cj.mcuarray.MCUs[MCUidx].DCTCoef[Yidx][DCTidx] += depth;
                return;
            }
        }

        /// <summary>
        /// 埋め込み対象ブロックのハッシュ入力を出力
        /// </summary>
        /// <param name="cj">対象Cjpeg</param>
        /// <param name="passwd">パスフレーズ</param>
        /// <param name="color"></param>
        /// <param name="b_x">埋め込み対象ブロックの横方向座標</param>
        /// <param name="b_y">埋め込み対象ブロックの縦方向座標</param>
        /// <returns></returns>
        static byte[] GetHashKey(ref Cjpeg cj, string passwd, int color, int b_x, int b_y)
        {
            byte[] buf = new byte[key_len];
            buf.Initialize();
            byte[] pass = StringToByte(passwd);
            byte[] dst;

            int count = 0;
            int idx;
            while (count < 1024)
            {
                for (int y = 0; y < 4 / cj.mcuarray.VY; y++)
                {
                    for (int x = 0; x < 4 / cj.mcuarray.HY; x++)
                    {
                        for (int l = 0; l < cj.mcuarray.numBlock; l++)
                        {
                            //for (int k = 0; k < 64; k++)
                            for (int k = 0; k < 64; k++)
                            {
                                idx = (b_x * 4 / cj.mcuarray.HY) + x + (y * cj.mcuarray.MCUWidth) + (b_y * 4 * cj.mcuarray.MCUWidth/ cj.mcuarray.VY);
                                //buf[count] = (byte)(cj.mcuarray.MCUs[idx].DCTCoef[l][k] - (cj.mcuarray.MCUs[idx].DCTCoef[l][k] % 2));
                                buf[count] = (byte)(cj.mcuarray.MCUs[idx].DCTCoef[l][k]);
                                count++;
                            }
                        }
                    }
                }
            }
            
            buf[buf.Length - 7] = (byte)((cj.sof.width & 0xff00) >> 8);
            buf[buf.Length - 6] = (byte)(cj.sof.width & 0xff);
            buf[buf.Length - 5] = (byte)((cj.sof.height & 0xff00) >> 8);
            buf[buf.Length - 4] = (byte)(cj.sof.height & 0xff);
            buf[buf.Length - 3] = (byte)color;
            buf[buf.Length - 2] = (byte)(((b_x * b_y) & 0xff00) >> 8);
            buf[buf.Length - 1] = (byte)((b_x * b_y) & 0xff);

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

        /// <summary>
        /// 改ざんチェック関数
        /// </summary>
        /// <param name="cj">チェック対象Cjpegクラス</param>
        /// <param name="passwd">鍵</param>
        /// <returns></returns>
        public static int[] Check(ref Cjpeg cj, string passwd)
        {
            Cjpeg temp = (Cjpeg)cj.Clone();
            int[] error = new int[cj.mcuarray.MCUWidth * cj.mcuarray.HY * cj.mcuarray.MCUHeight * cj.mcuarray.VY / 16];
            error.Initialize();

            int err_count = 0;
            CbitStream cbs_extr = new CbitStream(32);
            CbitStream cbs_calc = null;
            SHA256 sha = SHA256.Create();
            int idx = 0;

            temp.UnDiffDC();

            for (int j = 0; j < cj.mcuarray.MCUHeight * cj.mcuarray.VY / 4; j++)
            {
                for (int i = 0; i < cj.mcuarray.MCUWidth * cj.mcuarray.HY/4; i++)
                {
                    cbs_extr.Initialize();

                    //ブロック処理
                    for (int y = 0; y < 4 / cj.mcuarray.VY; y++)
                    {
                        for (int x = 0; x < 4 / cj.mcuarray.HY; x++)
                        {
                            for (int l = 0; l < cj.mcuarray.numBlock; l++)
                            {
                                idx = (i * 4 / cj.mcuarray.HY) + x + (y * cj.mcuarray.MCUWidth) + (j * 4 * cj.mcuarray.MCUWidth / cj.mcuarray.VY);
                                DataSeparation(ref temp, ref cbs_extr, emb_pix, idx, l);
                            }
                        }
                    }
                    
                    cbs_calc = new CbitStream(sha.ComputeHash(GetHashKey(ref temp, passwd, 0, i, j)));

                    if (!CheckCbs(cbs_calc, cbs_extr, 48))
                    {
                        err_count++;
                        error[i + j * (cj.mcuarray.MCUWidth * cj.mcuarray.HY / 4)] = 1;
                    }
                }
            }
            Console.WriteLine("error_count=" + err_count);
            return error;
        }

        /// <summary>
        /// DCT係数と埋め込みビットを分離する
        /// </summary>
        /// <param name="cj">分離対象Cjpegクラス</param>
        /// <param name="cbs">分離したビットを格納するCbitStreamクラス</param>
        /// <param name="embed_bits">埋め込みビット数</param>
        /// <param name="MCUidx">分離対象MCU番号</param>
        /// <param name="Yidx"></param>
        public static void DataSeparation(ref Cjpeg cj, ref CbitStream cbs, int embed_bits, int MCUidx, int Yidx)
        {
            int val = 0;

            val = Math.Abs(cj.mcuarray.MCUs[MCUidx].DCTCoef[Yidx][0] % 2);
            cbs.CatchBit(val);
            cj.mcuarray.MCUs[MCUidx].DCTCoef[Yidx][0] -= (cj.mcuarray.MCUs[MCUidx].DCTCoef[Yidx][0] % 2);
        }

        /// <summary>
        /// CBS同士の比較
        /// 同一ならtrue，異なるならfalseを返す
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="embed_bits"></param>
        /// <returns></returns>
        static bool CheckCbs(CbitStream A, CbitStream B, int embed_bits)
        {

            for (int i = 0; i < embed_bits; i++)
            {
                if ( (A.data[(int)(i / 8)] & (byte)(1 << 7 - (i % 8))) !=
                     (B.data[(int)(i / 8)] & (byte)(1 << 7 - (i % 8))) )
                {
                    return false;
                }
            }
            return true;
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

        static int NearestMultipleofEight(int a, int EvenOdd)
        {
            if (EvenOdd == 0)
            {
                return (int)Math.Ceiling((double)a / 8);
            }
            return (int)Math.Floor((double)a / 8);
            
        }
    }
}
