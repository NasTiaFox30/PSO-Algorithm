namespace PSO_Algorithm
{
    internal static class Program
    {
        /// <summary>
        ///  Start progamu
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new PSOVisualization());

        }
    }
}