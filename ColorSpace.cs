using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    public static class ColorSpace
    {
        public static void toRGB(double[][]  Y, double[][] Cb, double[][] Cr, out double[][] R, out double[][] G, out double[][] B)
        {
            R = Matrix.Add(Y, Matrix.Mult(Cr,1.40));
            G = Matrix.Add(Matrix.Add(Y, Matrix.Mult(Cb, -0.34)), Matrix.Mult(Cr, -0.71));
            B = Matrix.Add(Y, Matrix.Mult(Cb, 1.77));
        }
    }
}
