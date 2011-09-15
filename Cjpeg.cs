using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ConsoleApplication1
{
    public class Cjpeg : ICloneable
    {
        public DQT dqt = new DQT();     //量子化テーブル
        public DHT dht = new DHT();     //ハフマンテーブル
        public APP0 app0 = null;        
        public SOF0 sof = null;         //ファイルサイズ情報
        public SOS sos = null;          //
        public CBlock cb = null;
        public MCUArray mcuarray = null;
        

        public Cjpeg(string path)
        {
            BinaryReader br = null;
            try
            {
                br = new BinaryReader(File.Open(path, FileMode.Open));
            }
            catch
            {
                Console.WriteLine(path + "doesn't exist");
                return;
            }
            read_file(br);
            mcuarray = new MCUArray(this.sof);
            br.Close();
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public Cjpeg(Cjpeg prev)
        {
            app0 = new APP0(prev.app0);
            dqt = new DQT(prev.dqt);
            dht = new DHT(prev.dht);
            sof = new SOF0(prev.sof);
            sos = new SOS(prev.sos);
            mcuarray = new MCUArray(prev.mcuarray);
        }

        public void read_file(BinaryReader br)
        {
            for (int i = 0; br.BaseStream.Position < br.BaseStream.Length; i++)
            {
                if (br.ReadByte() == 0xff)
                {
                    switch (br.ReadByte())
                    {
                        case 0xe0:
                            app0 = new APP0(ref br);
                            break;

                        case 0xdb:
                            dqt.ReadMarker(ref br);
                            break;

                        case 0xc0:
                            sof = new SOF0(ref br);
                            break;

                        case 0xc4:
                            dht.ReadMarker(ref br);
                            break;

                        case 0xda:
                            sos = new SOS(ref br);
                            break;

                        case 0xd9:
                            return;

                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 逆量子化
        /// </summary>
        public void deQuantize()
        {
            for (int i = 0; i < mcuarray.MCULength; i++)
            {
                for (int j = 0; j < mcuarray.numBlock; j++)
                {
                    for (int k = 0; k < 64; k++)
                    {
                        mcuarray.MCUs[i].DCTCoef[j][k] *= dqt.table[sof.DQTSelecter[mcuarray.colorTable[j]]][k];
                    }
                }
            }
        }

        /// <summary>
        /// 量子化
        /// </summary>
        public void Quantize()
        {
            double buf;
            for (int i = 0; i < mcuarray.MCULength; i++)
            {
                for (int j = 0; j < mcuarray.numBlock; j++)
                {
                    for (int k = 0; k < 64; k++)
                    {
                        buf = (double)mcuarray.MCUs[i].DCTCoef[j][k] / dqt.table[sof.DQTSelecter[mcuarray.colorTable[j]]][k];
                        mcuarray.MCUs[i].DCTCoef[j][k] = (int)Math.Round(buf);
                    }
                }
            }
        }

        /// <summary>
        /// 逆差分化
        /// </summary>
        /// <param name="cj"></param>
        public void UnDiffDC()
        {
            int color = 0;
            for (int i = 0; i < mcuarray.MCULength; i++)
            {
                for (int j = 0; j < mcuarray.numBlock; j++)
                {
                    //最初のMCU
                    if (i == 0)
                    {
                        if (j != mcuarray.colorFirstIdx[color])
                        {
                            mcuarray.MCUs[i].DCTCoef[j][0] += mcuarray.MCUs[i].DCTCoef[j - 1][0];
                        }
                    }
                    //二個目以降のMCU
                    else
                    {
                        if (j == mcuarray.colorFirstIdx[color])
                        {
                            mcuarray.MCUs[i].DCTCoef[j][0] +=
                                mcuarray.MCUs[i - 1].DCTCoef[mcuarray.colorLastIdx[color]][0];
                        }
                        else
                        {
                            mcuarray.MCUs[i].DCTCoef[j][0] += mcuarray.MCUs[i].DCTCoef[j - 1][0];
                        }
                    }
                    if (j == mcuarray.colorLastIdx[color])
                    {
                        color++;
                    }
                }
                color = 0;
            }
        }

        /// <summary>
        /// 差分化
        /// </summary>
        public void DiffDC()
        {
            int color = 2;
            for (int i = mcuarray.MCULength - 1; i > -1; i--)
            {
                for (int j = mcuarray.numBlock - 1; j > -1; j--)
                {
                    if (i == 0)
                    {
                        if (j != mcuarray.colorFirstIdx[color])
                        {
                            mcuarray.MCUs[i].DCTCoef[j][0] -= mcuarray.MCUs[i].DCTCoef[j - 1][0];
                        }
                    }
                    else
                    {
                        if (j == mcuarray.colorFirstIdx[color])
                        {
                            mcuarray.MCUs[i].DCTCoef[j][0] -=
                                mcuarray.MCUs[i - 1].DCTCoef[mcuarray.colorLastIdx[color]][0];
                        }
                        else
                        {
                            mcuarray.MCUs[i].DCTCoef[j][0] -= mcuarray.MCUs[i].DCTCoef[j - 1][0];
                        }
                    }
                    if (j == mcuarray.colorFirstIdx[color])
                    {
                        color--;
                    }
                }
                color = 2;
            }
        }

        public void writeMCU(string path, int numMCU, int numBlock, int numDCTCoef)
        {
            StreamWriter sw = null;
            int num = 0;

            try
            {
                sw = new StreamWriter(path);
            }
            catch
            {
                while (true)
                {
                    try
                    {
                        sw = new StreamWriter(path.Replace(".csv", num.ToString() + ".csv"));
                        break;
                    }
                    catch
                    {
                        num++;
                    }
                }
            }
            string buf = "";



            for (int i = 0; i < mcuarray.MCULength; i++)
            {
                for (int j = 0; j < mcuarray.numBlock; j++)
                {
                    buf = "";
                    for (int k = 0; k < numDCTCoef; k++)
                    {
                        buf += this.mcuarray.MCUs[i].DCTCoef[j][k].ToString() + ",";
                    }
                    sw.WriteLine(buf);
                }
            }

            sw.Close();
        }
    }
}
