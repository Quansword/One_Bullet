using System;

namespace OneBullet
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            GunManager CL = new GunManager();

            using (var game = new GunManager())
                game.Run();
        }
    }
}
