using System;
using System.Diagnostics;

namespace Sphere
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            ConfigureEnvironment();
            //TODO: In a real-world game, this would be a try/catch block with an exception logger.
            using (Game game = new Game())
            {
                game.Run();
            }
        }

        /// <summary>
        /// Configures the run-time environment for all platforms.
        /// </summary>
        static private void ConfigureEnvironment()
        {
#if !XBOX
            ConfigureWindowsEnvironment();
#else
            ConfigureXboxEnvironment();
#endif
            
        }

#if !XBOX
        /// <summary>
        /// Configures the run-time environment for the Windows platform.
        /// </summary>
        [Conditional("WINDOWS")]
        static private void ConfigureWindowsEnvironment()
        {
            // @todo:  We really need to find a better way to get at the executing directory.
            System.Environment.CurrentDirectory = System.Reflection.Assembly.GetExecutingAssembly().Location.Replace("Sphere.exe", "");
        }
#else
        /// <summary>
        /// Configures the run-time environment for the Xbox platform.
        /// </summary>
        [Conditional("XBOX")]
        static private void ConfigureXboxEnvironment()
        {
        }
#endif


    }
}

