using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ConsoleApplication1
{
    public partial class MainForm : Form
    {
        CBitmap cbmp;
        public MainForm(CBitmap src)
        {
            InitializeComponent();
            this.pictureBox1.Image = src.ToBitmap();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        public void GetPicture(CBitmap src)
        {
            this.pictureBox1.Image = src.ToBitmap();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
