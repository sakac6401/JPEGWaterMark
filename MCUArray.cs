using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    public class MCUArray
    {
        /// <summary>
        /// MCU配列の長さ
        /// </summary>
        public int MCULength = 0;
        /// <summary>
        /// MCU配列
        /// </summary>
        public MCU[] MCUs = null;
        /// <summary>
        /// MCUの高さ
        /// </summary>
        public int MCUHeight = 0;
        /// <summary>
        /// MCUの幅
        /// </summary>
        public int MCUWidth = 0;
        /// <summary>
        /// MCU内の小ブロック数
        /// </summary>
        public int numBlock = 0;
        public int[] sampleRatioH = null;
        public int[] sampleRatioV = null;

        /// <summary>
        /// MCU内の番号と色番号の対応テーブル[MCU内番号]
        /// </summary>
        public int[] colorTable = null;

        public MCUArray(SOF0 sof)
        {
            MCULength = (int)Math.Ceiling(((double)sof.width / ((double)sof.SampleRatioH[0] * 8.0))) * 
                            (int)Math.Ceiling((double)sof.height / ((double)sof.SampleRatioV[0] * 8.0));
            MCUs = new MCU[MCULength];
            MCUWidth = sof.SampleRatioH[0];
            MCUHeight = sof.SampleRatioV[0];
            sampleRatioH = sof.SampleRatioH;
            sampleRatioV = sof.SampleRatioV;

            //MCU内の色数
            for (int i = 0; i < sof.numSample; i++)
            {
                numBlock += sof.SampleRatioH[i] * sof.SampleRatioV[i];
            }

            //colorTable初期化
            colorTable = new int[numBlock];
            int count = 0;
            for (int i = 0; i < numBlock; i++)
            {
                if (i >= sof.SampleRatioH[count] * sof.SampleRatioV[count])
                {
                    count++;
                }
                colorTable[i] = count;
            }

            //MCU配列作成
            for (int i = 0; i < MCULength; i++)
            {
                MCUs[i] = new MCU(numBlock);
            }
        }
    }
}
 