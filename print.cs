using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    public static class print
    {

        public static void Print2DMat<Type>(Type[][] src)
            where Type : struct
        {
            for (int i = 0; i < src.Length; i++)
            {
                for (int j = 0; j < src[i].Length; j++)
                {
                    Console.Write(" " + src[i][j]);
                }
                Console.WriteLine();
            }
        }

        public static void PrintArray<Type>(Type[] src)
            where Type : struct
        {
            for (int i = 0; i < src.Length; i++)
            {
                Console.Write(" " + src[i]);
            }
            Console.WriteLine();
        }

    }
}
