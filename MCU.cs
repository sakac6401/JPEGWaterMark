using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    public class MCU
    {
        /// <summary>
        /// DCT係数[色番号][係数番号]
        /// </summary>
        public int[][] DCTCoef = null;
        public MCU(int numComponents)
        {

            DCTCoef = new int[numComponents][];
            for (int i = 0; i < DCTCoef.Length; i++)
            {
                DCTCoef[i] = new int[64];
            }
        }

        public int[][] ToBlock()
        {
            int[][] dst = new int[8][];
            for (int i = 0; i < 8; i++)
            {
                dst[i] = new int[8];
            }



            return dst;
        }
    }
}
