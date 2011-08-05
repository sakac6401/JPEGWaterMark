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
            for (int j = 0; j < numBlock; j++)
            {
                for (int k = 0; k < numDCTCoef; k++)
                {
                    buf = "";
                    for (int i = 0; i < numMCU; i++)
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
