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
            Cjpeg cj = null;
            CBitmap cbmp = null;
            WaterMarking wm = null;
            int embed_bits = 8;
            int offset = 0;
            int color = 3;
            
            string path = @"c:\dst.jpg";
            string passwd = "aaaaa";
            string dst_path;

            string dst_files = "";
            Cjpeg cj_raw = null;
            //CJpegDecoderT.HuffmanDecode(ref cj_raw);
            FileInfo fi = null;
            Cjpeg temp = null;
            int[] err;

            switch (Console.ReadLine())
            {
                case "t":
                    cj_raw = new Cjpeg(args[0]);
                    CJpegDecoderT.HuffmanDecode(ref cj_raw);
                    dst_path = args[0];
                    dst_path = dst_path.Replace(".jpg", "_m.jpg");
                    CJpegEncoderT.WriteFile(ref cj_raw, dst_path);
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

                case "c":
                    int check_count = int.Parse(Console.ReadLine());
                    BinaryWriter bw_csv = new BinaryWriter(File.Open(args[0]+".csv", FileMode.Create));
                    for (int i = 0; i < args.Length; i++)
                    {
                        int emb = int.Parse(args[i].Substring(args[i].Length - 6, 2));
                        
                        cj_raw = new Cjpeg(args[i]);
                        cbmp = new CBitmap(new Bitmap(args[i]));
                        CJpegDecoderT.HuffmanDecode(ref cj_raw);
                        err = WaterMarkingT.Check(ref cj_raw, passwd, emb, 0, 3);
                        cbmp.CheckError(ref cj_raw, err, check_count);
                        //string dst_name = args[i] + args[i].Substring(args[i].Length - 6, 2) + ".bmp";
                        string dst_name = args[i] + ".bmp";
                        bw_csv.Write("" + i + "," + CountError(err, check_count) + "\n");
                        cbmp.ToBitmap().Save(dst_name);
                        Console.WriteLine(args[i] + " checked");
                    }

                    bw_csv.Close();
                    Console.ReadLine();
                    break;
            }

            return;

            while (true)
            {
                Console.Write(">");
                switch (Console.ReadLine())
                {
                    case "o":
                        Console.WriteLine("enter filename");
                        path = Console.ReadLine();
                        if (path == "")
                        {
                            path = @"h:\lenna512.jpg";
                            Console.WriteLine("read \"h:\\lenna512.jpg\"");
                        }
                        try
                        {
                            cj = new Cjpeg(path);
                            cbmp = new CBitmap(new Bitmap(path));
                            CJpegDecoderT.HuffmanDecode(ref cj);
                        }
                        catch
                        {
                            Console.WriteLine("read error");
                        }
                        
                        break;

                    case "w":
                        Console.WriteLine("enter filename");
                        dst_path = Console.ReadLine();
                        if (dst_path == "")
                        {
                            dst_path = @"h:\dst.jpg";
                            Console.WriteLine("write \"h:\\dst.jpg\"");
                        }
                        CJpegEncoderT.WriteFile(ref cj, dst_path);
                        break;

                    case "c":
                        wm.Extract(embed_bits, offset, color);
                        break;

                    case "e":
                        //wm.Embed(10);
                        WaterMarkingT.Embed(ref cj, "aaaaa",embed_bits, offset, color);
                        break;
                        
                    case "t":
                        Cjpeg test = new Cjpeg(cj);
                        cj = test;
                        break;

                    case "q":
                        return;

                }
            }

            




            //cjd.HuffmanDecode();
            ////wm.Embed(0);
            //wm.Extract(0);
            //wm.CheckError(ref cbmp);
            //cje.WriteFile();

            
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new MainForm(cbmp));
            
            
            

        }
    }
}
