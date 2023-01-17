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
        private bool isLMB = false, isPen = false;
        private ArrayPoints arrayPoints = new ArrayPoints(2);
        private int timer = 0;
        Graphics g;
        Pen pen = new Pen(Color.Red, 3f);
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
                img = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

                Graphics g = Graphics.FromImage(img as Image);
                Thread.Sleep(timer * 1000);
                g.CopyFromScreen(0, 0, 0, 0, img.Size);

                if (ÔˇÏÓÛ„ÓÎ¸ÌËÍToolStripMenuItem.Checked)
                {
                    Form2 photoShowing = new Form2();
                    photoShowing.ShowDialog();
                }

                //this.Size = new Size(Form2.width + 92, Form2.height + 160);
                if(img.Size != Screen.PrimaryScreen.Bounds.Size)
                {
                    if (Form2.width > 685)
                    {
                        if (Form2.height > 450)
                        {
                            this.Size = new Size(685 + (Form2.width - 685), 450 + (Form2.height - 450));
                        }
                        this.Size = new Size(685 + (Form2.width - 700), 450);
                    }
                    else if (Form2.height > 450)
                    {
                        this.Size = new Size(685, 450 + (Form2.height - 450));
                    }
                    else
                    {
                        this.Size = new Size(685, 450);
                    }
                    //(left, up, right, down)
                    pictureBox1.Margin = new Padding((this.Size.Width - pictureBox1.Size.Width) / 2, (this.Size.Height - pictureBox1.Size.Height) / 2, (this.Size.Width - pictureBox1.Size.Width) / 2, (this.Size.Height - pictureBox1.Size.Height) / 2);
                }

                pictureBox1.Size = new Size(Form2.width, Form2.height);
                pictureBox1.Padding = new Padding(0);
                pictureBox1.Margin = new Padding(3);

                pictureBox1.Image = img;
                pictureBox1.Visible = true;

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
                pictureBox1.Margin = new Padding(3);
                pictureBox1.Size = new Size(584, 242);
                this.Size = new Size(Width = 600, Height = 325);
                //panel2.Visible = true;
                SaveButton.Visible = false;
                CopyButton.Visible = false;
                ScanQRButton.Visible = false;
                //CancelButton.Visible = false;
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

        private void ÒÓÁ‰‡Ú¸‘‡„ÏÂÌÚToolStripMenuItem_Click(object sender, EventArgs e)
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
                    MessageBox.Show(ex.Message, "Œ¯Ë·Í‡", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show(ex.Message, "Œ¯Ë·Í‡", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("QR ÍÓ‰ ÌÂ Ì‡È‰ÂÌ", "¬˚·ÂËÚÂ ‰Û„Û˛ Í‡ÚËÌÍÛ", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            // Ò‰ÂÎ‡Ú¸ ÔÓ‚ÂÍÛ ÂÒÎË ÍÓ‰‡ ÌÂ Ó·Ì‡ÛÊÂÌÓ
            //label3.Text = text;
        }

        private string filePath = string.Empty;
        private string lang = "rus+eng";

        private void ScanTextButton_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                Tesseract tesseract = new Tesseract(@"C:\tessdata", lang, OcrEngineMode.TesseractLstmCombined);

                Image<Bgr, byte> emguImage = img.ToImage<Bgr, byte>();
                tesseract.SetImage(emguImage);
                //tesseract.SetImage(new Image<Bgr, byte>(filePath));
                tesseract.Recognize();

                text = tesseract.GetUTF8Text();
                if (String.IsNullOrEmpty(text) || String.IsNullOrWhiteSpace(text))
                {
                    MessageBox.Show("“ÂÍÒ ÌÂ Ì‡È‰ÂÌ", "¬˚·ÂËÚÂ ‰Û„Û˛ Í‡ÚËÌÍÛ", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show(ex.Message, "Œ¯Ë·Í‡", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ÓÍÌÓToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ÓÍÌÓToolStripMenuItem.Checked = true;
            ÔˇÏÓÛ„ÓÎ¸ÌËÍToolStripMenuItem.Checked = false;

            Form2.width = Screen.PrimaryScreen.Bounds.Width;
            Form2.height = Screen.PrimaryScreen.Bounds.Height;
        }

        private void ÔˇÏÓÛ„ÓÎ¸ÌËÍToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ÓÍÌÓToolStripMenuItem.Checked = false;
            ÔˇÏÓÛ„ÓÎ¸ÌËÍToolStripMenuItem.Checked = true;
        }

        private void SetSize()
        {
            g = Graphics.FromImage(img);
            pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
        }
        private void PenButton_Click(object sender, EventArgs e)
        {
            isPen = true;
            SetSize();
        }
        private void EraserButton_Click(object sender, EventArgs e)
        {

            isPen = false;
        }
        private void MarkerButton_Click(object sender, EventArgs e)
        {

        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (isPen)
            {
                isLMB = true;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (isLMB && isPen)
            {
                isLMB = false;
                arrayPoints.ResetPoints();
            }
        }

        private void ˜ÂÌÓÂœÂÓToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ˜ÂÌÓÂœÂÓToolStripMenuItem.Checked = true;
            Í‡ÒÌÓÂœÂÓToolStripMenuItem.Checked = false;
            ÒËÌÂÂœÂÓToolStripMenuItem.Checked = false;
            pen = new Pen(Color.Black, 3f);
        }

        private void Í‡ÒÌÓÂœÂÓToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ˜ÂÌÓÂœÂÓToolStripMenuItem.Checked = false;
            Í‡ÒÌÓÂœÂÓToolStripMenuItem.Checked = true;
            ÒËÌÂÂœÂÓToolStripMenuItem.Checked = false;
            pen = new Pen(Color.Red, 3f);
        }

        private void ÒËÌÂÂœÂÓToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ˜ÂÌÓÂœÂÓToolStripMenuItem.Checked = false;
            Í‡ÒÌÓÂœÂÓToolStripMenuItem.Checked = false;
            ÒËÌÂÂœÂÓToolStripMenuItem.Checked = true;
            pen = new Pen(Color.Blue, 3f);
        }

        private void Ì‡ÒÚÓËÚ¸ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ˜ÂÌÓÂœÂÓToolStripMenuItem.Checked = false;
            Í‡ÒÌÓÂœÂÓToolStripMenuItem.Checked = false;
            ÒËÌÂÂœÂÓToolStripMenuItem.Checked = false;

        }

        private void ·ÂÁ«‡‰ÂÊÍËToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ·ÂÁ«‡‰ÂÊÍËToolStripMenuItem.Checked = true;
            OneSecToolStripMenuItem.Checked = false;
            TwoSecToolStripMenuItem.Checked = false;
            ThreeSecToolStripMenuItem1.Checked = false;
            FourSecToolStripMenuItem2.Checked = false;
            FiveSecToolStripMenuItem.Checked = false;
            timer = 0;
        }

        private void OneSecToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            ·ÂÁ«‡‰ÂÊÍËToolStripMenuItem.Checked = false;
            OneSecToolStripMenuItem.Checked = true;
            TwoSecToolStripMenuItem.Checked = false;
            ThreeSecToolStripMenuItem1.Checked = false;
            FourSecToolStripMenuItem2.Checked = false;
            FiveSecToolStripMenuItem.Checked = false;
            timer = 1;
        }

        private void TwoSecToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ·ÂÁ«‡‰ÂÊÍËToolStripMenuItem.Checked = false;
            OneSecToolStripMenuItem.Checked = false;
            TwoSecToolStripMenuItem.Checked = true;
            ThreeSecToolStripMenuItem1.Checked = false;
            FourSecToolStripMenuItem2.Checked = false;
            FiveSecToolStripMenuItem.Checked = false;
            timer = 2;
        }

        private void ThreeSecToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ·ÂÁ«‡‰ÂÊÍËToolStripMenuItem.Checked = false;
            OneSecToolStripMenuItem.Checked = false;
            TwoSecToolStripMenuItem.Checked = false;
            ThreeSecToolStripMenuItem1.Checked = true;
            FourSecToolStripMenuItem2.Checked = false;
            FiveSecToolStripMenuItem.Checked = false;
            timer = 3;
        }

        private void FourSecToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            ·ÂÁ«‡‰ÂÊÍËToolStripMenuItem.Checked = false;
            OneSecToolStripMenuItem.Checked = false;
            TwoSecToolStripMenuItem.Checked = false;
            ThreeSecToolStripMenuItem1.Checked = false;
            FourSecToolStripMenuItem2.Checked = true;
            FiveSecToolStripMenuItem.Checked = false;
            timer = 4;
        }

        private void FiveSecToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ·ÂÁ«‡‰ÂÊÍËToolStripMenuItem.Checked = false;
            OneSecToolStripMenuItem.Checked = false;
            TwoSecToolStripMenuItem.Checked = false;
            ThreeSecToolStripMenuItem1.Checked = false;
            FourSecToolStripMenuItem2.Checked = false;
            FiveSecToolStripMenuItem.Checked = true;
            timer = 5;
        }

        private void ÒÓı‡ÌËÚ¸ ‡ÍToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void ‚˚ıÓ‰ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ÍÓÔËÓ‚‡Ú¸ToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void ÔÂÓToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PenButton_Click(sender, e);
        }

        private void Ï‡ÍÂToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MarkerButton_Click(sender, e);
        }

        private void Î‡ÒÚËÍToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EraserButton_Click(sender, e);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isLMB && isPen)
            {
                arrayPoints.SetPoints(e.X, e.Y);
                if (arrayPoints.GetCountPoints() >= 2)
                {
                    g.DrawLines(pen, arrayPoints.GetPoints());
                    arrayPoints.SetPoints(e.X, e.Y);
                }
                pictureBox1.Refresh();
            }
        }


    }
}