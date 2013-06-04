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

        public Boolean isAttr = true;
        public Boolean isClass = false;

        public OptionsForm()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            isAttr = rbAttr.Checked;
            isClass = rbClass.Checked;
            DialogResult = DialogResult.OK;
        }
    }
}
