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

        public static double Max(double[][] A)
        {
            double max = double.MinValue;

            for (int i = 0; i < A.Length; i++)
            {
                for (int j = 0; j < A[i].Length; j++)
                {
                    if (A[i][j] > max)
                    {
                        max = A[i][j];
                    }
                }
            }

            return max;
        }

        public static double Min(double[][] A)
        {
            double min = double.MaxValue;

            for (int i = 0; i < A.Length; i++)
            {
                for (int j = 0; j < A[i].Length; j++)
                {
                    if (A[i][j] < min)
                    {
                        min = A[i][j];
                    }
                }
            }

            return min;
        }

        public static double[][] MakeMat(int[] i)
        {
            double[][] dst = new double[][]
            {
                new double[]{Sign2(i[0]),Sign2(i[1]),Sign2(i[2]),Sign2(i[3]),Sign2(i[4]),Sign2(i[5]),Sign2(i[6]),Sign2(i[7])},
                new double[]{Sign2(i[8]),Sign2(i[9]),Sign2(i[10]),Sign2(i[3]),Sign2(i[4]),Sign2(i[5]),Sign2(i[6]),Sign2(i[7])},
                new double[]{Sign2(i[16]),Sign2(i[17]),Sign2(i[18]),Sign2(i[3]),Sign2(i[4]),Sign2(i[5]),Sign2(i[6]),Sign2(i[7])},
                new double[]{Sign2(i[24]),Sign2(i[25]),Sign2(i[26]),Sign2(i[3]),Sign2(i[4]),Sign2(i[5]),Sign2(i[6]),Sign2(i[7])},
                new double[]{Sign2(i[32]),Sign2(i[33]),Sign2(i[34]),Sign2(i[3]),Sign2(i[4]),Sign2(i[5]),Sign2(i[6]),Sign2(i[7])},
                new double[]{Sign2(i[40]),Sign2(i[41]),Sign2(i[42]),Sign2(i[3]),Sign2(i[4]),Sign2(i[5]),Sign2(i[6]),Sign2(i[7])},
                new double[]{Sign2(i[48]),Sign2(i[49]),Sign2(i[50]),Sign2(i[3]),Sign2(i[4]),Sign2(i[5]),Sign2(i[6]),Sign2(i[7])},
                new double[]{Sign2(i[56]),Sign2(i[57]),Sign2(i[58]),Sign2(i[59]),Sign2(i[60]),Sign2(i[61]),Sign2(i[62]),Sign2(i[63])},
            };

            return dst;
        }

        public static double[][] MakeMat(Int64 i)
        {
            double[][] dst = new double[][]
            {
                new double[]{Sign2(i&(1<<0)), Sign2(i&(1<<1)), Sign2(i&(1<<2)), Sign2(i&(1<<3)), 0, 0, 0, 0 },
                new double[]{Sign2(i&(1<<4)), Sign2(i&(1<<5)), Sign2(i&(1<<6)), 0, 0, 0, 0, 0},
                new double[]{Sign2(i&(1<<7)), Sign2(i&(1<<8)), 0, 0, 0, 0, 0, 0},
                new double[]{Sign2(i&(1<<9)), 0, 0, 0, 0, 0, 0, 0},
                new double[]{0,0,0,0,0,0,0,0},
                new double[]{0,0,0,0,0,0,0,0},
                new double[]{0,0,0,0,0,0,0,0},
                new double[]{0,0,0,0,0,0,0,0}
            };

            return dst;
        }

        public static int Sign(Int64 N)
        {
            if (N != 0)
            {
                return 1;
            }
            return -1;
        }

        public static int Sign2(Int64 N)
        {
            if (N != 0)
            {
                return 1;
            }
            return 0;
        }

        public static int TriSign(int N)
        {
            if (N % 3 == 0)
            {
                return 0;
            }
            else if (N % 3 == 1)
            {
                return 1;
            }

            return -1;
        }

        public static void ArrayIncrement(ref int[] A, int n)
        {
            for (int i = 0; i < A.Length; i++)
            {
                A[i]++;
                if (A[i] >= n)
                {
                    A[i] = 0;
                }
                else
                {
                    break;
                }
            }
        }

        public static bool Equal(double[][] A, double[][] B)
        {
            for (int i = 0; i < A.Length; i++)
            {
                for (int j = 0; j < A[i].Length; j++)
                {
                    if (A[i][j] != B[i][j])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool AllZero(double[][] A)
        {
            for (int i = 0; i < A.Length; i++)
            {
                for (int j = 0; j < A[i].Length; j++)
                {
                    if (A[i][j] != 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static double[][] MultComponent<T1, T2>(T1[][] A, T2[][] B)
            where T1 : struct
            where T2 : struct
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
                    dst[i][j] = Convert.ToDouble(A[i][j]) * Convert.ToDouble(B[i][j]);
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
