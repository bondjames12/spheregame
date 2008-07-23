using System;

namespace Sphere
{
    static class Main
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Sphere game = new Sphere())
            {
                game.Run();
            }
        }
    }
}

