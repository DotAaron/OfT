using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace OfT
{
    public partial class Form1 : Form
    {

        private string[] Files = { "", "" };
        private string[] SafeFiles = { "", "" };

        public Form1()
        {
            InitializeComponent();
            textBox6.Text = "Copyright © _Company_ " + DateTime.Now.Year;
            pictureBox1.Image = Properties.Resources.NoProgram;
            pictureBox2.Image = Properties.Resources.NoProgram;
        }

        private void NoProgramSelectedOne_Click(object sender, EventArgs e)
        {
            SelectProgram(1);
        }

        private void NoProgramSelectedTwo_Click(object sender, EventArgs e)
        {
            SelectProgram(2);
        }

        private void SelectProgram(int program)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Title = "Select an executable file";

            open.ShowDialog();

            if(open.FileName != "")
            {
                Label one = new Label();
                PictureBox onepic = new PictureBox();

                if (program == 1)
                {
                    Files[0] = open.FileName;
                    SafeFiles[0] = open.SafeFileName;
                    one = NoProgramSelectedOne;
                    onepic = pictureBox1;
                }
                if (program == 2)
                {
                    Files[1] = open.FileName;
                    SafeFiles[1] = open.SafeFileName;
                    one = NoProgramSelectedTwo;
                    onepic = pictureBox2;
                }

                string ProgramName = open.SafeFileName;

                if(ProgramName.Length >= 22) { ProgramName = ProgramName.Remove(22, ProgramName.Length - 22) + "..."; }
                one.Text = ProgramName;

                onepic.Image = Icon.ExtractAssociatedIcon(open.FileName).ToBitmap();

                if(Files[0] != "" && Files[1] != "")
                {
                    double OneSize = new FileInfo(Files[0]).Length;
                    double SecondSize = new FileInfo(Files[1]).Length;
                    double ResultMb = 0;
                    double ResultKb = 0;

                    ResultKb = ((OneSize + SecondSize) / 1000) + 4;
                    ResultMb = ResultKb / 1000;

                    ResultKb = (int)ResultKb;

                    if(ResultMb >= 1) { sizeCalcul.Text = ResultMb.ToString(".00") + "Mb (" + ResultKb.ToString("N1", CultureInfo.CreateSpecificCulture("sv-SE")).Replace(",0", "") + "Kb)"; } else
                    {
                        sizeCalcul.Text = ResultKb.ToString(".00") + "Kb";
                    }

                    pictureBox3.Cursor = Cursors.Hand;

                }



            }

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            bool error = false;

            if (Files[0] == "" && Files[1] == "") { error = true; MessageBox.Show("Files one and two are missing.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }

            if (Files[0] == "" && !error) { error = true; MessageBox.Show("File one is missing.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            if (Files[1] == "" && !error) { error = true; MessageBox.Show("File two is missing.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }

            if (Files[0] == Files[1] && !error) { error = true; MessageBox.Show("You can't merge two identical files.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }


            if (!error)
            {
                FolderBrowserDialog folderDialog = new FolderBrowserDialog();

                folderDialog.Description = "Select a directory to save your app (App name : " + textBox1.Text.Replace(".exe", "") + ").";
                folderDialog.RootFolder = Environment.SpecialFolder.Desktop;
                folderDialog.ShowDialog();

                string[] Hidden = { HiddenOne.Checked.ToString().ToLower(), HiddenTwo.Checked.ToString().ToLower() };


                if (folderDialog.SelectedPath != "")
                {
                    Creating creating = new Creating();
                    creating.sourceName = folderDialog.SelectedPath + "/" + textBox1.Text;
                    creating.SelectedIcon = pictureBox3.Image;
                    creating.Files = Files;
                    creating.SafeFiles = SafeFiles;

                    string[] details = { textBox2.Text, textBox3.Text, textBox4.Text, textBox5.Text, textBox6.Text };

                    creating.FileDetail = details;
                    creating.Hidden = Hidden;

                    creating.encrypt = checkBox1.Checked;

                    creating.Show();

                    creating.Start();
                }
            }

        }

        private void PictureBox3_Click(object sender, EventArgs e)
        {
            if(pictureBox3.Cursor == Cursors.Hand)
            {
                IconSelector iconSelector = new IconSelector();
                Image[] img = { pictureBox1.Image, pictureBox2.Image };
                iconSelector.Images = img;
                iconSelector.form1 = this;
                iconSelector.ApplyImages();
                iconSelector.ShowDialog();
            }
        }

        public void SelectedIcon(int index, Icon custom = null)
        {
            if(custom == null)
            {
                Image[] img = { pictureBox1.Image, pictureBox2.Image };

                pictureBox3.Image = img[index];
            }
            else
            {
                pictureBox3.Image = custom.ToBitmap();
            }

        }

        private void TextBox1_Leave(object sender, EventArgs e)
        {
            if(textBox1.Text.IndexOf(".exe") == -1)
            {
                textBox1.Text += ".exe";
            }
        }

        private void Label3_Click(object sender, EventArgs e)
        {
            Process.Start("www.github.com/dotAaron/OfT");
        }

        private void GetDetails0_Click(object sender, EventArgs e) { GetFileInfo(0); }

        private void Button3_Click(object sender, EventArgs e) { GetFileInfo(1); }

        private void GetFileInfo(int index)
        {
            string name = "";
            if(index == 0) { name = "one"; } else { name = "two"; }
            if (Files[index] == "") { MessageBox.Show("File " + name + " is missing.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); } else
            {
                var versionInfo = FileVersionInfo.GetVersionInfo(Files[index]);
                textBox2.Text = GetInfo(versionInfo.FileDescription).Replace(" ", ""); ;
                textBox3.Text = GetInfo(versionInfo.FileVersion).Replace(" ", ""); ;
                textBox4.Text = GetInfo(versionInfo.ProductName).Replace(" ", ""); ;
                textBox5.Text = GetInfo(versionInfo.ProductVersion).Replace(" ", ""); ;
                textBox6.Text = GetInfo(versionInfo.LegalCopyright).Replace(" ", ""); ;
            }
        }

        private string GetInfo(string versionInfo)
        {
            if(versionInfo != null && versionInfo != "") { return versionInfo.ToString(); } else { return "Undefined"; }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();

            f.Title = "Select a file.";
            f.Filter = "Exe (*.exe)|*.exe";

            f.ShowDialog();
            this.UseWaitCursor = true;
            if (f.FileName != "")
            {
                string content = File.ReadAllText(f.FileName);

                if (content.IndexOf("OfTAppCode") != -1)
                {
                    MessageBox.Show("This file has been created with OfT.", "File created with OfT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("This file has not been created with OfT.", "File not created with OfT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            this.UseWaitCursor = false;
        }
    }
}