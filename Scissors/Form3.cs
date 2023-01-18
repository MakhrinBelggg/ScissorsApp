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
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
            richTextBox1.Text = Form1.text;
            richTextBox1.ReadOnly = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(richTextBox1.Text);
        }

        private void EditTextButton_Click(object sender, EventArgs e)
        {
            
            if(EditTextButton.Checked)
            {
                EditTextButton.Checked = false;
                richTextBox1.ReadOnly = true;
            }
            else
            {
                EditTextButton.Checked = true;
                richTextBox1.ReadOnly = false;
            }
        }
    }
}
