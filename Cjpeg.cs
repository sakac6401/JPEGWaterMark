﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ConsoleApplication1
{
    public class Cjpeg
    {
        public DQT dqt = new DQT();
        public DHT dht = new DHT();
        public APP0 app0 = null;
        public SOF0 sof = null;
        public SOS sos = null;
        public CBlock cb = null;

        public Cjpeg(string path)
        {
            BinaryReader br = new BinaryReader(File.Open(path, FileMode.Open));
            read_file(br);
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
                            //app0.ReadMarker(ref br);
                            break;

                        case 0xdb:
                            dqt.ReadMarker(ref br);
                            break;

                        case 0xc0:
                            sof = new SOF0(ref br);
                            //sof.ReadMarker(ref br);
                            break;

                        case 0xc4:
                            //dht = new DHT(ref br);
                            dht.ReadMarker(ref br);
                            break;

                        case 0xda:
                            sos = new SOS(ref br);
                            //sos.ReadMarker(ref br);
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
