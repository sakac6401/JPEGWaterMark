using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    public static class Matrix
    {
        public static double[][] Add(double[][] A, double[][] B)
        {
            double[][] dst = new double[A.Length][];
            for (int i = 0; i < A.Length; i++)
            {
                dst[i] = new double[A[i].Length];
                dst.Initialize();
            }

            for (int i = 0; i < A.Length; i++)
            {
                for (int j = 0; j < A[i].Length; j++)
                {
                    dst[i][j] = A[i][j] + B[i][j];
                }
            }

            return dst;
        }

        public static double[][] Add(double[][] A, double B)
        {
            double[][] dst = new double[A.Length][];
            for (int i = 0; i < A.Length; i++)
            {
                dst[i] = new double[A[i].Length];
                dst.Initialize();
            }

            for (int i = 0; i < A.Length; i++)
            {
                for (int j = 0; j < A[i].Length; j++)
                {
                    dst[i][j] = A[i][j] + B;
                }
            }

            return dst;
        }

        public static void Add(ref double[][] A, double B)
        {
            for (int i = 0; i < A.Length; i++)
            {
                for (int j = 0; j < A[i].Length; j++)
                {
                    A[i][j] += B;
                }
            }
        }

        public static double[][] Mult(double[][] A, double B)
        {
            double[][] dst = new double[A.Length][];
            for (int i = 0; i < A.Length; i++)
            {
                dst[i] = new double[A[i].Length];
                dst.Initialize();
            }

            for (int i = 0; i < A.Length; i++)
            {
                for (int j = 0; j < A[i].Length; j++)
                {
                    dst[i][j] = A[i][j] * B;
                }
            }

            return dst;
        }

        public static double[][] Round(double[][] A)
        {
            double[][] dst = new double[A.Length][];
            for (int i = 0; i < dst.Length; i++)
            {
                dst[i] = new double[A[i].Length];
                dst[i].Initialize();
            }

            for (int i = 0; i < A.Length; i++)
            {
                for (int j = 0; j < A[i].Length; j++)
                {
                    dst[i][j] = Math.Round(A[i][j]);
                }
            }

            return dst;
        }
    }
}
