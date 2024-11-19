using System;
using System.Windows.Forms;

namespace LibraryManagementSystems
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Start the application with AdminLogin form
            Application.Run(new AdminLogin());
        }
    }
}
