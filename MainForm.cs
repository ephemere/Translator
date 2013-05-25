using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Translator
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Транслятор из ERM-модели в UML-модель.\nАнастасия М. Данейко\nФакультет информатики\n2013 год",
                "О программе", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOpenErm_Click(object sender, EventArgs e)
        {
            String fileName = "";
            openFileDialog1.Filter = "ERM-model file (*.ermmdsl)|*.ermmdsl|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.FileName = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileName = openFileDialog1.FileName;
                edtErmName.Text = fileName;
            }
        }

        private void btnSaveUml_Click(object sender, EventArgs e)
        {
            String fileName = "";
            saveFileDialog1.Filter = "UML-model file (*.uml)|*.uml|All Files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.FileName = "";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileName = saveFileDialog1.FileName;
                edtUmlName.Text = fileName;
            }
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnOpenErm_Click(sender, e);
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnSaveUml_Click(sender, e);
        }
    }
}
