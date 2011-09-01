using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

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

                    case "q":
                        return;
                        //break;
                }

                //case "c":
                //    int check_count = int.Parse(Console.ReadLine());
                //    BinaryWriter bw_csv = new BinaryWriter(File.Open(args[0]+".csv", FileMode.Create));
                //    for (int i = 0; i < args.Length; i++)
                //    {
                //        int emb = int.Parse(args[i].Substring(args[i].Length - 6, 2));
                        
                //        cj_raw = new Cjpeg(args[i]);
                //        cbmp = new CBitmap(new Bitmap(args[i]));
                //        CJpegDecoderT.HuffmanDecode(ref cj_raw);
                //        err = WaterMarkingT.Check(ref cj_raw, passwd, emb, 0, 3);
                //        cbmp.CheckError(ref cj_raw, err, check_count);
                //        //string dst_name = args[i] + args[i].Substring(args[i].Length - 6, 2) + ".bmp";
                //        string dst_name = args[i] + ".bmp";
                //        bw_csv.Write("" + i + "," + CountError(err, check_count) + "\n");
                //        cbmp.ToBitmap().Save(dst_name);
                //        Console.WriteLine(args[i] + " checked");
                //    }

                //    bw_csv.Close();
                //    Console.ReadLine();
                //    break;
            }

        }
    }
}
