using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static int CountError(int[] src, int num)
        {
            int dst = 0;

            for (int i = 0; i < src.Length; i++)
            {
                if (src[i] >= num)
                {
                    dst++;
                }
            }

            return dst;
        }
        static void Main(string[] args)
        {
            CBitmap cbmp = null;
            string passwd = "aaaaa";
            string dst_path;

            string srcPath = args[0];
            string dstPath = args[0].Replace(".jpg", "_m.jpg");

            string dst_files = "";
            Cjpeg cj_raw = null;
            FileInfo fi = null;
            Cjpeg temp = null;
            int[] err;

            //DefaultTraceListenerオブジェクトを取得
            DefaultTraceListener drl;
            drl = (DefaultTraceListener)Trace.Listeners["Default"];
            //LogFileNameを変更する
            drl.LogFileName = @"H:\log.txt";

            Console.WriteLine(
                args[0] + "\n" +
                "t:テストモード\n"+
                "e:埋め込み\n"+
                "c:チェック\n"+
                "o:ファイルチェック\n"+
                "q:終了"
                );
            Console.WriteLine("target:" + args[0]);

            while(true){

                Console.WriteLine();
                Console.Write(">");
                switch (Console.ReadLine())
                {
                    case "t":
                        try
                        {
                            cj_raw = new Cjpeg(args[0]);
                        }
                        catch
                        {
                            return;
                        }
                        CJpegDecoderT.HuffmanDecode(ref cj_raw);
                        dst_path = args[0];
                        dst_path = dst_path.Replace(".jpg", "_m.jpg");
                        WaterMarkingM.Embed(ref cj_raw, "aaaaa");
                        CJpegEncoderT.WriteFile(ref cj_raw, dst_path);

                        cj_raw = new Cjpeg(dst_path);
                        CJpegDecoderT.HuffmanDecode(ref cj_raw);
                        int[] error = WaterMarkingM.Check(ref cj_raw, "aaaaa");
                        CBitmap dst = new CBitmap(new Bitmap(args[0]));
                        dst.CheckError(ref cj_raw, error);
                        dst.ToBitmap().Save(dstPath);

                        break;

                    case "c":
                        cj_raw = new Cjpeg(dstPath);
                        CJpegDecoderT.HuffmanDecode(ref cj_raw);
                        error = WaterMarkingM.Check(ref cj_raw, "aaaaa");
                        cbmp = new CBitmap(new Bitmap(dstPath));
                        cbmp.CheckError(ref cj_raw, error);
                        cbmp.ToBitmap().Save(dstPath.Replace(".jpg",".bmp"));
                        break;

                    case "o":
                        //原画像オープン
                        cj_raw = new Cjpeg(srcPath);
                        CJpegDecoderT.HuffmanDecode(ref cj_raw);
                        cj_raw.UnDiffDC();
                        cj_raw.writeMCU(args[0].Replace(".jpg", ".csv"), cj_raw.mcuarray.MCULength, cj_raw.mcuarray.numBlock, 10);

                        //埋め込み画像
                        cj_raw = new Cjpeg(dstPath);
                        CJpegDecoderT.HuffmanDecode(ref cj_raw);
                        cj_raw.UnDiffDC();
                        cj_raw.writeMCU(args[0].Replace(".jpg", "_emb.csv"), cj_raw.mcuarray.MCULength, cj_raw.mcuarray.numBlock, 10);

                        ////再保存
                        //Process ImageMagick = new Process();
                        //ImageMagick.StartInfo.FileName = "convert";
                        //ImageMagick.StartInfo.Arguments = dstPath + " " + dstPath;
                        //Console.WriteLine("convert " + dstPath + " " + dstPath);
                        //ImageMagick.Start();

                        ////再保存埋め込み画像オープン
                        //cj_raw = new Cjpeg(dstPath);
                        //CJpegDecoderT.HuffmanDecode(ref cj_raw);
                        //cj_raw.UnDiffDC();
                        //cj_raw.writeMCU(args[0].Replace(".jpg", "_resave.csv"), cj_raw.mcuarray.MCULength, cj_raw.mcuarray.numBlock, 10);


                        break;

                    case "e":
                        cj_raw = new Cjpeg(srcPath);
                        CJpegDecoderT.HuffmanDecode(ref cj_raw);
                        WaterMarkingM.Embed(ref cj_raw, "aaaaa");
                        CJpegEncoderT.WriteFile(ref cj_raw, dstPath);
                        Console.WriteLine("wrote " + dstPath);
                        break;

                    case "i":
                        cj_raw = new Cjpeg(srcPath);
                        CJpegDecoderT.HuffmanDecode(ref cj_raw);
                        WaterMarkingM.Embed(ref cj_raw, "aaaaa");
                        break;

                    case "dt":
                        cj_raw = new Cjpeg(srcPath);
                        CJpegDecoderT.HuffmanDecode(ref cj_raw);
                        cj_raw.UnDiffDC();
                        //cj_raw.deQuantize();

                        cj_raw.Quantize();
                        cj_raw.DiffDC();
                        CJpegEncoderT.WriteFile(ref cj_raw, dstPath);

                        break;

                    case "k":
                        for (int i = 0; i < Math.Pow(3, 10); i++)
                        {
                            DCT.MultMatrix(DCT.MultMatrix(DCT.C, DCT.Ct), DCT.C);
                            if (i % 1000 == 0)
                            {
                                Console.WriteLine(i);

                            }
                        }
                        Console.WriteLine("end");
                        Console.WriteLine(Math.Pow(3, 10));
                        break;
                    case "r":
                        for (int i = 0; i < args.Length; i++)
                        {
                            cj_raw = new Cjpeg(args[i]);
                            CJpegDecoderT.HuffmanDecode(ref cj_raw);
                            fi = new FileInfo(args[i]);
                            //Directory.CreateDirectory(args[i]+"\\");
                            dst_files = args[i];
                            dst_files += " ";
                            for (int bits = 0; bits < 65; bits++)
                            {
                                temp = new Cjpeg(cj_raw);
                                WaterMarkingT.Embed(ref temp, passwd, bits, 0, 3);
                                dst_path = args[i].Substring(0, args[i].Length - 4);

                                dst_path += "\\b";
                                for (int j = 0; j < 2 - bits.ToString().Length; j++)
                                {
                                    dst_path += "0";
                                }
                                dst_path += bits.ToString() + ".jpg";
                                CJpegEncoderT.WriteFile(ref temp, dst_path);
                                Console.WriteLine(dst_path + " wrote");

                                err = WaterMarkingT.Check(ref temp, passwd, bits, 0, 3);
                                dst_files += dst_path + " ";
                            }
                            //Process exec = Process.Start("PSNR.exe", dst_files);
                        }
                        break;

                    case "z":


                        cj_raw = new Cjpeg(srcPath);
                        CJpegDecoderT.HuffmanDecode(ref cj_raw);
                        cj_raw.UnDiffDC();
                        cj_raw.deQuantize();

                        double[] Yd = new double[64];
                        Yd.Initialize();
                        for (int i = 0; i < 10; i++)
                        {
                            Yd[i] = cj_raw.dqt.table[0][i];
                        }
                        double[][] Y = DCT.IDCT2D(Zigzag.toArray(cj_raw.mcuarray.MCUs[0].DCTCoef[0]));
                        double[][] Cb = DCT.IDCT2D(Zigzag.toArray(cj_raw.mcuarray.MCUs[0].DCTCoef[1]));
                        double[][] Cr = DCT.IDCT2D(Zigzag.toArray(cj_raw.mcuarray.MCUs[0].DCTCoef[2]));

                        double[][] R;
                        double[][] G;
                        double[][] B;

                        Console.WriteLine("Yd");
                        print.Print2DMat(Zigzag.toArray(Yd));
                        Console.WriteLine("YdRe");
                        print.Print2DMat(Matrix.Round(DCT.DCT2D(Matrix.Round(DCT.IDCT2D(Zigzag.toArray(Yd))))));

                        break;

                    case "dy":
                        
                        StreamWriter sw = new StreamWriter(@"H:\jpg\log.csv");
                        StreamWriter sw_mat = new StreamWriter(@"H:\jpg\log_mat.txt");
                        Console.SetOut(sw);
                        int num_bit = 15;
                        int[] d = new int[num_bit];

                        Console.WriteLine("coef,error,parsentage");

                        for (int i = 50; i < 101; i++)
                        {
                            string openFile = args[0].Replace(".jpg", "Q" + i.ToString() + ".jpg");
                            cj_raw = new Cjpeg(openFile);
                            byte[][] dqt = Zigzag.toArray(cj_raw.dqt.table[0]);

                            int not_equal = 0;
                            d.Initialize();
                            for (int j = 0; j < (int)Math.Pow(3, num_bit); j++)
                            {
                                double[][] mkMat = Matrix.MakeMat(d);
                                double[][] buf1 = Matrix.Round(Matrix.MultComponent(mkMat, dqt));
                                double[][] buf2 = Matrix.Round(DCT.DCT2D(Matrix.Round(DCT.IDCT2D(Matrix.MultComponent(mkMat, dqt)))));
                                if (Matrix.Equal(buf1, buf2))
                                {
                                    if (!Matrix.AllZero(buf1))
                                    {
                                        Console.SetOut(sw_mat);
                                        print.Print2DMat(buf1, "mathematica");
                                        Console.WriteLine();
                                        Console.SetOut(sw);
                                    }
                                }
                                else
                                {
                                    not_equal++;
                                }

                                Matrix.ArrayIncrement(ref d, 3);
                            }
                            Console.WriteLine(i + "," + not_equal + "," + (double)(not_equal / Math.Pow(3, num_bit)));
                        }
                        
                        
                        //for (int j = 2; j < 100; j++)
                        //{
                        //    not_equal = 0;
                        //    d.Initialize();
                        //    for (int i = 0; i < (int)Math.Pow(3, num_bit); i++)
                        //    {
                        //        buf1 = Matrix.Round(Matrix.Mult(Matrix.MakeMat(d), j));
                        //        buf2 = Matrix.Round(DCT.DCT2D(Matrix.Round(DCT.IDCT2D(Matrix.Mult(Matrix.MakeMat(d),j)))));
                        //        if (!Matrix.Equal(buf1, buf2))
                        //        {
                        //            not_equal++;
                        //        }
                        //        else if(!Matrix.AllZero(buf1))
                        //        {
                        //            Console.SetOut(sw_mat);
                        //            print.Print2DMat(buf1,"mathematica");
                        //            Console.WriteLine();
                        //            Console.SetOut(sw);                                    
                        //        }

                        //        Matrix.ArrayIncrement(ref d, 3);
                        //    }
                        //    Console.WriteLine(j + "," + not_equal + "," + (double)(not_equal / Math.Pow(3, num_bit)));
                        //}

                        sw.Close();
                        sw_mat.Close();
                        Console.OpenStandardOutput();
                        
                        return;

                    //case "dy":
                    //    StreamWriter sw = new StreamWriter(@"H:\jpg\log.csv");
                    //    StreamWriter sw_mat = new StreamWriter(@"H:\jpg\log_mat.txt");
                    //    Console.SetOut(sw);
                    //    int num_bit = 6;

                    //    Console.WriteLine("coef,error,parsentage");
                    //    int[] d = new int[10];
                    //    double[][] buf1;
                    //    double[][] buf2;
                    //    int not_equal;
                    //    for (int j = 2; j < 100; j++)
                    //    {
                    //        not_equal = 0;
                    //        d.Initialize();
                    //        for (int i = 0; i < (int)Math.Pow(3, num_bit); i++)
                    //        {
                    //            buf1 = Matrix.Round(Matrix.Mult(Matrix.MakeMat(d), j));
                    //            buf2 = Matrix.Round(DCT.DCT2D(Matrix.Round(DCT.IDCT2D(Matrix.Mult(Matrix.MakeMat(d), j)))));
                    //            if (!Matrix.Equal(buf1, buf2))
                    //            {
                    //                not_equal++;
                    //            }
                    //            else if (!Matrix.AllZero(buf1))
                    //            {
                    //                Console.SetOut(sw_mat);
                    //                print.Print2DMat(buf1, "mathematica");
                    //                Console.WriteLine();
                    //                Console.SetOut(sw);
                    //            }

                    //            Matrix.ArrayIncrement(ref d, 3);
                    //        }
                    //        Console.WriteLine(j + "," + not_equal + "," + (double)(not_equal / Math.Pow(3, num_bit)));
                    //    }

                    //    sw.Close();
                    //    sw_mat.Close();
                    //    Console.OpenStandardOutput();

                    //    return;

                    case "zikken":
                            
                        int step = Convert.ToInt32(Console.ReadLine());
                        cj_raw = new Cjpeg(dstPath);
                        CJpegDecoderT.HuffmanDecode(ref cj_raw);
                        cj_raw.UnDiffDC();
                        cj_raw.deQuantize();

                        //for (int i = 0; i < 1; i++)
                        //{
                        //    cj_raw.mcuarray.MCUs[0].DCTCoef[0][i] += step;
                        //}

                        cj_raw.mcuarray.MCUs[0].DCTCoef[0][3] -= 30;
                        cj_raw.mcuarray.MCUs[0].DCTCoef[0][5] += 30;

                        CJpegEncoderT.WriteFile(ref cj_raw, dstPath);
                            
                        break;;

                    case "q":
                        return;
                }
            }
        }

        
    }
}
