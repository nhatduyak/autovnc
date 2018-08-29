namespace NPNDAutoVNC
{
    using System;
    using System.Windows.Forms;

    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            CTLConfig.GetConfiguration();
            Application.Run(new Form1());
        }
    }
}

