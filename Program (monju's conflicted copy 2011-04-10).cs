using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {

            Cjpeg cj = null;
            try
            {
                cj = new Cjpeg(@"c:\88.jpg");
            }
            catch
            {
                Console.WriteLine("file cannnot open.");
                return;
            }



            for (int i = 0; i<cj.dqt.num_color; i++)
            {
                for (int j = 0; j < 64; j++)
                {
                    Console.Write("" + cj.dqt.table[i][j]+ ",");
                }
                Console.WriteLine();
            }

            for (int i = 0; i < cj.sos.cbs.data_length; i++)
            {
                Console.Write("" + cj.sos.cbs.GetBit());
            }
            Console.WriteLine("\n");

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 1; j++)
                {
                    Console.WriteLine("" + i + "," + j + "sum_nodes:" + cj.dht.table[i, j].sum_nodes);
                    Console.WriteLine("" + i + "," + j + "sum_reaves:" + cj.dht.table[i, j].sum_reaves);
                    for (int k = 0; k < cj.dht.table[i, j].sum_nodes; k++)
                    {
                        if (cj.dht.table[i, j].nodes[k].is_reaf)
                        {
                            Console.Write("" + k + " ");
                        }
                    }
                    Console.WriteLine("");

                    for (int k = 0; k < cj.dht.table[i, j].sum_nodes; k++)
                    {
                        Console.Write("" + k + " depth:" + cj.dht.table[i,j].nodes[k].depth + 
                            " child:" + cj.dht.table[i, j].nodes[k].child[0] + " " +
                            cj.dht.table[i, j].nodes[k].child[1] + " ");

                        if (cj.dht.table[i, j].nodes[k].is_reaf)
                        {
                            Console.WriteLine(cj.dht.table[i, j].nodes[k].value);
                        }
                        else
                        {
                            Console.WriteLine();
                        }
                    }

       

                }
                Console.WriteLine();
            }
                

            Console.Write("\n" + cj.sof.height + "," + cj.sof.width);
                


        }
    }
}
