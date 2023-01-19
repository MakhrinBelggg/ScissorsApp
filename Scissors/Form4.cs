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
    public partial class Form4 : Form
    {

        public static string lang = Properties.Settings.Default.OCRLanguage;
        public Form4()
        {
            InitializeComponent();
            checkBox1.Checked = Properties.Settings.Default.copyImmediatelyMode;
            checkBox2.Checked = Properties.Settings.Default.choosingFragmentFromScreenMode;
            toolStripComboBox1.Text = Properties.Settings.Default.OCRLanguage;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.choosingFragmentFromScreenMode = checkBox2.Checked;
            Properties.Settings.Default.Save();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Изменения сохранены", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.copyImmediatelyMode = checkBox2.Checked;
            Properties.Settings.Default.Save();
        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {
            
        }

        private void toolStripComboBox1_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.OCRLanguage = toolStripComboBox1.Text;
            Properties.Settings.Default.Save();
        }
    }
}
