using System;

namespace Poseidon
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (PoseidonGame game = new PoseidonGame())
            {
                game.Run();
            }
        }
    }
#endif
}

