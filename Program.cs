using System;
using System.Windows.Forms;

namespace MazeMasters
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Form form = new Window();

            Application.Run(form);
        }
    }
}
