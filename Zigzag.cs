using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    /// <summary>
    /// ジグザグ行列変換クラス
    /// </summary>
    public static class Zigzag
    {
        static int[][] zigzagmat =
        new int[][]{
            new int[]{0,1,5,6,14,15,27,28},
            new int[]{2,4,7,13,16,26,29,42},
            new int[]{3,8,12,17,25,30,41,43},
            new int[]{9,11,18,24,31,40,44,53},
            new int[]{10,19,23,32,39,45,52,54},
            new int[]{20,22,33,38,46,51,55,60},
            new int[]{21,34,37,47,50,56,59,61},
            new int[]{35,36,48,49,57,58,62,63}
        };

        /// <summary>
        /// 2次元の配列を1次元のジグザグ配列に変換する
        /// </summary>
        /// <typeparam name="Type">値型</typeparam>
        /// <param name="src">変換する2次元配列</param>
        /// <returns>変換された1次元配列</returns>
        public static Type[] toZigzag<Type>(Type[][] src)
            where Type : struct
        {
            Type[] dst = new Type[64];
            dst.Initialize();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    dst[zigzagmat[i][j]] = src[i][j];
                }
            }

            return dst;
        }

        /// <summary>
        /// 1次元のジグザグ配列を2次元の配列に変換する
        /// </summary>
        /// <typeparam name="Type">値型</typeparam>
        /// <param name="src">変換対象の1次元ジグザグ配列</param>
        /// <returns>2次元配列</returns>
        public static Type[][] toArray<Type>(Type[] src)
            where Type:struct
        {
            Type[][] dst = new Type[8][];
            for (int i = 0; i < 8; i++)
            {
                dst[i] = new Type[8];
                dst[i].Initialize();
            }

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    dst[i][j] = src[zigzagmat[i][j]];
                }
            }

            return dst;
        }

    }
}
