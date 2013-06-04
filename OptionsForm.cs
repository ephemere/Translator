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
    public partial class OptionsForm : Form
    {

        private String path = "";
        private String prevPath = "";

        public OptionsForm()
        {
            InitializeComponent();
        }


        public String getPath()
        {
            return path;
        }

        public void setPath(String value)
        {
            path = value;
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            edtXslPath.Text = path;
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "XSL-file (*.xsl)|*.xsl|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.FileName = path;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                prevPath = path;
                path = openFileDialog1.FileName;
                edtXslPath.Text = path;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            path = prevPath;
            DialogResult = DialogResult.Cancel;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
