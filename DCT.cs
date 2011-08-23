using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    /// <summary>
    /// 離散コサイン変換を行う
    /// </summary>
    public class DCT
    {
        /// <summary>
        /// DCTII
        /// </summary>
        static public double[][] C =
            new double[][]{
                new double[]{0.35355, 0.35355, 0.35355, 0.35355, 0.35355, 0.35355, 0.35355, 0.35355},
                new double[]{0.49039, 0.41573, 0.27779, 0.097545, -0.097545, -0.27779, -0.41573, -0.49039}, 
                new double[]{0.46194, 0.19134, -0.19134, -0.46194, -0.46194, -0.19134, 0.19134, 0.46194},
                new double[]{0.41573, -0.097545, -0.49039, -0.27779, 0.27779, 0.49039, 0.097545, -0.41573},
                new double[]{0.35355, -0.35355, -0.35355, 0.35355, 0.35355, -0.35355, -0.35355, 0.35355}, 
                new double[]{0.27779, -0.49039, 0.097545, 0.41573, -0.41573, -0.097545, 0.49039, -0.27779}, 
                new double[]{0.19134, -0.46194, 0.46194, -0.19134, -0.19134,0.46194, -0.46194, 0.19134}, 
                new double[]{0.097545, -0.27779, 0.41573, -0.49039, 0.49039, -0.41573, 0.27779, -0.097545}};
        /// <summary>
        /// DCTII逆行列
        /// </summary>
        static public double[][] Ct =
            new double[][]{
                new double[]{0.35355,0.49039,0.46194,0.41573,0.35355,0.27779,0.19134,0.097545},
                new double[]{0.35355,0.41573,0.19134,-0.097545,-0.35355,-0.49039,-0.46194,-0.27779},
                new double[]{0.35355,0.27779,-0.19134,-0.49039,-0.35355,0.097545,0.46194,0.41573},
                new double[]{0.35355,0.097545,-0.46194,-0.27779,0.35355,0.41573,-0.19134,-0.49039},
                new double[]{0.35355,-0.097545,-0.46194,0.27779,0.35355,-0.41573,-0.19134,0.49039},
                new double[]{0.35355,-0.27779,-0.19134,0.49039,-0.35355,-0.097545,0.46194,-0.41573},
                new double[]{0.35355,-0.41573,0.19134,0.097545,-0.35355,0.49039,-0.46194,0.27779},
                new double[]{0.35355,-0.49039,0.46194,-0.41573,0.35355,-0.27779,0.19134,-0.097545}};

        public static double[][] MultMatrix(double[][] A, double[][] B)
        {
            double[][] dst = new double[A.Length][];
            double buf = 0;
            for (int i = 0; i < A.Length; i++)
            {
                dst[i] = new double[A[i].Length];
            }

            for (int j = 0; j < A.Length; j++)
            {
                for (int i = 0; i < A.Length; i++)
                {
                    buf = 0;
                    for (int k = 0; k < A.Length; k++)
                    {
                        buf += A[i][k] * B[k][j];
                    }
                    dst[i][j] = buf;
                }
            }
            return dst;
        }

        public static double[][] DCT2D(double[][] A)
        {
            return MultMatrix(MultMatrix(C, A), Ct);
        }

        public static double[][] IDCT2D(double[][] A)
        {
            return MultMatrix(MultMatrix(Ct, A), C);
        }

        /// <summary>
        /// Cjpegに対してDCTを行う．
        /// 逆差分化，逆量子化は事前行っておくこと．
        /// </summary>
        /// <param name="cj">対象</param>
        /// <returns></returns>
        public static double[][] DCT2D(ref Cjpeg cj)
        {

        }

        public static void PrintMatrix(double[][] A)
        {
            for (int i = 0; i < A.Length; i++)
            {
                for (int j = 0; j < A.Length; j++)
                {
                    Console.Write(A[i][j] + ",");
                }
                Console.WriteLine();
            }
        }
    }
}
