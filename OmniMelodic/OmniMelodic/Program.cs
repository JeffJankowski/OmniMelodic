using System;

namespace OmniMelodic
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (OmniGame game = new OmniGame())
            {
                game.Run();
            }
        }
    }
#endif
}

