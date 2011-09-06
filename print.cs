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
                    Console.Write(src[i][j]);
                    Console.Write(",");
                }
                Console.WriteLine();
            }
        }

        public static void Print2DMat<Type>(Type[][] src, string t)
            where Type : struct
        {
            string buf = "";
            if (t == "mathematica")
            {
                for (int i = 0; i < src.Length; i++)
                {
                    if (i == 0)
                    {
                        buf = "{{";
                    }
                    else
                    {
                        buf = "{";
                    }
                    for (int j = 0; j < src[i].Length; j++)
                    {
                        buf += src[i][j].ToString();
                        if (j < src[i].Length - 1)
                        {
                            buf += ",";
                        }
                    }
                    if (i == src.Length - 1)
                    {
                        buf += "}}";
                    }
                    else
                    {
                        buf += "},";
                    }
                    Console.WriteLine(buf);
                }
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
