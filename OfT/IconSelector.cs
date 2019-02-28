using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OfT
{
    public partial class IconSelector : Form
    {

        public Form1 form1;
        public Image[] Images;

        public IconSelector()
        {
            InitializeComponent();
        }

        public void ApplyImages()
        {
            pictureBox3.Image = Images[0];
            pictureBox1.Image = Images[1];
        }

        private void PictureBox3_Click(object sender, EventArgs e)
        {
            form1.SelectedIcon(0);
            this.Close();
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            form1.SelectedIcon(1);
            this.Close();
        }

        private void Button1_Click(object sender, EventArgs e)
        {

        }
    }
}
