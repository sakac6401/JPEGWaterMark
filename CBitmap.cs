using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace ConsoleApplication1
{
    public class CBitmap : CBlock
    {
        public int height;
        public int width;
        public CBitmap(Bitmap bmp)
        {
            
            this.height = bmp.Height;
            this.width = bmp.Width;

            this.data = new int[3][][];
            for (int i = 0; i < 3; i++)
            {
                data[i] = new int[width][];
                for (int j = 0; j < width; j++)
                {
                    data[i][j] = new int[height];
                }
            }

            for (int i = 0; i < this.width; i++)
            {
                for (int j = 0; j < this.height; j++)
                {
                    this.data[0][i][j] = bmp.GetPixel(i, j).R;
                    this.data[1][i][j] = bmp.GetPixel(i, j).G;
                    this.data[2][i][j] = bmp.GetPixel(i, j).B;
                }
            }

            
        }

        public CBitmap(int w, int h)
        {
            this.width = w;
            this.height = h;

            this.data = new int[3][][];
            for (int k = 0; k < 3; k++)
            {
                this.data[k] = new int[w][];
                for (int i = 0; i < w; i++)
                {
                    this.data[k][i] = new int[h];
                }
            }

            
        }

        public void CheckError(ref Cjpeg cj, int[] error, int check_count)
        {
            for (int i = 0; i < cj.cb.block_width; i++)
            {
                for (int j = 0; j < cj.cb.block_height; j++)
                {
                    if (error[i + (j * cj.cb.block_width)] >= check_count)
                    {
                        SetPixel(i * 8, j * 8, i * 8 + 8, j * 8 + 8, 0, 0, 0);
                    }
                }
            }
        }

        public Bitmap ToBitmap()
        {
            Bitmap dst = new Bitmap(this.width, this.height);
            
            for (int j = 0; j < width; j++)
            {
                for (int k = 0; k < height; k++)
                {
                    dst.SetPixel(j, k, Color.FromArgb(data[0][j][k], data[1][j][k], data[2][j][k]));
                }
            }
            
            return dst;
        }

        public void SetPixel(int x, int y, int R, int G, int B)
        {
            this.data[0][x][y] = R;
            this.data[1][x][y] = G;
            this.data[2][x][y] = B;
        }

        public void SetPixel(int fromX, int fromY, int toX, int toY, int R, int G, int B)
        {
            for(int i=fromX; i<toX; i++){
                for (int j = fromY; j < toY; j++)
                {
                    this.data[0][i][j] = R;
                    this.data[1][i][j] = G;
                    this.data[2][i][j] = B;
                }
            }
        }
    }
}
