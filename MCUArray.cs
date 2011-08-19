﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    /// <summary>
    /// 
    /// </summary>
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
        /// 画像縦方向のMCUの個数
        /// </summary>
        public int MCUHeight = 0;
        /// <summary>
        /// 画像横方向のMCUの個数
        /// </summary>
        public int MCUWidth = 0;
        /// <summary>
        /// MCU内の小ブロック数
        /// </summary>
        public int numBlock = 0;


        const int SUB111x111 = 0;
        const int SUB211x111 = 1;
        const int SUB111x211 = 2;
        const int SUB211x211 = 3;
        public int SubSamplingPattern = 0;

        /// <summary>
        /// MCU中のY成分の個数
        /// </summary>
        public int numY = 0;

        /// <summary>
        /// Y成分の水平方向の数
        /// </summary>
        public int HY = 0;

        /// <summary>
        /// Y成分の垂直方向の数
        /// </summary>
        public int VY = 0;
        
        public int[] sampleRatioH = null;
        public int[] sampleRatioV = null;

        /// <summary>
        /// MCU内の番号と色番号の対応テーブル[MCU内番号]
        /// </summary>
        public int[] colorTable = null;

        /// <summary>
        /// MCU内で各色が初めて出る小ブロック番号[色番号]
        /// </summary>
        public int[] colorFirstIdx = null;

        public int[] colorLastIdx = null;

        public MCUArray(SOF0 sof)
        {
            MCUWidth = (int)Math.Ceiling((double) sof.width / (8 * sof.SampleRatioH[0]));
            MCUHeight = (int)Math.Ceiling((double)sof.height / (8 * sof.SampleRatioV[0]));
            MCULength = MCUWidth * MCUHeight;
            MCUs = new MCU[MCULength];
            sampleRatioH = sof.SampleRatioH;
            sampleRatioV = sof.SampleRatioV;
            HY = sampleRatioH[0];
            VY = sampleRatioV[0];

            SubSamplingPattern = CheckSubSamplingPattern();


            //MCU内の色数
            for (int i = 0; i < sof.numSample; i++)
            {
                numBlock += sof.SampleRatioH[i] * sof.SampleRatioV[i];
            }

            numY = sof.SampleRatioH[0] * sof.SampleRatioV[0];

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

            //colorFirstIdx処理
            colorFirstIdx = new int[colorTable[colorTable.Length - 1] + 1];
            count = 0;
            for (int i = 0; i < numBlock; i++)
            {
                if (count == colorTable[i])
                {
                    colorFirstIdx[count] = i;
                    count++;
                }
            }

            //colorLastIdx処理
            colorLastIdx = new int[colorTable[colorTable.Length - 1] + 1];
            count = 0;
            for (int i = 0; i < numBlock-1; i++)
            {
                if ((count == colorTable[i]) && (count != colorTable[i + 1]))
                {
                    colorLastIdx[count] = i;
                    count++;
                }
            }
            colorLastIdx[colorLastIdx.Length - 1] = colorTable.Length - 1;
        

            //MCU配列作成
            for (int i = 0; i < MCULength; i++)
            {
                MCUs[i] = new MCU(numBlock);
            }
        }

        //コピーコンストラクタ
        public MCUArray(MCUArray prev)
        {
            this.colorTable = new int[prev.colorTable.Length];
            this.MCUHeight = prev.MCUHeight;
            this.MCULength = prev.MCULength;
            this.MCUs = new MCU[MCULength];
            prev.MCUs.CopyTo(this.MCUs, 0);
            this.MCUWidth = prev.MCUWidth;
            this.numBlock = prev.numBlock;
            this.sampleRatioH = new int[prev.sampleRatioH.Length];
            prev.sampleRatioH.CopyTo(this.sampleRatioH, 0);
            this.sampleRatioV = new int[prev.sampleRatioV.Length];
            prev.sampleRatioV.CopyTo(this.sampleRatioV, 0);
        }

        public int CheckSubSamplingPattern()
        {
            if (this.sampleRatioH[0] == 1 && this.sampleRatioH[1] == 1 && this.sampleRatioH[2] == 1)
            {
                if (this.sampleRatioV[0] == 1 && this.sampleRatioV[1] == 1 && this.sampleRatioV[2] == 1)
                {
                    return SUB111x111;
                }
            }
            if (this.sampleRatioH[0] == 2 && this.sampleRatioH[1] == 1 && this.sampleRatioH[2] == 1)
            {
                if (this.sampleRatioV[0] == 1 && this.sampleRatioV[1] == 1 && this.sampleRatioV[2] == 1)
                {
                    return SUB211x111;
                }
            }
            if (this.sampleRatioH[0] == 1 && this.sampleRatioH[1] == 1 && this.sampleRatioH[2] == 1)
            {
                if (this.sampleRatioV[0] == 2 && this.sampleRatioV[1] == 1 && this.sampleRatioV[2] == 1)
                {
                    return SUB111x211;
                }
            }
            if (this.sampleRatioH[0] == 2 && this.sampleRatioH[1] == 1 && this.sampleRatioH[2] == 1)
            {
                if (this.sampleRatioV[0] == 2 && this.sampleRatioV[1] == 1 && this.sampleRatioV[2] == 1)
                {
                    return SUB211x211;
                }
            }
            return -1;
        }
    }
}
 