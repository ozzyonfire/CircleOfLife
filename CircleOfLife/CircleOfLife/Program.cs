using System;

namespace CircleOfLife
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (CircleOfLifeGame game = new CircleOfLifeGame())
            {
                game.Run();
            }
        }
    }
#endif
}

