using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ConsoleApplication1
{
    public class Cjpeg
    {
        public APP0 app0 = new APP0();
        public DQT dqt = new DQT();
        public DHT dht = new DHT();
        public SOF0 sof = new SOF0();
        public SOS sos = new SOS();
        public CBlock cb = null;
        BinaryReader br;

        public Cjpeg(string path)
        {
            br = new BinaryReader(File.Open(path, FileMode.Open));
            read_file();
            cb = new CBlock(sof.width / 8, sof.height / 8);
            br.Close();
        }

        public Cjpeg(Cjpeg prev)
        {
            app0 = new APP0(prev.app0);
            dqt = new DQT(prev.dqt);
            dht = new DHT(prev.dht);
            sof = new SOF0(prev.sof);
            sos = new SOS(prev.sos);
            cb = new CBlock(prev.cb);
        }

        public void read_file()
        {
            for (int i = 0; br.BaseStream.Position < br.BaseStream.Length; i++)
            {
                if (br.ReadByte() == 0xff)
                {
                    switch (br.ReadByte())
                    {
                        case 0xe0:
                            app0.ReadMarker(ref br);
                            break;

                        case 0xdb:
                            dqt.ReadMarker(ref br);
                            break;

                        case 0xc0:
                            sof.ReadMarker(ref br);
                            break;

                        case 0xc4:
                            dht.ReadMarker(ref br);
                            break;

                        case 0xda:
                            sos.ReadMarker(ref br);
                            break;

                        case 0xd9:
                            return;

                        default:
                            break;
                    }
                }
            }
        }
    }













}
