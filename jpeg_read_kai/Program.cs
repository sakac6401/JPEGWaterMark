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
            byte[] data = { 0xff, 0xfe, 0xfd, 4, 5, 6, 7, 8, 9, 10 };
            CbitStream cbs = new CbitStream(data);
            int bit;

            for (int i = 0; (bit = cbs.getBit()) != -1; i++)
            {
                Console.Write(bit);
                if (i % 8 == 7)
                {
                    Console.WriteLine();
                }
            }

            cbs.setSeek(0);

            Console.WriteLine(cbs.getBits(10));
            Console.WriteLine(cbs.getBits(10));
            Console.WriteLine(cbs.getBits(10));
            Console.WriteLine(cbs.getBits(10));
            Console.WriteLine(cbs.getBits(10));
            Console.WriteLine(cbs.getBits(10));
            Console.WriteLine(cbs.getBits(10));
            Console.WriteLine(cbs.getBits(10));
            Console.WriteLine(cbs.getBits(10));
            Console.WriteLine(cbs.getBits(10));
            Console.WriteLine(cbs.getBits(10));
            Console.WriteLine(cbs.getBits(10));
        }
    }
}
