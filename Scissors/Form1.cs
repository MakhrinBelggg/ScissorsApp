using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ZXing;
using ZXing.QrCode;
using ZXing.Rendering;
using ZXing.Client.Result;
using ZXing.Common;
using ZXing.Windows.Compatibility;

using Emgu;
using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.CvEnum;


namespace Scissors
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();           
        }
        public static Bitmap img;
        public static string text;
        private bool isLMB = false, isPen = false, isMarker = false;
        private ArrayPoints arrayPoints = new ArrayPoints(2);
        private int timer = 0;
        Graphics g;
        Pen pen = new Pen(Color.Red, 3f);
        Pen marker = new Pen(Color.FromArgb(81, Color.Yellow), 12f);
        private class ArrayPoints
        {
            private int index = 0;
            private Point[] points;

            public ArrayPoints(int size)
            {
                if (size <= 0) size = 2;
                points = new Point[size];
            }
            public void SetPoints(int x, int y)
            {
                if (index >= points.Length)
                {
                    index = 0;
                }
                points[index] = new Point(x, y);
                index++;
            }

            public void ResetPoints()
            {
                index = 0;
            }

            public int GetCountPoints()
            {
                return index;
            }

            public Point[] GetPoints()
            {
                return points;
            }
        }

        private void CreateButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.Opacity = 0;
                this.Visible = false;
                

                panelForPictureBox.Padding = new Padding(0);
                pictureBox1.Size = new Size(584, 242);
                panelForPictureBox.Size = new Size(584, 242);
                this.MinimumSize = new Size(685, 325);
                this.Size = new Size(685, 325);
                
                img = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

                Graphics g = Graphics.FromImage(img as Image);
                Thread.Sleep(75);
                Thread.Sleep(timer * 1000);
                g.CopyFromScreen(0, 0, 0, 0, img.Size);

                if (ïğÿìîóãîëüíèêToolStripMenuItem.Checked)
                {
                    this.WindowState = FormWindowState.Normal;                  
                    Form2 photoShowing = new Form2();
                    photoShowing.ShowDialog();                   
                }

                //auto layring images to its place on screen
                if(Form2.start.Y < Form2.end.Y) // start point highter then end point
                {
                    if (Form2.start.X < Form2.end.X) // start point righter then end point
                    {
                        this.Location = new Point(Form2.start.X - 9, Form2.start.Y - 101);                            
                    }
                    else
                    {
                         this.Location = new Point(Form2.end.X - 9, Form2.start.Y - 101);                    
                    }
                }
                else 
                {
                    if (Form2.start.X < Form2.end.X)
                    {                       
                         this.Location = new Point(Form2.start.X - 9, Form2.end.Y - 101);
                    }
                    else
                    {
                        this.Location = new Point(Form2.end.X - 9, Form2.end.Y - 101);                                  
                    }                
                }
                
                //autosize form1
                if (img.Size != Screen.PrimaryScreen.Bounds.Size)
                {
                    if (img.Width > this.Width)
                    {
                        if (img.Height > this.Height)
                        {
                            this.Size = new Size(Form2.width + 17, Form2.height + 109);
                        }
                        else
                        {
                            this.Size = new Size(Form2.width + 17, 450);
                        }                     
                    }
                    else if (img.Height > this.Height)
                    {
                        this.Size = new Size(685, Form2.height + 109);
                    }
                    //pictureBox1.Size = new Size(Form2.width, Form2.height);
                    //panelForPictureBox.Size = new Size(Form2.width, Form2.height);
                    //(left, up, right, down)
                    //Padding = new Padding((this.Size.Width - pictureBox1.Size.Width) / 2, (this.Size.Height - pictureBox1.Size.Height) / 2, (this.Size.Width - pictureBox1.Size.Width) / 2, (this.Size.Height - pictureBox1.Size.Height) / 2);
                    //panelForPictureBox.Padding = new Padding((panelForPictureBox.Size.Width - Form2.width) / 2, (panelForPictureBox.Size.Height - Form2.height)/2, 0, 0);
                }
                else
                {
                    this.Size = Screen.PrimaryScreen.Bounds.Size;
                    //this.Location = new Point(0, 0);                                  
                    this.WindowState = FormWindowState.Maximized;
                }
                
                pictureBox1.Image = img;
                pictureBox1.Size = img.Size;
                //panelForPictureBox.Size = img.Size;
                pictureBox1.Visible = true;

                // calculating padding for image into panelForPictureBox
                //if (pictureBox1.Size.Width < this.Size.Width)
                //{
                //    int paddingX = (this.Size.Width - pictureBox1.Size.Width) / 2;
                //    if (pictureBox1.Size.Height < this.Size.Height)
                //    {
                //        int paddingY = (this.Size.Height - pictureBox1.Size.Height) / 2;
                //        panelForPictureBox.Padding = new Padding(paddingX, paddingY, paddingX, paddingY);
                //    }
                //    else
                //    {
                //        panelForPictureBox.Padding = new Padding(paddingX, 0, paddingX, 0);
                //    }
                //}
                //else if (pictureBox1.Size.Height < this.Size.Height)
                //{
                //    int paddingY = (this.Size.Height - pictureBox1.Size.Height) / 2;
                //    panelForPictureBox.Padding = new Padding(0, paddingY, 0, paddingY);
                //}
                


                SaveButton.Visible = true;
                CopyButton.Visible = true;
                ScanQRButton.Visible = true;
                CancelButton.Visible = true;
                CancelButton.Enabled = true;
                PenButton.Visible = true;
                EraserButton.Visible = true;
                toolStripSeparator4.Visible = true;
                MarkerButton.Visible = true;
                ScanTextButton.Visible = true;
                TimerButton.Visible = false;
                SettingsButton.Visible = false;

                MaximizeBox = true;
                menuStrip1.Visible = true;

                
                this.Visible = true;
                this.Opacity = 1;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Visible)
            {
                MaximizeBox = false;
                menuStrip1.Visible = false;
                pictureBox1.Visible = false;
                img.Dispose();
                panelForPictureBox.Padding = new Padding(0);
                pictureBox1.Size = new Size(584, 242);
                panelForPictureBox.Size = new Size(584, 242);
                this.WindowState = FormWindowState.Normal;
                this.MinimumSize = new Size(600, 325);
                this.Size = new Size(600, 325);
                //panel2.Visible = true;
                SaveButton.Visible = false;
                CopyButton.Visible = false;
                ScanQRButton.Visible = false;
                CancelButton.Enabled = false;
                PenButton.Visible = false;
                EraserButton.Visible = false;
                toolStripSeparator4.Visible = false;
                MarkerButton.Visible = false;
                ScanTextButton.Visible = false;
                TimerButton.Visible = true;
                SettingsButton.Visible = true;
            }
        }

        private void ñîçäàòüÔğàãìåíòToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateButton_Click(sender, e);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Visible)
            {
                try
                {
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        Form1.img.Save(saveFileDialog1.FileName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Îøèáêà", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(img);
        }

        public static string[] BarcodeScan(Bitmap bmp, bool scanQRCodeOnly = false)
        {
            try
            {
                BarcodeReader barcodeReader = new BarcodeReader
                {
                    AutoRotate = true,
                    TryInverted = true,
                    Options = new DecodingOptions
                    {
                        TryHarder = true
                    }
                };

                if (scanQRCodeOnly)
                {
                    barcodeReader.Options.PossibleFormats = new List<BarcodeFormat>() { BarcodeFormat.QR_CODE };
                }

                Result[] results = barcodeReader.DecodeMultiple(bmp);

                if (results != null)
                {
                    return results.Where(x => x != null && !string.IsNullOrEmpty(x.Text)).Select(x => x.Text).ToArray();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Îøèáêà", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        public static string DecodeImage(Image img)
        {
            string outString = string.Empty;
            string[] results = BarcodeScan((Bitmap)img);

            if (results != null)
            {
                outString = string.Join(Environment.NewLine + Environment.NewLine, results);
            }
            else
            {
                MessageBox.Show("QR êîä íå íàéäåí", "Âûáåğèòå äğóãóş êàğòèíêó", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return outString;
        }

        private void ScanQRButton_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            text = new string(DecodeImage(pictureBox1.Image));
            
            if (String.IsNullOrEmpty(text) || String.IsNullOrWhiteSpace(text))
            {
                Cursor = Cursors.Default;
                return;
            }

            Cursor = Cursors.Default;
            Form3 messageForm = new Form3();
            messageForm.ShowDialog();

            // ñäåëàòü ïğîâåğêó åñëè êîäà íå îáíàğóæåíî
            //label3.Text = text;
        }

        private string filePath = string.Empty;
        private string lang = Properties.Settings.Default.OCRLanguage;

        private void ScanTextButton_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                Tesseract tesseract = new Tesseract(@"C:\tessdata", lang.ToLower(), OcrEngineMode.TesseractLstmCombined);

                Image<Bgr, byte> emguImage = img.ToImage<Bgr, byte>();
                tesseract.SetImage(emguImage);
                //tesseract.SetImage(new Image<Bgr, byte>(filePath));
                tesseract.Recognize();

                text = tesseract.GetUTF8Text();
                if (String.IsNullOrEmpty(text) || String.IsNullOrWhiteSpace(text))
                {
                    MessageBox.Show("Òåêñ íå íàéäåí", "Âûáåğèòå äğóãóş êàğòèíêó", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Cursor = Cursors.Default;
                    return;
                }
                    
                Cursor = Cursors.Default;
                Form3 messageForm = new Form3();
                messageForm.ShowDialog();
                tesseract.Dispose();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Îøèáêà", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void îêíîToolStripMenuItem_Click(object sender, EventArgs e)
        {
            îêíîToolStripMenuItem.Checked = true;
            ïğÿìîóãîëüíèêToolStripMenuItem.Checked = false;

            Form2.width = Screen.PrimaryScreen.Bounds.Width;
            Form2.height = Screen.PrimaryScreen.Bounds.Height;
        }

        private void ïğÿìîóãîëüíèêToolStripMenuItem_Click(object sender, EventArgs e)
        {
            îêíîToolStripMenuItem.Checked = false;
            ïğÿìîóãîëüíèêToolStripMenuItem.Checked = true;
        }

        private void SetSize()
        {
            g = Graphics.FromImage(img);
        }
        private void PenButton_Click(object sender, EventArgs e)
        {
            isPen = true;
            isMarker = false;
            SetSize();
            pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
        }
        private void EraserButton_Click(object sender, EventArgs e)
        {
            isPen = false;
            isMarker = false;

        }
        private void MarkerButton_Click(object sender, EventArgs e)
        {
            isMarker = true;
            isPen = false;
            SetSize();
            marker.StartCap = System.Drawing.Drawing2D.LineCap.Square;
            marker.EndCap = System.Drawing.Drawing2D.LineCap.Square;
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (isPen || isMarker)
            {
                isLMB = true;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (isLMB && isPen || isLMB && isMarker)
            {
                isLMB = false;
                arrayPoints.ResetPoints();
            }
        }

        private void ÷åğíîåÏåğîToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ÷åğíîåÏåğîToolStripMenuItem.Checked = true;
            êğàñíîåÏåğîToolStripMenuItem.Checked = false;
            ñèíååÏåğîToolStripMenuItem.Checked = false;
            pen = new Pen(Color.Black, 3f);
        }

        private void êğàñíîåÏåğîToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ÷åğíîåÏåğîToolStripMenuItem.Checked = false;
            êğàñíîåÏåğîToolStripMenuItem.Checked = true;
            ñèíååÏåğîToolStripMenuItem.Checked = false;
            pen = new Pen(Color.Red, 3f);
        }

        private void ñèíååÏåğîToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ÷åğíîåÏåğîToolStripMenuItem.Checked = false;
            êğàñíîåÏåğîToolStripMenuItem.Checked = false;
            ñèíååÏåğîToolStripMenuItem.Checked = true;
            pen = new Pen(Color.Blue, 3f);
        }

        private void íàñòğîèòüToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ÷åğíîåÏåğîToolStripMenuItem.Checked = false;
            êğàñíîåÏåğîToolStripMenuItem.Checked = false;
            ñèíååÏåğîToolStripMenuItem.Checked = false;

        }

        private void áåçÇàäåğæêèToolStripMenuItem_Click(object sender, EventArgs e)
        {
            áåçÇàäåğæêèToolStripMenuItem.Checked = true;
            OneSecToolStripMenuItem.Checked = false;
            TwoSecToolStripMenuItem.Checked = false;
            ThreeSecToolStripMenuItem1.Checked = false;
            FourSecToolStripMenuItem2.Checked = false;
            FiveSecToolStripMenuItem.Checked = false;
            timer = 0;
        }

        private void OneSecToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            áåçÇàäåğæêèToolStripMenuItem.Checked = false;
            OneSecToolStripMenuItem.Checked = true;
            TwoSecToolStripMenuItem.Checked = false;
            ThreeSecToolStripMenuItem1.Checked = false;
            FourSecToolStripMenuItem2.Checked = false;
            FiveSecToolStripMenuItem.Checked = false;
            timer = 1;
        }

        private void TwoSecToolStripMenuItem_Click(object sender, EventArgs e)
        {
            áåçÇàäåğæêèToolStripMenuItem.Checked = false;
            OneSecToolStripMenuItem.Checked = false;
            TwoSecToolStripMenuItem.Checked = true;
            ThreeSecToolStripMenuItem1.Checked = false;
            FourSecToolStripMenuItem2.Checked = false;
            FiveSecToolStripMenuItem.Checked = false;
            timer = 2;
        }

        private void ThreeSecToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            áåçÇàäåğæêèToolStripMenuItem.Checked = false;
            OneSecToolStripMenuItem.Checked = false;
            TwoSecToolStripMenuItem.Checked = false;
            ThreeSecToolStripMenuItem1.Checked = true;
            FourSecToolStripMenuItem2.Checked = false;
            FiveSecToolStripMenuItem.Checked = false;
            timer = 3;
        }

        private void FourSecToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            áåçÇàäåğæêèToolStripMenuItem.Checked = false;
            OneSecToolStripMenuItem.Checked = false;
            TwoSecToolStripMenuItem.Checked = false;
            ThreeSecToolStripMenuItem1.Checked = false;
            FourSecToolStripMenuItem2.Checked = true;
            FiveSecToolStripMenuItem.Checked = false;
            timer = 4;
        }

        private void FiveSecToolStripMenuItem_Click(object sender, EventArgs e)
        {
            áåçÇàäåğæêèToolStripMenuItem.Checked = false;
            OneSecToolStripMenuItem.Checked = false;
            TwoSecToolStripMenuItem.Checked = false;
            ThreeSecToolStripMenuItem1.Checked = false;
            FourSecToolStripMenuItem2.Checked = false;
            FiveSecToolStripMenuItem.Checked = true;
            timer = 5;
        }

        private void ñîõğàíèòüÊàêToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                SaveButton_Click(sender, e);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void âûõîäToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void êîïèğîâàòüToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CopyButton_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ïåğîToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PenButton_Click(sender, e);
        }

        private void ìàğêåğToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MarkerButton_Click(sender, e);
        }

        private void ëàñòèêToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EraserButton_Click(sender, e);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_LocationChanged(object sender, EventArgs e)
        {

            Size size = SystemInformation.PrimaryMonitorSize;
            if (this.Location.X < 0)
            {
                this.Location = new Point(0, this.Location.Y);
            }
            if (this.Location.Y < 0)
            {
                this.Location = new Point(this.Location.X, 0);
            }
            if (this.Location.X + this.Size.Width > size.Width)
            {
                this.Location = new Point(size.Width - this.Size.Width, this.Location.Y);
            }
            if (this.Location.Y + this.Size.Height > size.Height)
            {
                this.Location = new Point(this.Location.X, size.Height - this.Size.Height);
            }
            
        }

        private void æåëòûéToolStripMenuItem_Click(object sender, EventArgs e)
        {
            æåëòûéToolStripMenuItem.Checked = true;
            çåëåíûéToolStripMenuItem.Checked = false;
            ğîçîâûéToolStripMenuItem.Checked = false;

            marker = new Pen(Color.FromArgb(81, Color.Yellow), 12f);
        }

        private void çåëåíûéToolStripMenuItem_Click(object sender, EventArgs e)
        {
            æåëòûéToolStripMenuItem.Checked = false;
            çåëåíûéToolStripMenuItem.Checked = true;
            ğîçîâûéToolStripMenuItem.Checked = false;

            marker = new Pen(Color.FromArgb(81, Color.LimeGreen), 12f);
        }

        private void ğîçîâûéToolStripMenuItem_Click(object sender, EventArgs e)
        {
            æåëòûéToolStripMenuItem.Checked = false;
            çåëåíûéToolStripMenuItem.Checked = false;
            ğîçîâûéToolStripMenuItem.Checked = true;

            marker = new Pen(Color.FromArgb(81, Color.HotPink), 12f);
        }

        private void íàñòğîèòüToolStripMenuItem1_Click(object sender, EventArgs e)
        {
           
        }

        private void ïàğàìåòğûToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form4 settingsForm = new Form4();
            settingsForm.ShowDialog();
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            Form4 settingsForm = new Form4();
            settingsForm.ShowDialog();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isLMB && isPen)
            {
                arrayPoints.SetPoints(e.X, e.Y);
                if (arrayPoints.GetCountPoints() >= 2)
                {                    
                    g.DrawLines(pen, arrayPoints.GetPoints());
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    arrayPoints.SetPoints(e.X, e.Y);
                }
                pictureBox1.Refresh();
            }
            else if (isLMB && isMarker)
            {
                arrayPoints.SetPoints(e.X, e.Y);
                if (arrayPoints.GetCountPoints() >= 2)
                {
                    g.DrawLines(marker, arrayPoints.GetPoints());
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    arrayPoints.SetPoints(e.X, e.Y);
                }
                pictureBox1.Refresh();
            }
        }


    }
}