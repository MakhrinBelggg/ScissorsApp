using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Scissors
{
    public partial class Form2 : Form
    {

        public static Point start, end, now;
        public static int width;
        public static int height;
        private bool isLMB = false;
        private bool choosingFragmentFromScreenMode = false;
        //public static Bitmap imag;
        public Form2()
        {
            InitializeComponent();
            if(Form1.img == null)
            {
                MessageBox.Show("Фрагмент не найден", "Повторите попытку", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            
            pictureBox1.Image = Form1.img;
            choosingFragmentFromScreenMode = true; 
            // 1 - cut out screen fragment
            // 0 - draw blue rectangle 
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e) // нажали
        {
            isLMB = true;
            start = new Point(Cursor.Position.X, Cursor.Position.Y);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e) // отпустили
        {
            if (isLMB == false) return;

            isLMB = false;
            end = new Point(Cursor.Position.X, Cursor.Position.Y);

            width = Math.Abs(start.X - end.X);
            height = Math.Abs(start.Y - end.Y);
            if (width == 0 && height == 0) return;
            try
            {
                Form1.img = Form1.img.Crop(new Rectangle(Math.Min(start.X, now.X), Math.Min(start.Y, now.Y), width, height));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            this.Visible = false;
            this.Close();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isLMB == false) return;

            now = e.Location;
            Refresh();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
           
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if(choosingFragmentFromScreenMode) // settings
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, 132, 132, 132)), new Rectangle(0, 0, this.Size.Width, this.Size.Height));

                if (!isLMB) return;

                Rectangle rect = GetRect();

                if (rect.IsEmpty || rect.Width == 0 && rect.Height == 0) return;
                try
                {
                    e.Graphics.DrawImage(Form1.img, 0, 0);
                    e.Graphics.ExcludeClip(rect);
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, 132, 132, 132)), new Rectangle(0, 0, this.Size.Width, this.Size.Height));

                    //e.Graphics.FillRectangle(b, rect);
                    //e.Graphics.DrawRectangle(p, rect);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }           
            else 
            {
                if (!isLMB) return;

                Brush b = new SolidBrush(Color.FromArgb(72, 174, 225, 238));
                Pen p = new Pen(Color.FromArgb(150, 174, 225, 238), 1.3f);
                Rectangle rect = GetRect();

                if (rect.IsEmpty || rect.Width == 0 && rect.Height == 0) return;
                try
                {
                    e.Graphics.FillRectangle(b, rect);
                    e.Graphics.DrawRectangle(p, rect);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = Graphics.FromImage(Form1.img);
            g.FillRectangle(new SolidBrush(Color.FromArgb(50, 192, 192, 192)), new Rectangle(0, 0, this.Size.Width, this.Size.Height));
            //e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(50, 192, 192, 192)), new Rectangle(0, 0, this.Size.Width, this.Size.Height));
        }

        private Rectangle GetRect()
        {
            Rectangle rect = new Rectangle();
            rect.X = Math.Min(start.X, now.X);
            rect.Y = Math.Min(start.Y, now.Y);
            rect.Width = Math.Abs(start.X - now.X);
            rect.Height = Math.Abs(start.Y - now.Y);

            return rect;
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            //this.Close();
        }
    }
}
