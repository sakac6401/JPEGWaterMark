using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class jpeg_struct
    {
        APP0 app0 = new APP0();
        DQT dqt = new DQT();
        DHT dht = new DHT();



    }

    /// <summary>
    /// 解像度情報
    /// </summary>
    private struct APP0
    {
        int dpiX;
        int dpiY;
    }

    /// <summary>
    /// 量子化テーブル定義
    /// </summary>
    private struct DQT
    {
        int[,] Y = new int[8, 8];
        int[,] Cb = new int[8, 8];
        int[,] Cr = new int[8, 8];
    }

    /// <summary>
    /// ハフマンテーブル定義
    /// </summary>
    private struct DHT
    {

    }

    private struct SOF0
    {
        int height;
        int width;
    }

    private struct SOS
    {
        CbitStream cbs;
    }

}
