using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace SupercowTexturesTool
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            if (Process.GetProcesses().Count(p => p.ProcessName ==
                Process.GetCurrentProcess().ProcessName) > 1)
                return;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}