using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;



namespace ColorSelect
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            //this.KeyDown += new KeyEventHandler(Form1_KeyDown);
        }
        bool m_Drawing = false;
        int m_X1;
        int m_Y1;
        int m_X2;
        int m_Y2;
        Bitmap m_bmDesktop;
        Graphics m_grDesktop;

       
        private void Form1_Load(object sender, EventArgs e)
        {
            //button1.Visible = true;        
        }

		private void timer1_Tick(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(1, 1);
            using (var g=Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(System.Windows.Forms.Cursor.Position,new Point(0, 0), new Size(1, 1));
            }
            System.Drawing.Color pixel = bmp.GetPixel(0, 0);
            textBox1.Text = bmp.GetPixel(0, 0).R.ToString()+';'+bmp.GetPixel(0, 0).G.ToString()+';'+bmp.GetPixel(0, 0).B.ToString();
            textBox3.Text = bmp.GetPixel(0, 0).R.ToString() + ',' + bmp.GetPixel(0, 0).G.ToString() + ',' + bmp.GetPixel(0, 0).B.ToString();
            label9.Text = bmp.GetPixel(0, 0).R.ToString();
            label4.Text = bmp.GetPixel(0, 0).G.ToString();
            label6.Text = bmp.GetPixel(0, 0).B.ToString();
            label11.Text = bmp.GetPixel(0, 0).A.ToString();
            //label7.Text = bmp.GetPixel(0, 0).Name.ToString();
            textBox2.Text='#'+bmp.GetPixel(0, 0).Name.ToString();
            double dr = bmp.GetPixel(0, 0).R / 255;
			double dg = bmp.GetPixel(0, 0).G / 255;
			double db = bmp.GetPixel(0, 0).B / 255;
            pictureBox1.BackColor = pixel;          
            this.Invalidate();
        }
        private void button1_Click(object sender, EventArgs e)
        {
          
            timer1.Enabled = false;
            button1.Visible = false;        
            button2.Visible = false;
            linkLabel1.Visible = false;
            textBox3.Visible = false;
            panel1.Visible = false;
            label7.Visible = false;
            label11.Visible = false;
            label8.Visible = false;

            pictureBox2.Visible = true;
            button2.Enabled = true;
            pictureBox2.Dock = DockStyle.Fill;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;

            this.Hide();
            Application.DoEvents();
            m_bmDesktop = DesktopImage();
            pictureBox2.Image = (Bitmap)m_bmDesktop.Clone();


            this.pictureBox2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox2_MouseMove);
            this.pictureBox2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox2_MouseDown);
            this.pictureBox2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox2_MouseUp);
            this.pictureBox2.Cursor = Cursors.Cross;
            this.Show();

        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Space)
            {
                if (timer1.Enabled == true)
                {
                    timer1.Enabled = false;
                    this.BackColor = Color.White;
                }
                else
                {
                    timer1.Enabled = true;
                    this.BackColor = Color.WhiteSmoke;
                }
                e.Handled = true;
            }     

        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            m_Drawing = true;
            m_X1 = e.X;
            m_X2 = e.X;
            m_Y1 = e.Y;
            m_Y2 = e.Y;
            m_grDesktop = pictureBox2.CreateGraphics();
        }

        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
                     
            if (!m_Drawing)
                return;

            // Save the new point.
            m_X2 = e.X;
            m_Y2 = e.Y;

            // Redraw the desktop image.
            m_grDesktop.DrawImage(m_bmDesktop, 0, 0);

            // Draw the rubberband rectangle.
            Rectangle rect = new Rectangle(System.Math.Min(m_X1, m_X2), System.Math.Min(m_Y1, m_Y2), System.Math.Abs(m_X1 - m_X2), System.Math.Abs(m_Y1 - m_Y2));
            m_grDesktop.DrawRectangle(Pens.Blue, rect);
           
        }

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (!m_Drawing)
                    return;
                m_Drawing = false;


                // Grab the selected piece of the image.
                Rectangle rect = new Rectangle(System.Math.Min(m_X1, m_X2), System.Math.Min(m_Y1, m_Y2), System.Math.Abs(m_X1 - m_X2), System.Math.Abs(m_Y1 - m_Y2));
                Bitmap bm = (Bitmap)m_bmDesktop.Clone(rect, m_bmDesktop.PixelFormat);
                this.pictureBox2.Image = bm;

                // Save the image into the file.
                this.pictureBox2.MouseMove -= new System.Windows.Forms.MouseEventHandler(this.pictureBox2_MouseMove);
                this.pictureBox2.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.pictureBox2_MouseDown);
                this.pictureBox2.MouseUp -= new System.Windows.Forms.MouseEventHandler(this.pictureBox2_MouseUp);
                pictureBox2.Dock = DockStyle.None;
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.pictureBox2.Cursor = Cursors.Default;
                this.WindowState = FormWindowState.Normal;

                timer1.Enabled = true;
                button1.Visible = true;
                button2.Visible = true;
                linkLabel1.Visible = true;
                textBox3.Visible = true;
                panel1.Visible = true;
                label7.Visible = true;
                label11.Visible = true;
                label8.Visible = true;
            }
            catch 
            {              
            }
          
        }


        #region "Desktop Capture Routines"

        [DllImport("user32.dll")]
        static extern int GetDesktopWindow();
        [DllImport("user32.dll")]
        static extern int GetDC(int Hwnd);
        [DllImport("gdi32.dll")]
        public static extern int StretchBlt(IntPtr hDc, int x, int y,
            int nWidth, int nHeight, int hSrcDC, int xSrc,
            int ySrc, int nSrcWidth, int nSrcHeight, int dwRop);
        [DllImport("user32.dll")]
        static extern int ReleaseDC(int Hwnd, int hdc);

        private const Int32 SRCCOPY = 0xcc0020;

        private Bitmap DesktopImage()
        {
            int desktop_win = GetDesktopWindow();
            int desktop_dc = GetDC(desktop_win);
            Rectangle desktop_bounds = Screen.GetBounds(new Point(1, 1));
            int desktop_wid = desktop_bounds.Width;
            int desktop_hgt = desktop_bounds.Height;
            Bitmap bm = new Bitmap(desktop_wid, desktop_hgt);
            Graphics bm_gr = Graphics.FromImage(bm);
            IntPtr bm_hdc = bm_gr.GetHdc();
            StretchBlt(bm_hdc, 0, 0, desktop_wid, desktop_hgt, desktop_dc, 0, 0, desktop_wid, desktop_hgt, SRCCOPY);
            bm_gr.ReleaseHdc(bm_hdc);
            ReleaseDC(desktop_win, desktop_dc);
            return bm;
        }

        #endregion

       
       //Bitmap bmp1;
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {

           
            SaveFileDialog sfd = new SaveFileDialog();//yeni bir kaydetme diyaloğu oluşturuyoruz.

            sfd.Filter = "JPEG (*.jpg)|*.jpg|Bitmap(*.bmp)|*.bmp";//.bmp veya .jpg olarak kayıt imkanı sağlıyoruz.

                sfd.Title = "ColorSelectImageSave";//diğaloğumuzun başlığını belirliyoruz.

             sfd.FileName = "ImageCS";//kaydedilen resmimizin adını 'resim' olarak belirliyoruz.
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                pictureBox2.Image.Save(sfd.FileName);
                MessageBox.Show("Success");
            }
            }
            catch
            {
                MessageBox.Show("Error");
            }


        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("www.alpersahinoner.com");
        }
    }
}
