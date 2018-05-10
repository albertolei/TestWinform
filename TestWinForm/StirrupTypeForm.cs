using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestWinForm
{
    public partial class StirrupTypeForm : Form
    {
        public string type = "";

        public StirrupTypeForm()
        {
            InitializeComponent();
        }

        private void panel1_DoubleClick(object sender, EventArgs e)
        {
            type = "1";
            this.DialogResult = DialogResult.OK;
        }

        private void panel2_DoubleClick(object sender, EventArgs e)
        {
            type = "2";
            this.DialogResult = DialogResult.OK;
        }

        private void panel6_DoubleClick(object sender, EventArgs e)
        {
            type = "6";
            this.DialogResult = DialogResult.OK;
        }

        private void panel7_DoubleClick(object sender, EventArgs e)
        {
            type = "7";
            this.DialogResult = DialogResult.OK;
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            panel1.BorderStyle = BorderStyle.Fixed3D;
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel6.BorderStyle = BorderStyle.FixedSingle;
            panel7.BorderStyle = BorderStyle.FixedSingle;
        }
        private void panel2_Click(object sender, EventArgs e)
        {
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel2.BorderStyle = BorderStyle.Fixed3D;
            panel6.BorderStyle = BorderStyle.FixedSingle;
            panel7.BorderStyle = BorderStyle.FixedSingle;
        }
        private void panel6_Click(object sender, EventArgs e)
        {
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel6.BorderStyle = BorderStyle.Fixed3D;
            panel7.BorderStyle = BorderStyle.FixedSingle;
        }
        private void panel7_Click(object sender, EventArgs e)
        {
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel6.BorderStyle = BorderStyle.FixedSingle;
            panel7.BorderStyle = BorderStyle.Fixed3D;
        }

    }
}
